using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace mouse_event_paint
{
    public partial class Form1 : Form
    {
        bool drawing = false;
        Point last;
        Point start;
        Point current;

        Color penColor = Color.Black;
        int penWidth = 1;
        string mode = "pen";   // "pen", "rect", "line", "circle"

        Bitmap canvas;

        public Form1()
        {
            InitializeComponent();

            DoubleBuffered = true;
            KeyPreview = true;
            canvas = new Bitmap(Math.Max(1, ClientSize.Width), Math.Max(1, ClientSize.Height));
            using (Graphics g = Graphics.FromImage(canvas))
                g.Clear(Color.White);

            this.Paint += Form1_Paint;
            this.Resize += Form1_Resize;
            this.MouseDown += Form1_MouseDown;
            this.MouseMove += Form1_MouseMove;
            this.MouseUp += Form1_MouseUp;
            this.KeyDown += Form1_KeyDown;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        // P = харандаа, R = тэгш өнцөг, L = шулуун, C = дугуй
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.P) mode = "pen";
            else if (e.KeyCode == Keys.R) mode = "rect";
            else if (e.KeyCode == Keys.L) mode = "line";
            else if (e.KeyCode == Keys.C) mode = "circle";
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImageUnscaled(canvas, 0, 0);

            if (!drawing) return;

            using (Pen pen = new Pen(penColor, penWidth))
            {
                if (mode == "rect")
                    e.Graphics.DrawRectangle(pen, GetRect());
                else if (mode == "line")
                    e.Graphics.DrawLine(pen, start, current);
                else if (mode == "circle")
                    e.Graphics.DrawEllipse(pen, GetRect());
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            int w = Math.Max(ClientSize.Width, canvas.Width);
            int h = Math.Max(ClientSize.Height, canvas.Height);
            if (w == canvas.Width && h == canvas.Height) return;

            Bitmap nb = new Bitmap(w, h);
            using (Graphics g = Graphics.FromImage(nb))
            {
                g.Clear(Color.White);
                g.DrawImageUnscaled(canvas, 0, 0);
            }
            canvas.Dispose();
            canvas = nb;
            Invalidate();
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            drawing = true;
            last = e.Location;
            start = e.Location;
            current = e.Location;
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!drawing) return;

            current = e.Location;

            if (mode == "pen")
            {
                using (Graphics g = Graphics.FromImage(canvas))
                using (Pen pen = new Pen(penColor, penWidth) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                {
                    g.DrawLine(pen, last, current);
                }
                last = current;
            }

            Invalidate();
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (!drawing) return;

            drawing = false;
            current = e.Location;

            if (mode != "pen")
            {
                using (Graphics g = Graphics.FromImage(canvas))
                using (Pen pen = new Pen(penColor, penWidth))
                {
                    if (mode == "rect")
                        g.DrawRectangle(pen, GetRect());
                    else if (mode == "line")
                        g.DrawLine(pen, start, current);
                    else if (mode == "circle")
                        g.DrawEllipse(pen, GetRect());
                }
            }

            Invalidate();
        }

        // Хулганыг аль чиглэлд чирсэн ч зөв хэмжээтэй тэгш өнцөг гаргана
        private Rectangle GetRect()
        {
            return new Rectangle(
                Math.Min(start.X, current.X),
                Math.Min(start.Y, current.Y),
                Math.Abs(current.X - start.X),
                Math.Abs(current.Y - start.Y));
        }

        // Tools → Харандаа
        private void customizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mode = "pen";
        }

        // Tools → Тэгш өнцөг
        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mode = "rect";
        }

        // Tools → Шулуун
        private void lineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mode = "line";
        }

        // Tools → Дугуй
        private void circleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mode = "circle";
        }

        // Tools → Өнгө
        private void өнгөToolStripMenuItem_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = penColor;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                penColor = colorDialog1.Color;
            }
        }

        // Өргөн сонгох ComboBox
        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (int.TryParse(toolStripComboBox1.SelectedItem?.ToString(), out int w))
                penWidth = w;
        }
    }
}