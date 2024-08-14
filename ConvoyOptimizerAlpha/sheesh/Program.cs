using System;
using System.Collections.Generic;



namespace sheesh
{
    class Program
    {
        static void Main()
        {
            int rInterval = 10; // Interval between incoming resources
            int f1Time = 5; // Process time for factory 1
            int f2Time = 7; // Process time for factory 2
            int incomingQueueLength = 5; // Incoming resource queue length
            int f1InputQueueLength = 3; // Factory 1 input queue length
            int f1OutputQueueLength = 3; // Factory 1 output queue length
            int f2InputQueueLength = 3; // Factory 2 input queue length
            int f2OutputQueueLength = 3; // Factory 2 output queue length
            int finalProductGoal = 6; // Number of final products needed

            Optimizer optimizer = new Optimizer(rInterval, f1Time, f2Time, incomingQueueLength, f1InputQueueLength, f1OutputQueueLength, f2InputQueueLength, f2OutputQueueLength, finalProductGoal);
            optimizer.RunOptimization();
        }
    }
}