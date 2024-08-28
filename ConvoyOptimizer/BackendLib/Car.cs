using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimizerFrontend.BackendLib
{
    internal class Car
    {
        public int Id { get; set; }
        public MovementState State { get; set; }
        public Point2D Pos { get; set; }
        public int delivering { get; set; }

        public enum MovementState
        {
            Moving = 1,
            Waiting = 2,
            Idle = 3
        }

        public Car(int id, MovementState state, Point2D point, int Delivering)
        {
            Id = id;
            State = state;
            Pos = point;
            delivering = Delivering;
        }

        public void StartMoving(Point2D newPos)
        {
            Debug.WriteLine($"Car {Id} is moving to {newPos}");
            
            // TODO: Implement moving logic
            while (true) {
                if (HasArrivedAt(newPos)) {
                    Debug.WriteLine($"Car {Id} has arrived");
                    Pos = newPos;
                    State = MovementState.Waiting;
                    return;
                }
            }
        }

        public bool HasArrivedAt(Point2D pos)
        {
            // Check if the car is in a given area of position
            return Pos.DistanceTo(pos) < 25;
        }






    }
}
