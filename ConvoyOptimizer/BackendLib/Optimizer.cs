using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Diagnostics;

namespace OptimizerFrontend.BackendLib
{
    internal class Optimizer(int resourceInterval, int factory1ProcessTime, int factory2ProcessTime, int fact1in, int fact1out, int fact2in, int fact2out)
    {
        // Base values
        public int ResourceInterval { get; set; } = resourceInterval;
        public int Factory1ProcessTime { get; set; } = factory1ProcessTime;
        public int Factory2ProcessTime { get; set; } = factory2ProcessTime;
        public int Factory1InputQueueLength { get; set; } = fact1in;
        public int Factory1OutputQueueLength { get; set; } = fact1out;
        public int Factory2InputQueueLength { get; set; } = fact2in;
        public int Factory2OutputQueueLength { get; set; } = fact2out;

        // Lists
        public List<Factory> Factories { get; set; }
        public List<Car> Cars { get; set; }
        public List<PositionNode> PositionNodes { get; set; }

        public List<Car> IdleCars { get; set; }

        // Dynamic values
        public int hasResource = 0;
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

            foreach (Car car in Cars)
            {
                if (car.State == Car.MovementState.Idle)
                    IdleCars.Add(car);
            }
        }

        

        public void Start()
        {
            Task.Run(() => MyTimer.RunTimer()); // Start the timer in a new task

            while (true)
            {
                // Spawn in resource
                if (MyTimer.elapsedSeconds != MyTimer.prevSeconds && MyTimer.elapsedSeconds % ResourceInterval == 0)
                {
                    Debug.WriteLine("Resource added");
                    MyTimer.prevSeconds = MyTimer.elapsedSeconds;
                    hasResource = 1;
                }

                // Check for idle cars to pickup resource
                foreach (Car car in IdleCars)
                {
                    if (hasResource == 1)
                    {
                        car.delivering = ++resourceIndex;
                        car.StartMoving(PositionNodes[(int)PositionNode.NodeName.FirstInput].Pos);
                        hasResource = 0;
                    }
                }

                // Check for cars at factory entrances
                foreach (Factory factory in Factories)
                {
                    foreach (Car car in Cars)
                    {
                        if (car.delivering != 0 && car.State == Car.MovementState.Waiting && car.HasArrivedAt(factory.StartPoint.Pos) )
                        {
                            car.delivering = 0;
                            factory.InputQueue.Enqueue(resourceIndex);
                        }
                    }
                }



            }
        }
    }
}
