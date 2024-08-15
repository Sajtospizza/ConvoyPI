using OptimizerFrontend.BackendLib;
using System.Diagnostics;

namespace ConvoyOptimizer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
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


            button1.Text = "Calculate";
        }

        private void button1_Click(object sender, EventArgs e)
        {
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

            Optimizer engine = new Optimizer(resourceInterval, resourceQueueLength,productTakeAwayTime, productQueueLength,factory1ProcessTime, factory2ProcessTime,factory1InputQueueLength,factory1OutputQueueLength,factory2InputQueueLength,factory2OutputQueueLength);
            engine.Setup();
            engine.Start();
        }


    }
}
