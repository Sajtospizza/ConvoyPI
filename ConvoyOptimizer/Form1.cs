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

            textBox1.Text = "10";
            textBox2.Text = "5";
            textBox3.Text = "7";

            button1.Text = "Calculate";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int resourceInterval = int.Parse(textBox1.Text);
            int factory1ProcessTime = int.Parse(textBox2.Text);
            int factory2ProcessTime = int.Parse(textBox3.Text);

            Optimizer engine = new Optimizer(resourceInterval, factory1ProcessTime, factory2ProcessTime,1,1,1,1);
            engine.Start();
        }


    }
}
