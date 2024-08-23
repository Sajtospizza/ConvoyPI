using OptimizerFrontend.BackendLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimizerFrontend.DrawingLib
{
    internal class DrawBase
    {
        int width;
        int height;
        Bitmap bm;
        Graphics g;

        public DrawBase(int width, int height)
        {
            this.width = width;
            this.height = height;
            bm = new Bitmap(width, height);
            g = Graphics.FromImage(bm);
            g.TranslateTransform(0, height);
            g.ScaleTransform(1, -1);
        }

        // DO NOT TOUCH THIS FUNCTION OR I WILL KILL MYSELF
        public void DrawMap(PictureBox picturebox)
        {

            // Init drawing tools
            int factorysize = 60;
            int nodesize = 10;
            Pen pen = new Pen(Color.Black, 1);
            Brush brush = new SolidBrush(Color.Red);
            Brush brush2 = new SolidBrush(Color.Black);
            Font font = new Font("Arial", 8, FontStyle.Regular);
            Font font2 = new Font("Arial", 12, FontStyle.Regular);

            // Draw the map
            g.DrawArc(pen, width / 4 - height / 4, height / 2 - height / 4, height / 2,  height / 2, 90,180);
            g.DrawArc(pen, 3 * width / 4 - height / 4, height / 2 - height / 4, height / 2,  height / 2, 270,180);
            g.DrawLine(pen, width / 4, height / 2 + height/4, 3 * width / 4 , height / 2 + height/4);
            g.DrawLine(pen, width / 4, height / 2 - height/4, 3 * width / 4 , height / 2 - height/4);

            // Draw the factories and their nodes
            // Factory 1
            g.TranslateTransform(width / 4, height / 2);
            g.RotateTransform(-30);
            g.DrawRectangle(pen, -factorysize / 2, - factorysize , factorysize, 2 * factorysize);
            // Curve and lines 
            g.DrawArc(pen, -height / 4, -height / 4, height / 2, height / 2, 90, 240);
            g.TranslateTransform(0, height / 4);
            g.DrawLine(pen, (float)0.0, (float)0.0, (float)((width / 2) * Math.Sqrt(3) / 2), (float)0.0);
            g.RotateTransform(30);
            g.TranslateTransform(0, (float)(-height / 4 * Math.Sqrt(3)));
            g.RotateTransform(30);
            g.DrawLine(pen, (float)0.0, (float)0.0, (float)((width / 2) * Math.Sqrt(3) / 2), (float)0.0);
            g.RotateTransform(-30);
            g.TranslateTransform(0, (float)(height / 4 * Math.Sqrt(3) ));
            g.RotateTransform(-30);
            g.TranslateTransform(0, -height / 4);
            // Nodes
            g.FillEllipse(brush, -nodesize / 2, -height / 4 - nodesize / 2, nodesize, nodesize);
            g.FillEllipse(brush, -nodesize / 2, height / 4 - nodesize / 2, nodesize, nodesize);
            // Text
            g.ScaleTransform(1, -1);
            g.DrawString("2", font, brush2, -nodesize, -height / 4 - 2 * nodesize + nodesize);
            g.DrawString("1", font, brush2, -nodesize, height / 4 - 2 * nodesize - nodesize);
            g.DrawString("Factory 1", font2, brush2, -factorysize / 2, -factorysize - 20);
            g.ScaleTransform(1, -1);
            g.RotateTransform(30);
            // Factory 2
            g.TranslateTransform(width / 2, 0);
            g.RotateTransform(-30);
            g.DrawRectangle(pen, -factorysize / 2, -factorysize, factorysize, 2 * factorysize);
            // Curves
            g.DrawArc(pen, -height / 4, -height / 4, height / 2, height / 2, -90, 240);
            // Nodes
            g.FillEllipse(brush, -nodesize / 2, -height / 4 - nodesize / 2, nodesize, nodesize);
            g.FillEllipse(brush, -nodesize / 2, height / 4 - nodesize / 2, nodesize, nodesize);
            // Text
            g.ScaleTransform(1, -1);
            g.DrawString("4", font, brush2, -nodesize, -height / 4 - 2 * nodesize + 2 * nodesize);
            g.DrawString("3", font, brush2, -nodesize, height / 4 - 2 * nodesize - nodesize);
            g.DrawString("Factory 2", font2, brush2, -factorysize / 2, -factorysize - 20);
            g.ScaleTransform(1, -1);
            g.RotateTransform(30);
            g.TranslateTransform(-3 * width / 4, - height / 2);
            // Draw nodes
            g.FillEllipse(brush, width / 4 - nodesize / 2, height / 2 - height / 4 - nodesize / 2, nodesize, nodesize);
            g.FillEllipse(brush, 3 * width / 4 - nodesize / 2, height / 2 + height / 4 - nodesize / 2, nodesize, nodesize);
            g.ScaleTransform(1, -1);
            g.DrawString("0", font, brush2, width / 4, -(height / 2 - height / 4 - 2 *nodesize));
            g.DrawString("5", font, brush2, 3 * width / 4 - nodesize, -( height / 2 + height / 4 + 2 * nodesize));
            g.ScaleTransform(1, -1);


            // Assign the bitmap to the PictureBox
            picturebox.Image = bm;
            picturebox.BackColor = Color.White;
        }
    }
}
