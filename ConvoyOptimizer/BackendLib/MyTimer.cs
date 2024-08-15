using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace OptimizerFrontend.BackendLib
{
    internal class MyTimer
    {
        public static System.Timers.Timer timer;
        public static double elapsedSeconds = 0;
        public static double prevSecondsSpawn = 0;
        public static double prevSecondsTake = 0;
        public static void RunTimer()
        {
            // Create and configure the timer
            timer = new System.Timers.Timer(1000); // 1-second interval
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = true; // Repeat the timer event
            timer.Start();
        }

        public static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            elapsedSeconds += 1;
        }
    }
}
