using OptimizerFrontend.BackendLib;
using OptimizerFrontend.CommsLib;
using OptimizerFrontend.DrawingLib;
using System.Diagnostics;
using System.Reflection;

namespace ConvoyOptimizer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // Variables
        Optimizer engine;
        PositionListener listener;
        Graphics drawingboard;
        Bitmap basemap;
        Bitmap bm;

        private void Form1_Load(object sender, EventArgs e)
        {
            // Set the title
            Text = "Convoy Optimizer";

            // Input labels
            label1.Text = "Resource interval in seconds";
            label2.Text = "Factory 1 process time in seconds";
            label3.Text = "Factory 2 process time in seconds";
            label4.Text = "Resource queue length";
            label5.Text = "Factory 1 input queue length";
            label6.Text = "Factory 2 input queue length";
            label7.Text = "Factory 1 output queue length";
            label8.Text = "Factory 2 output queue length";
            label9.Text = "Product takeaway time";
            label10.Text = "Product queue length";
            label11.Text = "Number of cars";

            // Default values
            textBox1.Text = "5";
            textBox2.Text = "5";
            textBox3.Text = "7";
            textBox4.Text = "3";
            textBox5.Text = "3";
            textBox6.Text = "3";
            textBox7.Text = "3";
            textBox8.Text = "3";
            textBox9.Text = "5";
            textBox10.Text = "3";
            textBox11.Text = "3";

            // Capacity labels
            label12.Text = "0";
            label13.Text = "0";
            label14.Text = "0";
            label15.Text = "0";
            label16.Text = "0";
            label17.Text = "0";

            // Button labels
            button1.Text = "Calculate";

            // Timer setup
            timer1.Interval = 100;

            // Draw the base map
            DrawBase drawer = new DrawBase(pictureBox1.Width, pictureBox1.Height);
            basemap = drawer.DrawMap(pictureBox1);
            pictureBox1.Image = basemap;

            // Create a new PositionListener
            string ip = "172.22.0.6";
            int port = 6944;
            listener = new PositionListener(ip, port);

            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Get the input values
            int resourceInterval = int.Parse(textBox1.Text);
            int factory1ProcessTime = int.Parse(textBox2.Text);
            int factory2ProcessTime = int.Parse(textBox3.Text);
            int factory1InputQueueLength = int.Parse(textBox5.Text);
            int factory2InputQueueLength = int.Parse(textBox6.Text);
            int factory1OutputQueueLength = int.Parse(textBox7.Text);
            int factory2OutputQueueLength = int.Parse(textBox8.Text);
            int resourceQueueLength = int.Parse(textBox4.Text);
            int productQueueLength = int.Parse(textBox10.Text);
            int productTakeAwayTime = int.Parse(textBox9.Text);
            int numberOfCars = int.Parse(textBox11.Text);

            // Set the progress bar maximum values
            progressBar1.Maximum = factory1ProcessTime * 1000;
            progressBar2.Maximum = factory2ProcessTime * 1000;

            // Setting up the engine
            engine = new Optimizer(resourceInterval, resourceQueueLength, productTakeAwayTime, productQueueLength, factory1ProcessTime, factory2ProcessTime, factory1InputQueueLength, factory1OutputQueueLength, factory2InputQueueLength, factory2OutputQueueLength, numberOfCars);
            // Cheap ass solution, please make this look better
            engine.width = pictureBox1.Width;
            engine.height = pictureBox1.Height;
            engine.Setup();

            // Start the listener
            //listener.StartListening();
            //Task.Run(() => listener.ReceiveData());

            // Start timer
            Debug.WriteLine("Starting timer");
            timer1.Start();

            // Start the engine
            Debug.WriteLine("Starting engine");
            Task.Run(() => engine.Start());
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            // Update the progress bars
            progressBar1.Value = (int)( ((double)engine.Factories[0].currentPercent) / 100 * engine.Factory1ProcessTime * 1000);
            progressBar2.Value = (int)(((double)engine.Factories[1].currentPercent) / 100 * engine.Factory2ProcessTime * 1000);

            // Update the capacity labels
            label12.Text = engine.ResourceQueue.Count.ToString();
            label13.Text = engine.Factories[0].InputQueue.Count.ToString();
            label14.Text = engine.Factories[0].OutputQueue.Count.ToString();
            label15.Text = engine.Factories[1].InputQueue.Count.ToString();
            label16.Text = engine.Factories[1].OutputQueue.Count.ToString();
            label17.Text = engine.ProductQueue.Count.ToString();

            // Draw the cars
            if (listener.outcoordinates != null)
            {
                Bitmap currentmap = (Bitmap)basemap.Clone();
                Graphics currentmapg = Graphics.FromImage(currentmap);
                foreach (KeyValuePair<string, List<double>> entry in listener.outcoordinates)
                {
                    List<double> coords = entry.Value;
                    Debug.WriteLine(currentmap.Width + " " + currentmap.Height);
                    currentmapg.FillEllipse(Brushes.Black, (float)coords[0] - 5 + currentmap.Width / 4, (float)coords[1] - 5 + currentmap.Height / 4, 10 , 10);
                }
                pictureBox1.Image = currentmap;
            }
            

        }
    }
}
