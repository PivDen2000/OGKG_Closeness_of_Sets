using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Lab_OGKG
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        private KD Tree;
        public MainWindow()
        {
            InitializeComponent();
            txtNum.Text = _numValue.ToString();
            txtNum2.Text = _numValue2.ToString();
            var handle = GetConsoleWindow();

            // Hide
            ShowWindow(handle, SW_HIDE);
        }


        private int _numValue = 0;
        private int _numValue2 = 0;

        public int NumValue
        {
            get { return _numValue; }
            set
            {
                _numValue = value;
                txtNum.Text = value.ToString();
            }
        }

        public int NumValue2
        {
            get { return _numValue2; }
            set
            {
                _numValue2 = value;
                txtNum2.Text = value.ToString();
            }
        }

        private void cmdUp2_Click(object sender, RoutedEventArgs e)
        {
            NumValue2++;
        }

        private void cmdDown2_Click(object sender, RoutedEventArgs e)
        {
            NumValue2--;
        }

        private void txtNum2_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtNum2 == null)
            {
                return;
            }

            if (!int.TryParse(txtNum2.Text, out _numValue2))
                txtNum2.Text = _numValue2.ToString();
        }

        private void cmdUp_Click(object sender, RoutedEventArgs e)
        {
            NumValue++;
        }

        private void cmdDown_Click(object sender, RoutedEventArgs e)
        {
            NumValue--;
        }

        private void txtNum_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtNum == null)
            {
                return;
            }

            if (!int.TryParse(txtNum.Text, out _numValue))
                txtNum.Text = _numValue.ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if ((_numValue <= 0) || (_numValue2 <= 0))
            {
                MessageBox.Show("Кiлькiсть не може бути <= 0 !!!");
                return;
            }
            canvas.Children.Clear();
            Tree = new KD();
            Tree.RandomGenerate(_numValue, _numValue2, 350);
            Tree.FindNearest();
            foreach (var point in Tree.points1)
            {
                string text = "(" + Math.Round(point[0], 1) + ";" + Math.Round(point[1], 1) + ")";
                var textBlock = new TextBlock
                {
                    Text = text,
                    TextWrapping = TextWrapping.Wrap,
                    FontFamily = new FontFamily("Comic Sans MS, Verdana"),
                    FontSize = 8
                };

                var ellipse = new Ellipse();
                ellipse.Width = 10;
                ellipse.Height = 10;
                ellipse.Fill = Brushes.Green;
                Canvas.SetLeft(ellipse,  point[0]);
                Canvas.SetTop(ellipse,  350 - point[1]);
                Canvas.SetLeft(textBlock,  point[0] - 15);
                Canvas.SetTop(textBlock,  350 - point[1] + 10);
                canvas.Children.Add(ellipse);
                canvas.Children.Add(textBlock);
            }

            foreach (var point in Tree.points2)
            {
                string text = "(" + Math.Round(point[0], 1) + ";" + Math.Round(point[1], 1) + ")";
                var textBlock = new TextBlock
                {
                    Text = text,
                    TextWrapping = TextWrapping.Wrap,
                    FontFamily = new FontFamily("Comic Sans MS, Verdana"),
                    FontSize = 8
                };
                var ellipse = new Ellipse();
                ellipse.Width = 10;
                ellipse.Height = 10;
                ellipse.Fill = Brushes.Red;
                Canvas.SetLeft(ellipse, point[0]);
                Canvas.SetTop(ellipse, 350 - point[1]);
                Canvas.SetLeft(textBlock, point[0] - 15);
                Canvas.SetTop(textBlock, 350 - point[1] + 10);
                canvas.Children.Add(ellipse);
                canvas.Children.Add(textBlock);
            }

            var line = new Line();
            line.Stroke = Brushes.Aqua;

            line.X1 = Tree.firstPoint[0] + 3;
            line.Y1 = 350 - Tree.firstPoint[1] + 3;
            line.X2 = Tree.secondPoint[0] + 3;
            line.Y2 = 350 - Tree.secondPoint[1] + 3;

            line.StrokeThickness = 5;
            canvas.Children.Add(line);
            res_text.Text = "Point from array A: (" + Math.Round(Tree.firstPoint[0], 1) + ";" + Math.Round( Tree.firstPoint[1], 1) +  ") \n" + "Point from array B: (" + Math.Round( Tree.secondPoint[0], 1) + ";" + Math.Round( Tree.secondPoint[1], 1) + ")\n" + "Distance: " + Math.Round( Math.Sqrt(Tree.minDistance), 1) + "\n";

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if ((_numValue <= 0) || (_numValue2 <= 0))
            {
                MessageBox.Show("Кiлькiсть не може бути <= 0 !!!");
                return;
            }
            canvas.Children.Clear();
            Console.WriteLine("Маемо площину 350х350.");
            var handle = GetConsoleWindow();
            ShowWindow(handle, SW_SHOW);
            Console.WriteLine("Ведiть усi точки множини А:");
            List<double[]> points1 = new List<double[]>();
            for (int i = 0; i < _numValue; i++)
            {
                Console.WriteLine("Точка [" + (i + 1) + "/" + _numValue + "] :");
                Console.WriteLine("Введiть Х:");
                double x = Convert.ToDouble(Console.ReadLine());
                Console.WriteLine("Введiть Y:");
                double y = Convert.ToDouble(Console.ReadLine());
                double[] point = new double[] { x, y };
                points1.Add(point);
            }

            Console.WriteLine("Ведіть усi точки множини B:");
            List<double[]> points2 = new List<double[]>();
            for (int i = 0; i < _numValue2; i++)
            {
                Console.WriteLine("Точка [" + (i + 1) + "/" + _numValue2 + "] :");
                Console.WriteLine("Введiть Х:");
                double x = Convert.ToDouble(Console.ReadLine());
                Console.WriteLine("Введiть Y:");
                double y = Convert.ToDouble(Console.ReadLine());
                double[] point = new double[] { x, y };
                points2.Add(point);
            }

            canvas.Children.Clear();
            Tree = new KD();
            Tree.points1 = points1;
            Tree.points2 = points2;
            Tree.FindNearest();
            foreach (var point in Tree.points1)
            {
                string text = "(" + Math.Round(point[0], 1) + ";" + Math.Round(point[1], 1) + ")";
                var textBlock = new TextBlock
                {
                    Text = text,
                    TextWrapping = TextWrapping.Wrap,
                    FontFamily = new FontFamily("Comic Sans MS, Verdana"),
                    FontSize = 8
                };

                var ellipse = new Ellipse();
                ellipse.Width = 10;
                ellipse.Height = 10;
                ellipse.Fill = Brushes.Green;
                Canvas.SetLeft(ellipse, point[0]);
                Canvas.SetTop(ellipse, 350 - point[1]);
                Canvas.SetLeft(textBlock, point[0] - 15);
                Canvas.SetTop(textBlock, 350 - point[1] + 10);
                canvas.Children.Add(ellipse);
                canvas.Children.Add(textBlock);
            }

            foreach (var point in Tree.points2)
            {
                string text = "(" + Math.Round(point[0], 1) + ";" + Math.Round(point[1], 1) + ")";
                var textBlock = new TextBlock
                {
                    Text = text,
                    TextWrapping = TextWrapping.Wrap,
                    FontFamily = new FontFamily("Comic Sans MS, Verdana"),
                    FontSize = 8
                };
                var ellipse = new Ellipse();
                ellipse.Width = 10;
                ellipse.Height = 10;
                ellipse.Fill = Brushes.Red;
                Canvas.SetLeft(ellipse, point[0]);
                Canvas.SetTop(ellipse, 350 - point[1]);
                Canvas.SetLeft(textBlock, point[0] - 15);
                Canvas.SetTop(textBlock, 350 - point[1] + 10);
                canvas.Children.Add(ellipse);
                canvas.Children.Add(textBlock);
            }

            var line = new Line();
            line.Stroke = Brushes.Aqua;

            line.X1 = Tree.firstPoint[0] + 3;
            line.Y1 = 350 - Tree.firstPoint[1] + 3;
            line.X2 = Tree.secondPoint[0] + 3;
            line.Y2 = 350 - Tree.secondPoint[1] + 3;

            line.StrokeThickness = 5;
            canvas.Children.Add(line);

            Console.WriteLine("Done!");
            ShowWindow(handle, SW_HIDE);
            res_text.Text = "Point from array A: (" + Math.Round(Tree.firstPoint[0], 1) + ";" + Math.Round(Tree.firstPoint[1], 1) + ") \n" + "Point from array B: (" + Math.Round(Tree.secondPoint[0], 1) + ";" + Math.Round(Tree.secondPoint[1], 1) + ")\n" + "Distance: " + Math.Round(Math.Sqrt(Tree.minDistance), 1) + "\n";
        }
    }


    [System.Security.SuppressUnmanagedCodeSecurity]
    public static class ConsoleManager
    {
        private const string Kernel32_DllName = "kernel32.dll";

        [DllImport(Kernel32_DllName)]
        private static extern bool AllocConsole();

        [DllImport(Kernel32_DllName)]
        private static extern bool FreeConsole();

        [DllImport(Kernel32_DllName)]
        private static extern IntPtr GetConsoleWindow();

        [DllImport(Kernel32_DllName)]
        private static extern int GetConsoleOutputCP();

        public static bool HasConsole
        {
            get { return GetConsoleWindow() != IntPtr.Zero; }
        }

        /// <summary>
        /// Creates a new console instance if the process is not attached to a console already.
        /// </summary>
        public static void Show()
        {
            //#if DEBUG
            if (!HasConsole)
            {
                AllocConsole();
                InvalidateOutAndError();
            }
            //#endif
        }

        /// <summary>
        /// If the process has a console attached to it, it will be detached and no longer visible. Writing to the System.Console is still possible, but no output will be shown.
        /// </summary>
        public static void Hide()
        {
            //#if DEBUG
            if (HasConsole)
            {
                SetOutAndErrorNull();
                FreeConsole();
            }
            //#endif
        }

        public static void Toggle()
        {
            if (HasConsole)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }

        static void InvalidateOutAndError()
        {
            Type type = typeof(System.Console);

            System.Reflection.FieldInfo _out = type.GetField("_out",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

            System.Reflection.FieldInfo _error = type.GetField("_error",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

            System.Reflection.MethodInfo _InitializeStdOutError = type.GetMethod("InitializeStdOutError",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

            Debug.Assert(_out != null);
            Debug.Assert(_error != null);

            Debug.Assert(_InitializeStdOutError != null);

            _out.SetValue(null, null);
            _error.SetValue(null, null);

            _InitializeStdOutError.Invoke(null, new object[] { true });
        }

        static void SetOutAndErrorNull()
        {
            Console.SetOut(TextWriter.Null);
            Console.SetError(TextWriter.Null);
        }
    }
}
