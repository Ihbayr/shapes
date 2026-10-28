using System.Drawing.Drawing2D;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace shapes
{
    // ==========================================
    // 1. СУУРЬ БОЛОН УДАМШСАН КЛАССУУД
    // ==========================================

    public abstract class Shape
    {
        public Color Color;
        public float Width;
        public Point Start;
        public Point End;

        public Shape(Color color, float width, Point start)
        {
            Color = color;
            Width = width;
            Start = start;
            End = start;
        }

        public abstract void Draw(Graphics g);
    }

    // 1. Шулуун
    public class Line : Shape
    {
        public Line(Color color, float width, Point start) : base(color, width, start) { }

        public override void Draw(Graphics g)
        {
            using (Pen pen = new Pen(Color, Width))
            {
                g.DrawLine(pen, Start, End);
            }
        }
    }

    // 2. Тэгш өнцөгт
    public class RectangleShape : Shape
    {
        public RectangleShape(Color color, float width, Point start) : base(color, width, start) { }

        public override void Draw(Graphics g)
        {
            using (Pen pen = new Pen(Color, Width))
            {
                int x = Math.Min(Start.X, End.X);
                int y = Math.Min(Start.Y, End.Y);
                int width = Math.Abs(Start.X - End.X);
                int height = Math.Abs(Start.Y - End.Y);

                g.DrawRectangle(pen, x, y, width, height);
            }
        }
    }

    // 3. Тойрог / Эллипс
    public class Circle : Shape
    {
        public Circle(Color color, float width, Point start) : base(color, width, start) { }

        public override void Draw(Graphics g)
        {
            using (Pen pen = new Pen(Color, Width))
            {
                int x = Math.Min(Start.X, End.X);
                int y = Math.Min(Start.Y, End.Y);
                int width = Math.Abs(Start.X - End.X);
                int height = Math.Abs(Start.Y - End.Y);

                g.DrawEllipse(pen, x, y, width, height);
            }
        }
    }

    // 4. Гурвалжин (Нэмэлт дүрс)
    public class Triangle : Shape
    {
        public Triangle(Color color, float width, Point start) : base(color, width, start) { }

        public override void Draw(Graphics g)
        {
            using (Pen pen = new Pen(Color, Width))
            {
                int x = Math.Min(Start.X, End.X);
                int y = Math.Min(Start.Y, End.Y);
                int width = Math.Abs(Start.X - End.X);
                int height = Math.Abs(Start.Y - End.Y);

                Point p1 = new Point(x + width / 2, y); // Оройн цэг
                Point p2 = new Point(x, y + height);    // Зүүн доод
                Point p3 = new Point(x + width, y + height); // Баруун доод

                g.DrawPolygon(pen, new Point[] { p1, p2, p3 });
            }
        }
    }


    // ==========================================
    // 2. ҮНДСЭН FORM КЛАСС
    // ==========================================
    public class Form1 : Form
    {
        private List<Shape> shapes = new List<Shape>();
        private Shape currentShape = null;
        private bool isDrawing = false;

        private Color currentColor = Color.Black;
        private float currentWidth = 2.0f;
        private string selectedTool = "Line"; // Сонгогдсон хэрэгсэл

        public Form1()
        {
            this.Width = 1000;
            this.Height = 700;
            this.Text = "OOP Shapes Project - Paint";
            this.DoubleBuffered = true;

            this.MouseDown += Form1_MouseDown;
            this.MouseMove += Form1_MouseMove;
            this.MouseUp += Form1_MouseUp;
            this.Paint += Form1_Paint;

            SetupUI();
        }

        private void SetupUI()
        {
            Panel topPanel = new Panel();
            topPanel.Dock = DockStyle.Top;
            topPanel.Height = 60;
            topPanel.BackColor = Color.WhiteSmoke;

            // Товчлууруудыг үүсгэх
            Button btnLine = new Button { Text = "Шулуун", Location = new Point(15, 15), Size = new Size(80, 30) };
            btnLine.Click += (s, e) => selectedTool = "Line";

            Button btnRect = new Button { Text = "Тэгш өнцөгт", Location = new Point(100, 15), Size = new Size(95, 30) };
            btnRect.Click += (s, e) => selectedTool = "Rectangle";

            Button btnCircle = new Button { Text = "Тойрог", Location = new Point(200, 15), Size = new Size(80, 30) };
            btnCircle.Click += (s, e) => selectedTool = "Circle";

            Button btnTriangle = new Button { Text = "Гурвалжин", Location = new Point(285, 15), Size = new Size(95, 30) };
            btnTriangle.Click += (s, e) => selectedTool = "Triangle";

            Button btnColor = new Button { Text = "Өнгө", Location = new Point(395, 15), Size = new Size(80, 30) };
            btnColor.Click += (s, e) =>
            {
                ColorDialog cd = new ColorDialog();
                if (cd.ShowDialog() == DialogResult.OK)
                    currentColor = cd.Color;
            };

            Button btnClear = new Button { Text = "Цэвэрлэх", Location = new Point(485, 15), Size = new Size(90, 30) };
            btnClear.Click += (s, e) =>
            {
                shapes.Clear();
                this.Invalidate();
            };

            topPanel.Controls.Add(btnLine);
            topPanel.Controls.Add(btnRect);
            topPanel.Controls.Add(btnCircle);
            topPanel.Controls.Add(btnTriangle);
            topPanel.Controls.Add(btnColor);
            topPanel.Controls.Add(btnClear);

            this.Controls.Add(topPanel);
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Y < 65) return; // Цэс байрлах хэсэгт зурахгүй байх

            if (e.Button == MouseButtons.Left)
            {
                isDrawing = true;

                switch (selectedTool)
                {
                    case "Line":
                        currentShape = new Line(currentColor, currentWidth, e.Location);
                        break;
                    case "Rectangle":
                        currentShape = new RectangleShape(currentColor, currentWidth, e.Location);
                        break;
                    case "Circle":
                        currentShape = new Circle(currentColor, currentWidth, e.Location);
                        break;
                    case "Triangle":
                        currentShape = new Triangle(currentColor, currentWidth, e.Location);
                        break;
                }
            }
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing && currentShape != null)
            {
                currentShape.End = e.Location;
                this.Invalidate();
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (isDrawing && currentShape != null)
            {
                currentShape.End = e.Location;
                shapes.Add(currentShape);
                currentShape = null;
                isDrawing = false;
                this.Invalidate();
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            foreach (var shape in shapes)
            {
                shape.Draw(e.Graphics);
            }

            if (currentShape != null)
            {
                currentShape.Draw(e.Graphics);
            }
        }

        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}