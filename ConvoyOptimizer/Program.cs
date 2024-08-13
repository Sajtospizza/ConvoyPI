using OptimizerFrontend.BackendLib;
using System;
using System.Windows.Forms;

namespace ConvoyOptimizer
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            // Enable visual styles and set text rendering
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Start the Windows Forms application
            Application.Run(new Form1());

            
        }
    }
}
