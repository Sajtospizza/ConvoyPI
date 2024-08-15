using OptimizerFrontend.BackendLib;
using System;
using System.Collections.Generic;
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

        public void DrawMap(PictureBox picturebox)
        {
            // Draw rectangles
            g.DrawRectangle(Pens.Black, 150, 75, 100, 150);
            g.DrawRectangle(Pens.Black, 450, 75, 100, 150);

            // Draw circles
            g.DrawEllipse(Pens.Black, 175, 13, 50, 50);
            g.DrawEllipse(Pens.Black, 475, 13, 50, 50);

            g.DrawEllipse(Pens.Black, 475, 237, 50, 50);
            g.DrawEllipse(Pens.Black, 175, 237, 50, 50);

            g.DrawEllipse(Pens.Black, 325, 237, 50, 50);
            g.DrawEllipse(Pens.Black, 325, 13, 50, 50);

            // Set up font for drawing numbers
            Font font = new Font("Arial", 16);
            Brush brush = Brushes.Black;

            // Reverse the flip temporarily
            g.ScaleTransform(1, -1);

            // Draw numbers inside the circles with adjusted y-coordinates
            g.DrawString("1", font, brush, 190, -28);   // Inside first circle
            g.DrawString("2", font, brush, 490, -28);   // Inside second circle

            g.DrawString("3", font, brush, 490, -252);  // Inside third circle
            g.DrawString("4", font, brush, 190, -252);  // Inside fourth circle

            g.DrawString("5", font, brush, 340, -252);  // Inside fifth circle
            g.DrawString("6", font, brush, 340, -28);   // Inside sixth circle

            // Draw numbers next to the rectangles with adjusted y-coordinates
            g.DrawString("7", font, brush, 260, -140);  // Next to first rectangle
            g.DrawString("8", font, brush, 560, -140);  // Next to second rectangle

            // Reverse the flip back to the original state
            g.ScaleTransform(1, -1);

            // Assign the bitmap to the PictureBox
            picturebox.Image = bm;
            picturebox.BackColor = Color.White;
        }




    }
}
