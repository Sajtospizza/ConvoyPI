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
            State = MovementState.Moving;
            // Placeholder for moving logic
            Thread.Sleep(3000);
            // TODO: Implement moving logic
            Pos = newPos;
            Debug.WriteLine($"Car {Id} has arrived at {Pos}");
            State = MovementState.Waiting;
        }

        public bool HasArrivedAt(Point2D pos)
        {
            return pos == Pos;
        }






    }
}
