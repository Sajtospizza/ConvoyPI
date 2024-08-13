using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimizerFrontend.BackendLib
{
    internal class Factory
    {
        public int Id { get; set; }
        public int StartPoint { get; set; }
        public int EndPoint { get; set; }
        public int ProcessTime { get; set; } // in milliseconds
        public bool IsWorking { get; set; }

        public Factory(int id, int startPoint, int endPoint, int processTime, bool isWorking)
        {
            Id = id;
            StartPoint = startPoint;
            EndPoint = endPoint;
            ProcessTime = processTime;
            IsWorking = isWorking;
        }

        public void Processing()
        {
            IsWorking = true;
            Task.Run(async () =>
            {
                try
                {
                    // Wait for the processTime to elapse
                    await Task.Delay(ProcessTime);

                    IsWorking = false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            });
        }
    }
}
