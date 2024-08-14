using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sheesh
{
    public class Optimizer
    {
        private int numCars = 3;
        private List<Car> cars;
        private int R_interval;
        private int F1_time;
        private int F2_time;
        private int incomingQueueLength;
        private int F1_inputQueueLength;
        private int F1_outputQueueLength;
        private int F2_inputQueueLength;
        private int F2_outputQueueLength;
        private int finalProductGoal;

        private Queue<int> incomingQueue;
        private Queue<int> F1_inputQueue;
        private Queue<int> F1_outputQueue;
        private Queue<int> F2_inputQueue;
        private Queue<int> F2_outputQueue;

        public Optimizer(int rInterval, int f1Time, int f2Time, int incomingQueueLength, int f1InputQueueLength, int f1OutputQueueLength, int f2InputQueueLength, int f2OutputQueueLength, int finalProductGoal)
        {
            R_interval = rInterval;
            F1_time = f1Time;
            F2_time = f2Time;
            this.incomingQueueLength = incomingQueueLength;
            this.F1_inputQueueLength = f1InputQueueLength;
            this.F1_outputQueueLength = f1OutputQueueLength;
            this.F2_inputQueueLength = f2InputQueueLength;
            this.F2_outputQueueLength = f2OutputQueueLength;
            this.finalProductGoal = finalProductGoal;

            cars = new List<Car>(numCars);
            for (int i = 0; i < numCars; i++)
            {
                cars.Add(new Car(i));
            }

            incomingQueue = new Queue<int>(incomingQueueLength);
            F1_inputQueue = new Queue<int>(F1_inputQueueLength);
            F1_outputQueue = new Queue<int>(F1_outputQueueLength);
            F2_inputQueue = new Queue<int>(F2_inputQueueLength);
            F2_outputQueue = new Queue<int>(F2_outputQueueLength);
        }

        public void RunOptimization()
        {
            int currentTime = 1;
            int resourceCount = 0;
            int finalProductCount = 0;

            while (finalProductCount < finalProductGoal)
            {
                Console.WriteLine($"Time: {currentTime}");

                // Incoming resources are added to the incoming queue at specified intervals
                if (currentTime % R_interval == 0)
                {
                    if (incomingQueue.Count < incomingQueueLength)
                    {
                        incomingQueue.Enqueue(++resourceCount);
                        Console.WriteLine("New resource added to incoming queue");
                         
                    }
                }

                // Move resources through the system only if cars are available
                foreach (var car in cars)
                {
                    if (car.IsIdle())
                    {
                        if (car.Id == 0 && incomingQueue.Count > 0 && F1_inputQueue.Count < F1_inputQueueLength)
                        {
                            car.TransportToFactory1(incomingQueue.Dequeue(), currentTime, F1_time);
                            Console.WriteLine("Transporting resource to Factory 1");
                            
                        }
                        else if (car.Id == 1 && F1_outputQueue.Count > 0 && F2_inputQueue.Count < F2_inputQueueLength)
                        {
                            car.TransportToFactory2(F1_outputQueue.Dequeue(), currentTime, F2_time);
                            Console.WriteLine("Transporting resource to Factory 2");
                        }
                        else if (car.Id == 2 && F2_outputQueue.Count > 0)
                        {
                            car.DeliverFinalProduct(F2_outputQueue.Dequeue(), currentTime);
                            Console.WriteLine("Transporting resource to Delivery");
                            finalProductCount++;
                        }
                    }
                }

                // Update the state of each car and the queues
                foreach (var car in cars)
                {
                    car.Update(currentTime);

                    // Handle car completion for Factory 1
                    if (car.CompletedTask == "Factory1" && F1_outputQueue.Count < F1_outputQueueLength)
                    {
                        F1_outputQueue.Enqueue(car.LastResource);
                        car.CompletedTask = null;
                        Console.WriteLine("Resource " + car.LastResource + " completed Factory 1");
                        
                    }
                    // Handle car completion for Factory 2
                    else if (car.CompletedTask == "Factory2" && F2_outputQueue.Count < F2_outputQueueLength)
                    {
                        F2_outputQueue.Enqueue(car.LastResource);
                        car.CompletedTask = null;
                        Console.WriteLine("Resource " + car.LastResource + " completed Factory 2");
                       
                    }
                }

                // Advance time by one unit
                currentTime++;
            }

            Console.WriteLine($"Process completed. {finalProductGoal} final products were delivered in {currentTime} time units.");
        }
    }
}
