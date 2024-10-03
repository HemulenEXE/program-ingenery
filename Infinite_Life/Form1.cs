using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Globalization;

namespace Infinite_Life
{
    public partial class form1 : Form
    {
        private int currentGeneration = 0;
        private Graphics graphics;
        private int resolution;
        private bool[,] field;
        private int rows, cols;
        public form1()
        {
            InitializeComponent();
        }

        private void startgame()
        {
            if (timer1.Enabled)
                return;

            currentGeneration = 0;
            Text = $"Generation {currentGeneration}";

            nudResolution.Enabled = false;
            nudDensity.Enabled = false;

            resolution = (int)nudResolution.Value;
            rows = pictureBox1.Height / resolution;
            cols = pictureBox1.Width / resolution;
            field = new bool[cols ,rows];

            Random random = new Random();
            for (int x = 0; x < cols; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    field[x, y] = random.Next((int)nudDensity.Value) == 0;
                }
            }

            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            graphics = Graphics.FromImage(pictureBox1.Image);
            timer1.Start();
        }

        private void nextGeneration()
        {
            graphics.Clear(Color.Black);

            var newfield = new bool[cols, rows];

            for (int x = 0; x < cols; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    var neighbourscount = CountNextBours(x, y);
                    var haslife = field[x, y];

                    if (!haslife && neighbourscount == 3)
                        newfield[x, y] = true;
                    else if (haslife && (neighbourscount < 2 || neighbourscount > 3))
                        newfield[x, y] = false;
                    else
                        newfield[x, y] = field[x, y];

                    if (haslife)
                    {
                        graphics.FillRectangle(Brushes.Crimson, x * resolution, y * resolution, resolution, resolution);
                    }
                }
            }
            field = newfield;
            pictureBox1.Refresh();
            Text = $"Generation {++currentGeneration}";
        }

        private int CountNextBours(int x, int y)
        {
            int count = 0;

            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    var col = (x + i + cols) % cols;
                    var row = (y + j + rows) % rows;

                    var isSelfChecking = col == x && row == y;
                    var haslife = field[col, row];

                    if (haslife && !isSelfChecking)
                         count++;
                }
            }
            return count;
        }

        private void stopgame()
        {
            if (!timer1.Enabled)
                return;
            timer1.Stop();
            nudDensity.Enabled = true;
            nudResolution.Enabled = true;
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            nextGeneration();
        }

        private void bstart_Click(object sender, EventArgs e)
        {
            startgame();
        }

        

        private void bstop_Click(object sender, EventArgs e)
        {
            stopgame();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!timer1.Enabled)
                return;

            if (e.Button == MouseButtons.Left)
            {
                var x = e.Location.X / resolution;
                var y = e.Location.Y / resolution;
                var validationpassed = ValidateMousePosition(x, y);
                if (validationpassed)
                        field[x, y] = true;
            }

            if (e.Button == MouseButtons.Right)
            {
                var x = e.Location.X / resolution;
                var y = e.Location.Y / resolution;
                        var validationpassed = ValidateMousePosition(x, y);
                        if (validationpassed)
                            field[x, y] = false;
               
            }
        }

        private bool ValidateMousePosition (int x, int y)
        {
            return x >= 0 && y >= 0 && x < cols && y < rows;
        }
    }
}
