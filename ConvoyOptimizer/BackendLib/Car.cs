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
        public Postion Position { get; set; }
        public enum MovementState
        {
            Parking = 0,
            Moving = 1,
            Waiting = 2,
            Idle = 3
        }
        public enum Postion
        {
            SpawnPickup = 1,
            FirstPutdown = 2,
            FirstPickup = 3,
            SecondPutdown = 4,
            SecondPickup = 5,
            EndPutdown = 6,
            Park1 = 7,
            Park2 = 8
        }

        public Car(int id, MovementState state, Postion position)
        {
            Id = id;
            State = state;
            Position = position;
        }

    }
}
