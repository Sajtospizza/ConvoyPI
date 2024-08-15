using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Diagnostics;

namespace OptimizerFrontend.BackendLib
{
    internal class Optimizer(int resourceInterval, int resourceQueueLength,int takeAwayTime, int productQueueLength,int factory1ProcessTime, int factory2ProcessTime, int fact1in, int fact1out, int fact2in, int fact2out)
    {
        // Base values
        public int ResourceInterval { get; set; } = resourceInterval;
        public int ResourceQueueLength { get; set; } = resourceQueueLength;
        public int TakeAwayTime { get; set; } = takeAwayTime;
        public int ProductQueueLength { get; set; } = productQueueLength;
        public int Factory1ProcessTime { get; set; } = factory1ProcessTime;
        public int Factory2ProcessTime { get; set; } = factory2ProcessTime;
        public int Factory1InputQueueLength { get; set; } = fact1in;
        public int Factory1OutputQueueLength { get; set; } = fact1out;
        public int Factory2InputQueueLength { get; set; } = fact2in;
        public int Factory2OutputQueueLength { get; set; } = fact2out;

        // Lists and queues
        public List<Factory> Factories { get; set; }
        public List<Car> Cars { get; set; }
        public List<PositionNode> PositionNodes { get; set; }
        public Queue<int> ResourceQueue { get; set; }
        public Queue<int> ProductQueue { get; set; }

        // Dynamic values
        public int resourceIndex = 0;

        // Setup function
        public void Setup()
        {
            PositionNodes = new List<PositionNode>();
            foreach (PositionNode.NodeName name in Enum.GetValues(typeof(PositionNode.NodeName)))
            {
                PositionNode node = new PositionNode(name, new Point2D(0,0), false);
                PositionNodes.Add(node);
            }

            Factories = new List<Factory>
            {
                new Factory(1, PositionNodes[(int)PositionNode.NodeName.FirstInput], PositionNodes[(int)PositionNode.NodeName.FirstOutput], Factory1ProcessTime, false, Factory1InputQueueLength, Factory1OutputQueueLength),
                new Factory(2, PositionNodes[(int)PositionNode.NodeName.SecondInput], PositionNodes[(int)PositionNode.NodeName.SecondOutput], Factory2ProcessTime, false, Factory2InputQueueLength, Factory2OutputQueueLength),
            };

            Cars = new List<Car>
            {
                new Car(1, Car.MovementState.Idle, new Point2D(0,0), 0),
                new Car(2, Car.MovementState.Idle, new Point2D(0,0),0),
                new Car(3, Car.MovementState.Idle, new Point2D(0,0),0)
            };

            ResourceQueue = new Queue<int>(ResourceQueueLength);
            ProductQueue = new Queue<int>(ProductQueueLength);

            Debug.WriteLine("Setup complete");
        }

        

        public void Start()
        {
            Task.Run(() => MyTimer.RunTimer()); // Start the timer in a new task

            while (true)
            {
                // Spawn in resource
                if (MyTimer.elapsedSeconds != MyTimer.prevSecondsSpawn && MyTimer.elapsedSeconds % ResourceInterval == 0 && ResourceQueue.Count < ResourceQueueLength)
                {
                    Debug.WriteLine("Resource added");
                    MyTimer.prevSecondsSpawn = MyTimer.elapsedSeconds;
                    ResourceQueue.Enqueue(++resourceIndex);
                }

                // Take away product
                if (MyTimer.elapsedSeconds != MyTimer.prevSecondsTake && MyTimer.elapsedSeconds % TakeAwayTime == 0 && ProductQueue.Count != 0)
                {
                    Debug.WriteLine("Product taken away");
                    MyTimer.prevSecondsTake = MyTimer.elapsedSeconds;
                    ProductQueue.Dequeue();
                }

                // Check for idle cars to move resources and products
                foreach (Car car in Cars.Where(c => c.State == Car.MovementState.Idle))
                {
                    // Move resource to first factory
                    if (ResourceQueue.Count != 0 && Factories[0].InputQueue.Count < Factories[0].InputQueueLength)
                    {
                        Debug.WriteLine("Car {0} moving resource", car.Id);
                        car.delivering = ResourceQueue.Dequeue();
                        Task.Run(() => car.StartMoving(PositionNodes[(int)PositionNode.NodeName.FirstInput].Pos));
                        break;
                    }

                    // Move half-product to second factory
                    if (Factories[0].OutputQueue.Count != 0 && Factories[1].InputQueue.Count < Factories[1].InputQueueLength)
                    { 
                        Debug.WriteLine("Car {0} moving to second factory", car.Id);
                        car.delivering = Factories[0].OutputQueue.Dequeue();
                        Task.Run(() => car.StartMoving(PositionNodes[(int)PositionNode.NodeName.SecondInput].Pos));
                        break;
                    }

                    // Move final product to output
                    if (Factories[1].OutputQueue.Count != 0)
                    {
                        Debug.WriteLine("Car {0} is moving final product", car.Id);
                        car.delivering = Factories[1].OutputQueue.Dequeue();
                        Task.Run(() => car.StartMoving(PositionNodes[(int)PositionNode.NodeName.EndPoint].Pos));
                        break;
                    }
                }

                // Look at factories, unload resource if arrived and process
                foreach (Factory factory in Factories)
                {
                    // Unload
                    foreach (Car car in Cars)
                    {
                        // Unload resource
                        if (car.delivering != 0 && car.State == Car.MovementState.Waiting && car.HasArrivedAt(factory.StartPoint.Pos) )
                        {
                            Debug.WriteLine("Car {0} arrived at factory",car.Id);
                            factory.InputQueue.Enqueue(car.delivering);
                            car.delivering = 0;
                            car.State = Car.MovementState.Idle;
                        }

                        // Unload final product
                        if (car.delivering != 0 && car.State == Car.MovementState.Waiting && car.HasArrivedAt(PositionNodes[(int)PositionNode.NodeName.EndPoint].Pos) && ProductQueue.Count < ProductQueueLength)
                        {
                            Debug.WriteLine("Car {0} arrived at endpoint", car.Id);
                            ProductQueue.Enqueue(car.delivering);
                            car.delivering = 0;
                            car.State = Car.MovementState.Idle;
                            Debug.WriteLine("Product added to queue, amount now: {0}", ProductQueue.Count);  
                        }
                    }

                    // Process
                    if (factory.InputQueue.Count != 0 && factory.IsWorking == false && factory.OutputQueue.Count < factory.OutputQueueLength)
                    {
                        int resource = factory.InputQueue.Dequeue();
                        Task.Run(() => factory.Process(resource));
                    }
                }

              



            }
        }
    }
}
