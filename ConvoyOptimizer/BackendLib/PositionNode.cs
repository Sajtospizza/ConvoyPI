using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimizerFrontend.BackendLib
{
    internal class PositionNode
    {
        public NodeName Id { get; set; }
        public Point2D Pos { get; set; }
        public bool IsOccupied { get; set; }

        public enum NodeName
        {
            SpawnPoint = 0,
            FirstInput = 1,
            FirstOutput = 2,
            SecondInput = 3,
            SecondOutput = 4,
            EndPoint = 5,
            Park1 = 6,
            Park2 = 7
        }
        public PositionNode(NodeName id, Point2D pos, bool isOccupied)
        {
            Id = id;
            Pos = pos;
            IsOccupied = isOccupied;
        }
    }
}
