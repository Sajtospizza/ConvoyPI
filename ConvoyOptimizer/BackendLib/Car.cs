using System;
using System.Collections.Generic;
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
            // TODO: Implement moving logic
        }

        public bool HasArrivedAt(Point2D pos)
        {
            return pos == Pos;
        }






    }
}
