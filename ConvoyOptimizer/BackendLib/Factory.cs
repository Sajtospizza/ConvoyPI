using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using OptimizerFrontend.DrawingLib;

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

        int elapsedtime;
        public int currentPercent = 0;

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
            elapsedtime = 0;
            Debug.WriteLine("Factory {0} is processing resource {1}", Id, resource);
            System.Timers.Timer timer = new System.Timers.Timer(100);
            timer.Elapsed += (sender, e) => OnTimedEvent(sender, e, resource);
            // Placeholder for processing logic
            DrawProgressBar drawProgressBar = new DrawProgressBar(100, 100, ProcessTime);
            timer.Start();
            while (elapsedtime < ProcessTime * 1000)
            {
                currentPercent = (int)((double) elapsedtime / (ProcessTime * 1000) * 100);
            }
            Thread.Sleep(500);
            currentPercent = 0;
            timer.Stop();
            timer.Dispose();
            OutputQueue.Enqueue(resource);
            IsWorking = false;
            Debug.WriteLine("Factory {0} has processed resource {1}", Id, resource);
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e, int resource)
        {
            elapsedtime += 100;
        }


    }
}

