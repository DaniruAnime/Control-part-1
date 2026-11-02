
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Windows.Forms;

namespace QuadraticEquation
{
    public class GraphPanel : Panel
    {
        private double a;
        private double b;
        private double c;

        private double x1;
        private double x2;

        private bool hasFunction;
        private bool hasRoots;

        private const int LeftMargin = 65;
        private const int RightMargin = 25;
        private const int TopMargin = 30;
        private const int BottomMargin = 45;

        public GraphPanel()
        {
            DoubleBuffered = true;
            ResizeRedraw = true;

            BackColor = Color.White;
            BorderStyle = BorderStyle.Fixed3D;
        }

        public void SetFunction(
            double a, double b, double c,
            bool hasRoots, double x1, double x2)
        {
            this.a = a;
            this.b = b;
            this.c = c;

            this.hasRoots = hasRoots;
            this.x1 = x1;
            this.x2 = x2;

            hasFunction = true;

            Invalidate();
        }

        public void ClearGraph()
        {
            hasFunction = false;
            hasRoots = false;

            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle area = new Rectangle(
                LeftMargin,
                TopMargin,
                Width - LeftMargin - RightMargin,
                Height - TopMargin - BottomMargin
            );

            if (area.Width <= 0 || area.Height <= 0)
                return;

            if (!hasFunction)
            {
                DrawMessage(g, area);
                return;
            }

            DrawGraph(g, area);
        }

        private void DrawGraph(Graphics g, Rectangle area)
        {
            // Нормализация коэффициентов.
            // Она помогает избежать переполнения
            // при построении графика.

            double scale = Math.Max(
                Math.Abs(a),
                Math.Max(Math.Abs(b), Math.Abs(c))
            );

            if (scale == 0)
                return;

            double A = a / scale;
            double B = b / scale;
            double C = c / scale;

            double vertexX = -B / (2 * A);

            if (double.IsNaN(vertexX) ||
                double.IsInfinity(vertexX))
            {
                DrawMessage(g, area);
                return;
            }

            double xMin = vertexX - 5;
            double xMax = vertexX + 5;

            // Если есть корни, используем их
            // для выбора диапазона X.

            if (hasRoots &&
                !double.IsInfinity(x1) &&
                !double.IsInfinity(x2))
            {
                double minRoot = Math.Min(x1, x2);
                double maxRoot = Math.Max(x1, x2);

                double margin = Math.Max(
                    2,
                    Math.Abs(maxRoot - minRoot) * 0.6
                );

                xMin = minRoot - margin;
                xMax = maxRoot + margin;
            }

            if (double.IsInfinity(xMin) ||
                double.IsInfinity(xMax) ||
                xMin == xMax)
            {
                DrawMessage(g, area);
                return;
            }

            // Ограничиваем диапазон для корректной отрисовки.
            if (Math.Abs(xMin) > 1e100 ||
                Math.Abs(xMax) > 1e100)
            {
                DrawMessage(g, area);
                return;
            }

            double vertexY =
                (A * vertexX + B) * vertexX + C;

            if (!IsFinite(vertexY))
            {
                DrawMessage(g, area);
                return;
            }

            double yMin = Math.Min(0, vertexY);
            double yMax = Math.Max(0, vertexY);

            // Вычисление диапазона Y.
            for (int i = 0; i <= 200; i++)
            {
                double x = xMin +
                    (xMax - xMin) * i / 200.0;

                double y = (A * x + B) * x + C;

                if (!IsFinite(y))
                {
                    DrawMessage(g, area);
                    return;
                }

                yMin = Math.Min(yMin, y);
                yMax = Math.Max(yMax, y);
            }

            double padding = Math.Max(
                1,
                (yMax - yMin) * 0.1
            );

            yMin -= padding;
            yMax += padding;

            if (Math.Abs(yMax - yMin) < 0.000001)
            {
                yMin -= 1;
                yMax += 1;
            }

            Func<double, float> mapX = delegate(double x)
            {
                return area.Left + (float)(
                    (x - xMin) / (xMax - xMin) * area.Width
                );
            };

            Func<double, float> mapY = delegate(double y)
            {
                return area.Bottom - (float)(
                    (y - yMin) / (yMax - yMin) * area.Height
                );
            };

            DrawGrid(g, area, xMin, xMax, yMin, yMax);

            // Координатные оси.

            using (Pen axisPen = new Pen(Color.Black, 1.5f))
            {
                if (yMin <= 0 && yMax >= 0)
                {
                    g.DrawLine(
                        axisPen,
                        mapX(xMin), mapY(0),
                        mapX(xMax), mapY(0)
                    );
                }

                if (xMin <= 0 && xMax >= 0)
                {
                    g.DrawLine(
                        axisPen,
                        mapX(0), mapY(yMin),
                        mapX(0), mapY(yMax)
                    );
                }
            }

            // Построение параболы.

            using (Pen graphPen = new Pen(
                Color.FromArgb(45, 90, 170), 2.5f))
            {
                PointF previous = PointF.Empty;
                bool first = true;

                for (int i = 0; i <= 1200; i++)
                {
                    double x = xMin +
                        (xMax - xMin) * i / 1200.0;

                    double y = (A * x + B) * x + C;

                    if (!IsFinite(y))
                    {
                        first = true;
                        continue;
                    }

                    PointF current = new PointF(
                        mapX(x),
                        mapY(y)
                    );

                    if (!first)
                        g.DrawLine(graphPen, previous, current);

                    previous = current;
                    first = false;
                }
            }

            // Отметки корней.

            if (hasRoots && yMin <= 0 && yMax >= 0)
            {
                DrawPoint(
                    g, mapX(x1), mapY(0),
                    "x1"
                );

                if (Math.Abs(x1 - x2) > 0.000000001)
                {
                    DrawPoint(
                        g, mapX(x2), mapY(0),
                        "x2"
                    );
                }
            }

            // Вершина параболы.

            DrawPoint(
                g,
                mapX(vertexX),
                mapY(vertexY),
                "V"
            );

            using (Font font = new Font("Arial", 9))
            {
                if (yMin <= 0 && yMax >= 0)
                {
                    g.DrawString(
                        "X", font, Brushes.Black,
                        area.Right - 15,
                        mapY(0) - 20
                    );
                }

                if (xMin <= 0 && xMax >= 0)
                {
                    g.DrawString(
                        "Y", font, Brushes.Black,
                        mapX(0) + 8,
                        area.Top
                    );
                }
            }
        }

