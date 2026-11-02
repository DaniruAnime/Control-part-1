
using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace QuadraticEquation
{
    public class MainForm : Form
    {
        private TextBox txtA;
        private TextBox txtB;
        private TextBox txtC;

        private Label lblDiscriminant;
        private Label lblRoots;
        private Label lblVieta;
        private Label lblEquation;
        private Label lblStatus;

        private GraphPanel graph;

        public MainForm()
        {
            Text = "Quadratic Equation Solver";
            Size = new Size(1150, 760);
            MinimumSize = new Size(900, 620);

            StartPosition = FormStartPosition.CenterScreen;

            BackColor = SystemColors.Control;
            Font = new Font("Arial", 9);

            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = true;
            MinimizeBox = true;

            CreateInterface();
        }

        private void CreateInterface()
        {
            // Главное меню

            MenuStrip menu = new MenuStrip();

            ToolStripMenuItem fileMenu =
                new ToolStripMenuItem("Файл");

            ToolStripMenuItem clearItem =
                new ToolStripMenuItem("Очистить");

            ToolStripMenuItem exitItem =
                new ToolStripMenuItem("Выход");

            clearItem.Click += delegate
            {
                ClearFields();
            };

            exitItem.Click += delegate
            {
                Close();
            };

            fileMenu.DropDownItems.Add(clearItem);
            fileMenu.DropDownItems.Add(new ToolStripSeparator());
            fileMenu.DropDownItems.Add(exitItem);

            ToolStripMenuItem helpMenu =
                new ToolStripMenuItem("Справка");

            ToolStripMenuItem aboutItem =
                new ToolStripMenuItem("О программе");

            aboutItem.Click += delegate
            {
                MessageBox.Show(
                    "Quadratic Equation Solver 2.0\n\n" +
                    "Программа для решения квадратных уравнений.\n" +
                    "Построение графиков и проверка теоремы Виета.\n\n" +
                    "C# / Windows Forms / Mono",
                    "О программе",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            };

            helpMenu.DropDownItems.Add(aboutItem);

            menu.Items.Add(fileMenu);
            menu.Items.Add(helpMenu);

            MainMenuStrip = menu;
            Controls.Add(menu);

            // Левая панель

            Panel sidebar = new Panel();

            sidebar.Dock = DockStyle.Left;
            sidebar.Width = 345;
            sidebar.BackColor = SystemColors.Control;
            sidebar.Padding = new Padding(10);

            Controls.Add(sidebar);

            Label title = new Label();

            title.Text = "Quadratic Equation";
            title.Font = new Font(
                "Arial", 15, FontStyle.Bold
            );

            title.AutoSize = true;
            title.Location = new Point(15, 15);

            sidebar.Controls.Add(title);

            Label subtitle = new Label();

            subtitle.Text = "Решение квадратных уравнений";
            subtitle.AutoSize = true;
            subtitle.ForeColor = SystemColors.GrayText;
            subtitle.Location = new Point(17, 43);

            sidebar.Controls.Add(subtitle);

            // Группа коэффициентов

            GroupBox inputGroup = new GroupBox();

            inputGroup.Text = "Коэффициенты уравнения";
            inputGroup.Location = new Point(10, 75);
            inputGroup.Size = new Size(315, 205);

            sidebar.Controls.Add(inputGroup);

            Label equationLabel = new Label();

            equationLabel.Text = "ax² + bx + c = 0";
            equationLabel.Font = new Font(
                "Arial", 12, FontStyle.Bold
            );

            equationLabel.AutoSize = true;
            equationLabel.Location = new Point(85, 25);

            inputGroup.Controls.Add(equationLabel);

            txtA = CreateInput(
                inputGroup, "A:", "1", 60
            );

            txtB = CreateInput(
                inputGroup, "B:", "-5", 100
            );

            txtC = CreateInput(
                inputGroup, "C:", "6", 140
            );

            // Кнопки

            Button calculateButton = new Button();

            calculateButton.Text = "Вычислить";
            calculateButton.Location = new Point(10, 290);
            calculateButton.Size = new Size(155, 35);
            calculateButton.FlatStyle = FlatStyle.Standard;

            calculateButton.Click += delegate
            {
                CalculateEquation();
            };

            sidebar.Controls.Add(calculateButton);

            Button clearButton = new Button();

            clearButton.Text = "Очистить";
            clearButton.Location = new Point(175, 290);
            clearButton.Size = new Size(150, 35);
            clearButton.FlatStyle = FlatStyle.Standard;

            clearButton.Click += delegate
            {
                ClearFields();
            };

            sidebar.Controls.Add(clearButton);

            // Группа результатов

            GroupBox resultGroup = new GroupBox();

            resultGroup.Text = "Результаты";
            resultGroup.Location = new Point(10, 340);
            resultGroup.Size = new Size(315, 130);

            sidebar.Controls.Add(resultGroup);

            lblDiscriminant = CreateResultLabel(
                resultGroup, "Дискриминант: --", 25, 280, 25
            );

            lblRoots = CreateResultLabel(
                resultGroup, "Корни: --", 55, 280, 65
            );

            // Группа теоремы Виета

            GroupBox vietaGroup = new GroupBox();

            vietaGroup.Text = "Проверка теоремы Виета";
            vietaGroup.Location = new Point(10, 480);
            vietaGroup.Size = new Size(315, 155);

            sidebar.Controls.Add(vietaGroup);

            lblVieta = new Label();

            lblVieta.Text = "Ожидание вычислений...";
            lblVieta.Location = new Point(10, 25);
            lblVieta.Size = new Size(290, 120);
            lblVieta.AutoSize = false;

            vietaGroup.Controls.Add(lblVieta);

            // Правая область

            graph = new GraphPanel();

            graph.Dock = DockStyle.Fill;

            Controls.Add(graph);

            // Панель состояния

            StatusStrip status = new StatusStrip();

            lblStatus = new Label();
            lblStatus.Text = "Готово";

            ToolStripStatusLabel statusText =
                new ToolStripStatusLabel("Готово");

            status.Items.Add(statusText);

            Controls.Add(status);

            sidebar.BringToFront();
            menu.BringToFront();
            status.BringToFront();

            // Начальные значения

            CalculateEquation();
        }

        private TextBox CreateInput(
            Control parent,
            string name,
            string value,
            int top)
        {
            Label label = new Label();

            label.Text = name;
            label.Location = new Point(20, top + 3);
            label.Size = new Size(30, 23);

            parent.Controls.Add(label);

            TextBox input = new TextBox();

            input.Text = value;
            input.Location = new Point(65, top);
            input.Size = new Size(210, 23);

            input.BorderStyle = BorderStyle.Fixed3D;

            parent.Controls.Add(input);

            return input;
        }

        private Label CreateResultLabel(
            Control parent,
            string text,
            int top,
            int width,
            int height)
        {
            Label label = new Label();

            label.Text = text;
            label.Location = new Point(12, top);
            label.Size = new Size(width, height);
            label.AutoSize = false;

            parent.Controls.Add(label);

            return label;
        }

        // Проверка входных данных

        private bool TryReadNumber(
            TextBox input,
            out double value)
        {
            string text = input.Text.Trim();

            text = text.Replace(',', '.');

            bool success = double.TryParse(
                text,
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out value
            );

            return success &&
                   IsFinite(value);
        }

        private bool IsFinite(double value)
        {
            return !double.IsNaN(value) &&
                   !double.IsInfinity(value);
        }

        private bool NearlyEqual(double first, double second)
        {
            const double tolerance = 1e-8;

            if (!IsFinite(first) || !IsFinite(second))
                return false;

            double difference = Math.Abs(first - second);

            double scale = Math.Max(
                1.0,
                Math.Max(Math.Abs(first), Math.Abs(second))
            );

            return difference <= tolerance * scale;
        }

        // Основные вычисления

        private void CalculateEquation()
        {
            double a, b, c;

            if (!TryReadNumber(txtA, out a) ||
                !TryReadNumber(txtB, out b) ||
                !TryReadNumber(txtC, out c))
            {
                MessageBox.Show(
                    "Введите корректные числовые коэффициенты.\n\n" +
                    "Допустимые примеры: 5, -3, 2.5, 2,5.",
                    "Ошибка ввода",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                lblStatus.Text = "Ошибка ввода";
                return;
            }

            if (a == 0)
            {
                MessageBox.Show(
                    "Коэффициент A не может быть равен нулю.\n\n" +
                    "Уравнение должно быть квадратным.",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                lblStatus.Text = "Некорректное уравнение";
                return;
            }

            // Нормализация коэффициентов.
            // Корни уравнения при этом не меняются.

            double scale = Math.Max(
                Math.Abs(a),
                Math.Max(Math.Abs(b), Math.Abs(c))
            );

            double A = a / scale;
            double B = b / scale;
            double C = c / scale;

            if (A == 0)
            {
                MessageBox.Show(
                    "Коэффициенты имеют слишком разные порядки величин.\n" +
                    "Невозможно надёжно вычислить корни в double.",
                    "Ошибка вычисления",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            // Дискриминант нормализованного уравнения.
            // Его знак совпадает со знаком исходного D.

            double D = B * B - 4.0 * A * C;

            if (!IsFinite(D))
            {
                MessageBox.Show(
                    "Не удалось вычислить дискриминант.",
                    "Ошибка вычисления",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            double actualD = D * scale * scale;

            string discriminantText;

            if (IsFinite(actualD))
            {
                discriminantText = actualD.ToString(
                    "G8", CultureInfo.InvariantCulture
                );
            }
            else
            {
                discriminantText =
                    D > 0 ? "Положительный, превышает диапазон double" :
                    D < 0 ? "Отрицательный, превышает диапазон double" :
                    "0";
            }

            lblDiscriminant.Text =
                "Дискриминант: " + discriminantText;

            lblEquation.Text =
                "y = " + a.ToString("G5") +
                "x² + " + b.ToString("G5") +
                "x + " + c.ToString("G5");

            bool hasRoots = D >= 0;

            double x1 = 0;
            double x2 = 0;

            // Вычисление корней

            if (D > 0)
            {
                double sqrtD = Math.Sqrt(D);

                // Устойчивая формула вычисления корней.

                double q = -0.5 * (
                    B + (B >= 0 ? sqrtD : -sqrtD)
                );

                if (q == 0)
                {
                    MessageBox.Show(
                        "Не удалось вычислить корни.",
                        "Ошибка",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    return;
                }

                x1 = q / A;
                x2 = C / q;
            }
            else if (D == 0)
            {
                x1 = -B / (2.0 * A);
                x2 = x1;
            }

            if (hasRoots)
            {
                if (!IsFinite(x1) || !IsFinite(x2))
                {
                    MessageBox.Show(
                        "Корни выходят за допустимый диапазон double.",
                        "Ошибка вычисления",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    return;
                }

                if (D > 0)
                {
                    lblRoots.Text =
                        "x1 = " + x1.ToString("G8") +
                        "\nx2 = " + x2.ToString("G8");
                }
                else
                {
                    lblRoots.Text =
                        "Один корень:\nx = " +
                        x1.ToString("G8");
                }

                CalculateVieta(x1, x2, A, B, C);
            }
            else
            {
                lblRoots.Text =
                    "Действительных корней нет.";

                lblVieta.Text =
                    "Действительных корней нет.\n\n" +
                    "Проверка Виета для действительных корней " +
                    "не выполняется.";
            }

            graph.SetFunction(
                a, b, c,
                hasRoots, x1, x2
            );

            lblStatus.Text = "Вычисления завершены";
        }

        // Проверка теоремы Виета

        private void CalculateVieta(
            double x1,
            double x2,
            double A,
            double B,
            double C)
        {
            double sum = x1 + x2;
            double product = x1 * x2;

            double expectedSum = -B / A;
            double expectedProduct = C / A;

            if (!IsFinite(sum) ||
                !IsFinite(product) ||
                !IsFinite(expectedSum) ||
                !IsFinite(expectedProduct))
            {
                lblVieta.Text =
                    "Не удалось проверить теорему Виета:\n" +
                    "переполнение при вычислении суммы или произведения.";

                return;
            }

            bool sumCorrect = NearlyEqual(
                sum, expectedSum
            );

            bool productCorrect = NearlyEqual(
                product, expectedProduct
            );

            lblVieta.Text =
                "Сумма корней:\n" +
                "x1 + x2 = " + sum.ToString("G6") + "\n" +
                "-b / a = " + expectedSum.ToString("G6") + "\n\n" +

                "Произведение:\n" +
                "x1 * x2 = " + product.ToString("G6") + "\n" +
                "c / a = " + expectedProduct.ToString("G6") + "\n\n" +

                (sumCorrect && productCorrect
                    ? "Теорема Виета выполняется."
                    : "Проверка не пройдена.");
        }

        // Очистка формы

        private void ClearFields()
        {
            txtA.Clear();
            txtB.Clear();
            txtC.Clear();

            lblDiscriminant.Text = "Дискриминант: --";
            lblRoots.Text = "Корни: --";

            lblVieta.Text = "Ожидание вычислений...";
            lblEquation.Text = "y = ax² + bx + c";

            lblStatus.Text = "Поля очищены";

            graph.ClearGraph();

            txtA.Focus();
        }
    }

    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            // Не включаем современные визуальные стили:
            // используем классические элементы WinForms.

            Application.EnableVisualStyles();

            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new MainForm());
        }
    }
}
