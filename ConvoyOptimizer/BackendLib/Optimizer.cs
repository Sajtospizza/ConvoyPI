using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Diagnostics;
using static OptimizerFrontend.BackendLib.Car;
using static System.Windows.Forms.AxHost;

namespace OptimizerFrontend.BackendLib
{
    internal class Optimizer(int resourceInterval, int resourceQueueLength,int takeAwayTime, int productQueueLength,int factory1ProcessTime, int factory2ProcessTime, int fact1in, int fact1out, int fact2in, int fact2out, int numberofcars, Dictionary<string, List<double>> baseCars)
    {
        // Base values for the simulation
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
        public int numberOfCars { get; set; } = numberofcars;
        public Dictionary<string, List<double>> starterCars { get; set; } = baseCars;

        // Lists and queues
        public List<Factory> Factories { get; set; }
        public List<Car> Cars { get; set; }
        public Queue<int> ResourceQueue { get; set; }
        public Queue<int> ProductQueue { get; set; }
        public Dictionary<string, Point2D> PositionNodes { get; set; }

        // Dynamic values
        public int resourceIndex = 0;
        public int factory1Percent = 0;

        // Map information
        public int width { get; set; }
        public int height { get; set; }

        // Setup function
        public void Setup()
        {
            // Setting up the nodes

            PositionNodes = new Dictionary<string, Point2D>
            {
                {"SpawnPoint", new Point2D(height / 4, height / 2)},
                {"FirstInput", new Point2D((int)(height / 4 - height / 4 * Math.Sin(Math.PI/6)),(int)(height / 4 + height/4 * Math.Cos(Math.PI / 6)))},
                {"FirstOutput", new Point2D((int)(height / 4 + height / 4 * Math.Sin(Math.PI/6)),(int)(height / 4 - height/4 * Math.Cos(Math.PI / 6)))},
                {"SecondInput", new Point2D((int)(width/2 + height / 4 - height / 4 * Math.Sin(Math.PI/6)),(int)(height / 4 + height/4 * Math.Cos(Math.PI / 6)))},
                {"SecondOutput", new Point2D((int)(width/2 + height / 4 + height / 4 * Math.Sin(Math.PI/6)),(int)(height / 4 - height/4 * Math.Cos(Math.PI / 6)))},
                {"EndPoint", new Point2D(height / 4 + width / 2, 0 )},
                {"Park1", new Point2D(width/2,height / 2 )},
                {"Park2", new Point2D(width / 2,0 )}
            };

            // Debug print:
            foreach (KeyValuePair<string, Point2D> kvp in PositionNodes)
            {
                Debug.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
            }

            // Setup factories
            Factories = new List<Factory>
            {
                new Factory(1, PositionNodes["FirstInput"], PositionNodes["FirstOutput"], Factory1ProcessTime, false, Factory1InputQueueLength, Factory1OutputQueueLength),
                new Factory(2, PositionNodes["SecondInput"], PositionNodes["SecondOutput"], Factory2ProcessTime, false, Factory2InputQueueLength, Factory2OutputQueueLength),
            };

            // Setup cars
            Cars = new List<Car>(numberOfCars);
            foreach (KeyValuePair<string,List<double>> kvp in starterCars)
            {
                Cars.Add(new Car(int.Parse(kvp.Key), MovementState.Idle, new Point2D((int)kvp.Value[0], (int)kvp.Value[1]), 0));
            }
            while (Cars.Count > numberOfCars)
            {
                Cars.RemoveAt(Cars.Count - 1);
            }

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

                    // Move final product to output
                    if (Factories[1].OutputQueue.Count != 0 && ProductQueue.Count < ProductQueueLength && car.Pos == Factories[1].EndPoint)
                    {
                        car.delivering = Factories[1].OutputQueue.Dequeue();
                        car.State = MovementState.Moving;
                        Task.Run(() => car.StartMoving(PositionNodes["EndPoint"]));
                        continue;
                    }


                    // Move half-product to second factory
                    if (Factories[0].OutputQueue.Count != 0 && Factories[1].InputQueue.Count < Factories[1].InputQueueLength && car.Pos == Factories[0].EndPoint)
                    {
                        car.delivering = Factories[0].OutputQueue.Dequeue();
                        car.State = MovementState.Moving;
                        Task.Run(() => car.StartMoving(PositionNodes["SecondInput"]));
                        continue;
                    }


                    // Move to spawn point
                    if (ResourceQueue.Count != 0 && Factories[0].InputQueue.Count < Factories[0].InputQueueLength)
                    {
                        car.State = MovementState.Moving;
                        Task.Run(() => car.StartMoving(PositionNodes["SpawnPoint"])); // Move to spawn point (if not already there)
                        continue;
                    }

                }

                // Loop through waiting cars
                foreach (Car car in Cars.Where(c => c.State == Car.MovementState.Waiting))
                {
                    // Load and move to first factory
                    if (car.delivering == 0 && car.Pos == PositionNodes["SpawnPoint"])
                    {
                        car.delivering = ResourceQueue.Dequeue();
                        car.State = MovementState.Moving;
                        Task.Run(() => car.StartMoving(PositionNodes["FirstInput"]));
                        continue;
                    }

                }

                // Look at factories, unload resource if arrived and process
                foreach (Factory factory in Factories)
                {
                    // Unload
                    foreach (Car car in Cars)
                    {
                        // Unload resource
                        if (car.delivering != 0 && car.State == Car.MovementState.Waiting && car.Pos == factory.StartPoint && factory.InputQueue.Count < factory.InputQueueLength)
                        {
                            factory.InputQueue.Enqueue(car.delivering);
                            car.delivering = 0;
                            car.State = Car.MovementState.Idle;
                        }

                        // Unload final product
                        if (car.delivering != 0 && car.State == Car.MovementState.Waiting && car.Pos == factory.EndPoint && ProductQueue.Count < ProductQueueLength)
                        {
                            ProductQueue.Enqueue(car.delivering);
                            car.delivering = 0;
                            car.State = Car.MovementState.Idle;
                            Debug.WriteLine("Product added to queue, amount now: {0}", ProductQueue.Count);  
                        }

                        
                    }

                    // Process
                    if (factory.InputQueue.Count != 0 && factory.IsWorking == false && factory.OutputQueue.Count < factory.OutputQueueLength)
                    {
                        factory.IsWorking = true;             
                        int resource = factory.InputQueue.Dequeue();
                        Task.Run(() => factory.Process(resource));    
                    }

                }
            }
        }
    }
}
