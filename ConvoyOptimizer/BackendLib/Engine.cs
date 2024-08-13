using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace OptimizerFrontend.BackendLib
{
    internal class Engine
    {
        public int ResourceInterval { get; set; }
        public int Factory1ProcessTime { get; set; }
        public int Factory2ProcessTime { get; set; }

        public Engine(int resourceInterval, int factory1ProcessTime, int factory2ProcessTime)
        {
            ResourceInterval = resourceInterval;
            Factory1ProcessTime = factory1ProcessTime * 1000;
            Factory2ProcessTime = factory2ProcessTime * 1000;
        }

        public void Start()
        {
            // Create the factories in a list
            List<Factory> factories = new List<Factory>();
            Factory factory1 = new Factory(1, 2,3 , Factory1ProcessTime, false);
            Factory factory2 = new Factory(2, 4, 5, Factory2ProcessTime, false);
            factories.Add(factory1);
            factories.Add(factory2);

            // Create the cars in a list
            List<Car> cars = new List<Car>();
            Car car1 = new Car(1,  Car.MovementState.Parking, Car.Postion.SpawnPickup);
            Car car2 = new Car(2, Car.MovementState.Parking, Car.Postion.Park1);
            Car car3 = new Car(3,  Car.MovementState.Parking, Car.Postion.Park2);
            cars.Add(car1);
            cars.Add(car2);
            cars.Add(car3);

            foreach (Car car in cars)
            {
                if (car.State == Car.MovementState.)
                {
                    car.State = Car.MovementState.Moving;
                    car.Position = Car.Postion.FirstPutdown;
                    Debug.WriteLine($"Car {car.Id} is moving to {car.Position}");
                }
            }
            

            

            

        }
    }
}
