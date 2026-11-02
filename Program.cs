
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
        private Label lblEquation;
        private Label lblVieta;

        private StatusBar status;

        public MainForm()
        {
            Text = "Quadratic Equation Solver";
            Size = new Size(760, 600);
            MinimumSize = new Size(680, 520);

            StartPosition = FormStartPosition.CenterScreen;
            BackColor = SystemColors.Control;
            Font = new Font("Arial", 9);

            CreateInterface();
        }

        private void CreateInterface()
        {
            // Классическое меню Windows

            MainMenu mainMenu = new MainMenu();

            MenuItem fileMenu = new MenuItem("&Файл");

            MenuItem clearItem = new MenuItem("&Очистить");
            MenuItem exitItem = new MenuItem("&Выход");

            clearItem.Click += delegate
            {
                ClearFields();
            };

            exitItem.Click += delegate
            {
                Close();
            };

            fileMenu.MenuItems.Add(clearItem);
            fileMenu.MenuItems.Add("-");
            fileMenu.MenuItems.Add(exitItem);

            MenuItem helpMenu = new MenuItem("&Справка");

            helpMenu.MenuItems.Add(
                new MenuItem("&О программе", delegate
                {
                    MessageBox.Show(
                        "Quadratic Equation Solver 2.0\n\n" +
                        "Программа для решения квадратных уравнений.\n\n" +
                        "Вычисление дискриминанта.\n" +
                        "Нахождение корней.\n" +
                        "Проверка теоремы Виета.\n\n" +
                        "C# / Windows Forms / Mono",
                        "О программе",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                })
            );

            mainMenu.MenuItems.Add(fileMenu);
            mainMenu.MenuItems.Add(helpMenu);

            Menu = mainMenu;

            // Основная область

            Panel mainPanel = new Panel();

            mainPanel.Dock = DockStyle.Fill;
            mainPanel.Padding = new Padding(12);

            Controls.Add(mainPanel);

            // Заголовок

            Label title = new Label();

            title.Text = "Quadratic Equation Solver";
            title.Font = new Font(
                "Arial", 16, FontStyle.Bold
            );

            title.Location = new Point(15, 10);
            title.Size = new Size(500, 30);

            mainPanel.Controls.Add(title);

            Label subtitle = new Label();

            subtitle.Text = "Решение квадратных уравнений";
            subtitle.ForeColor = SystemColors.GrayText;

            subtitle.Location = new Point(17, 43);
            subtitle.Size = new Size(400, 20);

            mainPanel.Controls.Add(subtitle);

            // Группа ввода

            GroupBox inputGroup = new GroupBox();

            inputGroup.Text = "Ввод коэффициентов";
            inputGroup.Location = new Point(10, 75);
            inputGroup.Size = new Size(700, 130);

            inputGroup.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Left |
                AnchorStyles.Right;

            mainPanel.Controls.Add(inputGroup);

            Label formula = new Label();

            formula.Text = "ax² + bx + c = 0";
            formula.Font = new Font(
                "Arial", 12, FontStyle.Bold
            );

            formula.Location = new Point(20, 25);
            formula.Size = new Size(200, 25);

            inputGroup.Controls.Add(formula);

            txtA = CreateInput(inputGroup, "A:", "1", 65);
            txtB = CreateInput(inputGroup, "B:", "-5", 255);
            txtC = CreateInput(inputGroup, "C:", "6", 445);

            // Кнопка вычисления

            Button calculateButton = new Button();

            calculateButton.Text = "Вычислить";
            calculateButton.Location = new Point(565, 50);
            calculateButton.Size = new Size(115, 30);

            calculateButton.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Right;

            calculateButton.Click += delegate
            {
                CalculateEquation();
            };

            inputGroup.Controls.Add(calculateButton);

            Button clearButton = new Button();

            clearButton.Text = "Очистить";
            clearButton.Location = new Point(565, 85);
            clearButton.Size = new Size(115, 30);

            clearButton.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Right;

            clearButton.Click += delegate
            {
                ClearFields();
            };

            inputGroup.Controls.Add(clearButton);

            AcceptButton = calculateButton;

            // Группа результатов

            GroupBox resultGroup = new GroupBox();

            resultGroup.Text = "Результаты вычислений";
            resultGroup.Location = new Point(10, 215);
            resultGroup.Size = new Size(700, 140);

            resultGroup.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Left |
                AnchorStyles.Right;

            mainPanel.Controls.Add(resultGroup);

            lblDiscriminant = CreateResultLabel(
                resultGroup,
                "Дискриминант: --",
                25,
                670,
                25
            );

            lblRoots = CreateResultLabel(
                resultGroup,
                "Корни: --",
                55,
                670,
                50
            );

            lblEquation = CreateResultLabel(
                resultGroup,
                "Уравнение: --",
                110,
                670,
                25
            );

            // Группа теоремы Виета

            GroupBox vietaGroup = new GroupBox();

            vietaGroup.Text = "Проверка теоремы Виета";
            vietaGroup.Location = new Point(10, 365);
            vietaGroup.Size = new Size(700, 160);

            vietaGroup.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Bottom |
                AnchorStyles.Left |
                AnchorStyles.Right;

            mainPanel.Controls.Add(vietaGroup);

            lblVieta = new Label();

            lblVieta.Text = "Ожидание вычислений...";
            lblVieta.Location = new Point(15, 25);
            lblVieta.Size = new Size(665, 125);

            lblVieta.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Bottom |
                AnchorStyles.Left |
                AnchorStyles.Right;

            vietaGroup.Controls.Add(lblVieta);

            // Строка состояния

            status = new StatusBar();

            status.Text = "Готово";

            Controls.Add(status);

            // Первоначальный расчёт

            CalculateEquation();
        }

        private TextBox CreateInput(
            Control parent,
            string name,
            string value,
            int left)
        {
            Label label = new Label();

            label.Text = name;
            label.Location = new Point(left, 68);
            label.Size = new Size(25, 23);

            parent.Controls.Add(label);

            TextBox input = new TextBox();

            input.Text = value;
            input.Location = new Point(left + 30, 65);
            input.Size = new Size(130, 23);

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
            label.Location = new Point(15, top);
            label.Size = new Size(width, height);
            label.AutoSize = false;

            parent.Controls.Add(label);

            return label;
        }

        // Проверка числовых значений

        private bool IsFinite(double value)
        {
            return !double.IsNaN(value) &&
                   !double.IsInfinity(value);
        }

        private bool TryReadNumber(
            TextBox input,
            out double value)
        {
            string text = input.Text.Trim().Replace(',', '.');

            bool success = double.TryParse(
                text,
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out value
            );

            return success && IsFinite(value);
        }

        // Сравнение чисел с относительной погрешностью

        private bool NearlyEqual(
            double first,
            double second)
        {
            const double tolerance = 1e-8;

            if (!IsFinite(first) || !IsFinite(second))
                return false;

            double scale = Math.Max(
                Math.Abs(first),
                Math.Abs(second)
            );

            if (scale == 0)
                return true;

            return Math.Abs(
                first / scale - second / scale
            ) <= tolerance;
        }

        // Форматирование дискриминанта

        private string FormatDiscriminant(
            double normalizedD,
            double scale)
        {
            if (normalizedD == 0)
                return "0";

            double actualD = (normalizedD * scale) * scale;

            if (IsFinite(actualD) && actualD != 0)
            {
                return actualD.ToString(
                    "G8",
                    CultureInfo.CurrentCulture
                );
            }

            double logarithm =
                Math.Log10(Math.Abs(normalizedD)) +
                2.0 * Math.Log10(scale);

            int exponent = (int)Math.Floor(logarithm);

            double mantissa = Math.Pow(
                10.0,
                logarithm - exponent
            );

            string sign = normalizedD < 0 ? "-" : "";

            return sign +
                   mantissa.ToString(
                       "G6",
                       CultureInfo.InvariantCulture
                   ) +
                   "E" +
                   exponent.ToString(
                       "+0;-0;0",
                       CultureInfo.InvariantCulture
                   );
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
                    "Примеры: 5, -3, 2.5, 2,5.",
                    "Ошибка ввода",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                status.Text = "Ошибка ввода";
                return;
            }

            if (a == 0)
            {
                MessageBox.Show(
                    "Коэффициент A не должен быть равен нулю.\n\n" +
                    "Уравнение должно быть квадратным.",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                status.Text = "Некорректное уравнение";
                return;
            }

            // Нормализация коэффициентов

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
                    "Слишком большая разница между коэффициентами.\n" +
                    "Невозможно надёжно вычислить корни.",
                    "Ошибка вычисления",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                status.Text = "Ошибка вычисления";
                return;
            }

            // Дискриминант нормализованного уравнения

            double D = B * B - 4.0 * A * C;

            if (!IsFinite(D))
            {
                MessageBox.Show(
                    "Не удалось вычислить дискриминант.",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                status.Text = "Ошибка вычисления";
                return;
            }

            lblDiscriminant.Text =
                "Дискриминант D = " +
                FormatDiscriminant(D, scale);

            lblEquation.Text =
                "Уравнение: " +
                a.ToString("G6") + "x² + (" +
                b.ToString("G6") + ")x + (" +
                c.ToString("G6") + ") = 0";

            double x1 = 0;
            double x2 = 0;

            // Определение количества корней

            if (D < 0)
            {
                lblRoots.Text =
                    "Действительных корней нет.";

                lblVieta.Text =
                    "Дискриминант отрицательный.\n\n" +
                    "Действительных корней нет, поэтому " +
                    "проверка Виета для действительных корней невозможна.";

                status.Text = "Вычисления завершены";
                return;
            }

            // Вычисление корней

            if (D == 0)
            {
                x1 = -B / (2.0 * A);
                x2 = x1;
            }
            else
            {
                double sqrtD = Math.Sqrt(D);

                // Устойчивая формула вычисления корней

                double q = -0.5 * (
                    B + (B >= 0 ? sqrtD : -sqrtD)
                );

                if (q == 0)
                {
                    ShowCalculationError();
                    return;
                }

                x1 = q / A;
                x2 = C / q;
            }

            if (!IsFinite(x1) || !IsFinite(x2))
            {
                ShowCalculationError();
                return;
            }

            // Вывод корней

            if (D == 0)
            {
                lblRoots.Text =
                    "Один корень:\nx = " +
                    x1.ToString("G8");
            }
            else
            {
                lblRoots.Text =
                    "x1 = " + x1.ToString("G8") +
                    "\nx2 = " + x2.ToString("G8");
            }

            // Проверка теоремы Виета

            CalculateVieta(x1, x2, A, B, C);

            status.Text = "Вычисления завершены";
        }

        private void ShowCalculationError()
        {
            MessageBox.Show(
                "Корни выходят за допустимый диапазон double.\n\n" +
                "Попробуйте использовать другие коэффициенты.",
                "Ошибка вычисления",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );

            status.Text = "Ошибка вычисления";
        }

        // Теорема Виета

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
                    "Невозможно проверить теорему Виета:\n" +
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
                "x1 + x2 = " + sum.ToString("G8") + "\n" +
                "-b / a = " + expectedSum.ToString("G8") + "\n\n" +

                "Произведение корней:\n" +
                "x1 * x2 = " + product.ToString("G8") + "\n" +
                "c / a = " + expectedProduct.ToString("G8") + "\n\n" +

                (sumCorrect && productCorrect
                    ? "Теорема Виета выполняется."
                    : "Проверка теоремы Виета не пройдена.");
        }

        // Очистка полей

        private void ClearFields()
        {
            txtA.Clear();
            txtB.Clear();
            txtC.Clear();

            lblDiscriminant.Text = "Дискриминант: --";
            lblRoots.Text = "Корни: --";
            lblEquation.Text = "Уравнение: --";

            lblVieta.Text = "Ожидание вычислений...";

            status.Text = "Поля очищены";

            txtA.Focus();
        }
    }

    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new MainForm());
        }
    }
}
