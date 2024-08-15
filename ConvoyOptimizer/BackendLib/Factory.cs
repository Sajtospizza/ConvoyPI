using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimizerFrontend.BackendLib
{
    internal class Factory
    {
        public int Id { get; set; }

        public PositionNode StartPoint { get; set; }
        public PositionNode EndPoint { get; set; }

        public int ProcessTime { get; set; } // in seconds
        public bool IsWorking { get; set; }

        public int InputQueueLength { get; set; }
        public int OutputQueueLength { get; set; }

        public Queue<int> InputQueue { get; set; }
        public Queue<int> OutputQueue { get; set; }

        public Factory(int id, PositionNode input, PositionNode output, int processTime, bool isWorking, int InputQueueLength, int OutputQueueLength)
        {
            Id = id;
            StartPoint = input;
            EndPoint = output;
            ProcessTime = processTime;
            IsWorking = isWorking;
            this.InputQueueLength = InputQueueLength;
            this.OutputQueueLength = OutputQueueLength;
            InputQueue = new Queue<int>(InputQueueLength);
            OutputQueue = new Queue<int>(OutputQueueLength);
        }

        public void Process(int resource)
        {
            Debug.WriteLine("Factory {0} is processing resource {1}", Id, resource);
            // Placeholder for processing logic
            System.Threading.Thread.Sleep(ProcessTime * 1000);
            OutputQueue.Enqueue(resource);
            IsWorking = false;
            Debug.WriteLine("Factory {0} has processed resource {1}", Id, resource);
        }


    }
}