        private void DrawGrid(
            Graphics g,
            Rectangle area,
            double xMin, double xMax,
            double yMin, double yMax)
        {
            using (Pen gridPen = new Pen(
                Color.FromArgb(220, 220, 220), 1))
            using (Font font = new Font("Arial", 8))
            using (Brush brush = new SolidBrush(Color.Gray))
            {
                for (int i = 0; i <= 10; i++)
                {
                    float x = area.Left +
                        area.Width * i / 10f;

                    float y = area.Top +
                        area.Height * i / 10f;

                    g.DrawLine(
                        gridPen,
                        x, area.Top,
                        x, area.Bottom
                    );

                    g.DrawLine(
                        gridPen,
                        area.Left, y,
                        area.Right, y
                    );

                    double valueX = xMin +
                        (xMax - xMin) * i / 10.0;

                    double valueY = yMax -
                        (yMax - yMin) * i / 10.0;

                    g.DrawString(
                        valueX.ToString("G3"),
                        font, brush,
                        x - 12,
                        area.Bottom + 5
                    );

                    g.DrawString(
                        valueY.ToString("G3"),
                        font, brush,
                        2, y - 7
                    );
                }
            }
        }

        private void DrawPoint(
            Graphics g, float x, float y, string name)
        {
            using (Brush pointBrush = new SolidBrush(
                Color.FromArgb(20, 130, 70)))
            using (Font font = new Font(
                "Arial", 9, FontStyle.Bold))
            {
                g.FillEllipse(
                    pointBrush,
                    x - 4, y - 4,
                    8, 8
                );

                g.DrawString(
                    name,
                    font,
                    Brushes.DarkGreen,
                    x + 5,
                    y - 18
                );
            }
        }

        private void DrawMessage(
            Graphics g, Rectangle area)
        {
            const string message =
                "Невозможно построить график в выбранном масштабе";

            using (Font font = new Font("Arial", 10))
            {
                SizeF size = g.MeasureString(message, font);

                g.DrawString(
                    message,
                    font,
                    Brushes.Gray,
                    area.Left + (area.Width - size.Width) / 2,
                    area.Top + (area.Height - size.Height) / 2
                );
            }
        }

        private bool IsFinite(double value)
        {
            return !double.IsNaN(value) &&
                   !double.IsInfinity(value);
        }
    }
}