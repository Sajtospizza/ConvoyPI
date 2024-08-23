using System.Diagnostics;

namespace coordsdrawer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        Graphics g;
        DataReceiver dataReceiver;

        private void Form1_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            g = Graphics.FromImage(pictureBox1.Image);
            g.Clear(Color.White);
            timer1.Interval = 10;
            string ip = "172.22.0.192";
            int port = 6944;
            dataReceiver = new DataReceiver(ip, port);
            dataReceiver.StartListening();
            Task.Run(() => dataReceiver.ReceiveData());
            timer1.Start();
        }



        private void timer1_Tick(object sender, EventArgs e)
        {
             Dictionary<string, List<double>> coordinates = dataReceiver.outcoordinates;
             
            if (coordinates != null)
            {
                g.Clear(Color.White);
                foreach (KeyValuePair<string, List<double>> entry in coordinates)
                {
                    List<double> coords = entry.Value;
                    g.FillEllipse(Brushes.Black, (float)coords[0] - 5 , (float)coords[1] - 5, 10, 10);
                }
                pictureBox1.Refresh();
            }
            
        }
    }
}
