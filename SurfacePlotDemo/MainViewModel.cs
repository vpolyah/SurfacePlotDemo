using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;

namespace SurfacePlotDemo
{
    // http://reference.wolfram.com/mathematica/tutorial/ThreeDimensionalSurfacePlots.html

    public enum ColorCoding
    {
        /// <summary>
        /// No color coding, use coloured lights
        /// </summary>
        ByLights,

        /// <summary>
        /// Color code by gradient in y-direction using a gradient brush with white ambient light
        /// </summary>
        ByGradientY
    }

    public class MainViewModel
    {
        public double MinX { get; set; }
        public double MinY { get; set; }
        public double MaxX { get; set; }
        public double MaxY { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }

        public Func<double, double, double> Function { get; set; }
        public Point3D[,] Data { get; set; }
        public double[,] ColorValues { get; set; }

        public ColorCoding ColorCoding { get; set; }

        public Model3DGroup Lights
        {
            get
            {
                var group = new Model3DGroup();
                switch (ColorCoding)
                {
                    case ColorCoding.ByGradientY:
                        group.Children.Add(new AmbientLight(Colors.White));
                        break;
                    case ColorCoding.ByLights:
                        group.Children.Add(new AmbientLight(Colors.Gray));
                        group.Children.Add(new PointLight(Colors.Red, new Point3D(0, -1000, 0)));
                        group.Children.Add(new PointLight(Colors.Blue, new Point3D(0, 0, 1000)));
                        group.Children.Add(new PointLight(Colors.Green, new Point3D(1000, 1000, 0)));
                        break;
                }
                return group;
            }
        }

        public Brush SurfaceBrush
        {
            get
            {
                //Brush = BrushHelper.CreateGradientBrush(Colors.White, Colors.Blue);
                //Brush = GradientBrushes.RainbowStripes;
                //Brush = GradientBrushes.BlueWhiteRed;
                switch (ColorCoding)
                {
                    case ColorCoding.ByGradientY:
                        return BrushHelper.CreateGradientBrush(Colors.White, Colors.Black);
                    //case ColorCoding.ByLights:
                    //    return Brushes.White;
                }
                return null;
            }
        }


        public static double Lambda(double x, double y)
        {
            double e = 2;
            double xi = x * 4f;
            double yi = y * 4f;
            double[] xmas = { 5, 6, 30, 31, 50, 51, 60, 61, 75, 76, 80, 81, 70, 71, 40, 41 };
            double[] ymas = { 5, 6, 90, 91, 10, 11, 80, 81, 25, 26, 40, 41, 60, 61, 70, 71 };
            double[] h = new double[8];

            for (int i = 0, j = 0; i < 8; i++, j += 2)
                h[i] = hi(xi, xmas[j], xmas[j + 1], e) * hi(yi, ymas[j], ymas[j + 1], e);

            //return -h[0] + h[1] - h[2] + h[3] - h[4] + h[5] + h[6] + h[7];//Лямбда
            return h[0] - h[1] + h[2] - h[3] + h[4] - h[5] - h[6] - h[7];//Лямбда
        }
        private static double hi(double xyi, double xy0, double xy1, double e)
        {
            return (float)(((xyi - xy0 + Math.Sqrt(Math.Pow(xyi - xy0, 2) + Math.Pow(e, 2))) * (xy1 - xyi + Math.Sqrt(Math.Pow(xy1 - xyi, 2) + Math.Pow(e, 2)))) /
                (0.8 * Math.Sqrt((Math.Pow(xyi - xy0, 2) + Math.Pow(e, 2)) * (Math.Pow(xy1 - xyi, 2) + Math.Pow(e, 2)))));
        }

        public MainViewModel()
        {
            MinX = Global_variable.lower_bound_array[0];
            MaxX = Global_variable.upper_bound_array[0];
            MinY = Global_variable.lower_bound_array[1];
            MaxY = Global_variable.upper_bound_array[1];
            Rows = 250;
            Columns = Rows;

            if (Global_variable.chosen_function == "System.Windows.Controls.ComboBoxItem: Test")
            {
                Function = (x, y) => Math.Pow(Math.Sin(Math.Sqrt(Math.Abs(x - 1.3) + Math.Abs(y))) + Math.Cos(Math.Sqrt(Math.Abs(Math.Sin(x))) + Math.Sqrt(Math.Abs(Math.Sin(y)))), 4.0);//Тестовая
            }
            if (Global_variable.chosen_function == "System.Windows.Controls.ComboBoxItem: Lambda")
            {
                Function = (x, y) => Lambda(x, y);
            }
            if (Global_variable.chosen_function == "System.Windows.Controls.ComboBoxItem: Himmelblau")
            {
                Function = (x, y) => ((x * x + y - 11) * (x * x + y - 11)) + ((x + y * y - 7) * (x + y * y - 7));//Химмельблау
            }
            if (Global_variable.chosen_function == "System.Windows.Controls.ComboBoxItem: Rastrigin")
            {
                Function = (x, y) => 20 + Math.Pow(x, 2) + Math.Pow(y, 2) - 10 * (Math.Cos(2 * Math.PI * x) + Math.Cos(2 * Math.PI * y));//Растригин
            }
            if (Global_variable.chosen_function == "System.Windows.Controls.ComboBoxItem: Rosenbrok")
            {
                Function = (x, y) => Math.Pow(1 - x, 2) + 100 * Math.Pow(y - Math.Pow(x, 2), 2);//Розенброк
                
            }

            //lambda
            //Function = (x, y) => Lambda(x, y);
            //Растригин
            //Function = (x, y) => 20 + Math.Pow(x, 2) + Math.Pow(y, 2) - 10 * (Math.Cos(2 * Math.PI * x) + Math.Cos(2 * Math.PI * y));
            //Химмельблау
            //Function = (x, y) => ((x * x + y - 11) * (x * x + y - 11)) + ((x + y * y - 7) * (x + y * y - 7));
            
            //мое измен
            ColorCoding = ColorCoding.ByGradientY;
            
            
           
            UpdateModel();
        }

        private void UpdateModel()
        {
            Data = CreateDataArray(Function);
            switch (ColorCoding)
            {
                //case ColorCoding.ByGradientY:
                //    ColorValues = FindGradientY(Data);
                //    break;
                case ColorCoding.ByLights:
                    ColorValues = null;
                    break;
            }
            RaisePropertyChanged("Data");
            RaisePropertyChanged("ColorValues");
            RaisePropertyChanged("SurfaceBrush");
        }

        public Point GetPointFromIndex(int i, int j)
        {
            double x = MinX + (double)j / (Columns - 1) * (MaxX - MinX);
            double y = MinY + (double)i / (Rows - 1) * (MaxY - MinY);
            return new Point(x, y);
        }

        public Point3D[,] CreateDataArray(Func<double, double, double> f)
        {
            var data = new Point3D[Rows, Columns];
            for (int i = 0; i < Rows; i++)
                for (int j = 0; j < Columns; j++)
                {
                    var pt = GetPointFromIndex(i, j);
                    //if (f(pt.X, pt.Y) > Global_variable.boundbound/2)
                    //{
                    //    data[i, j] = new Point3D(pt.X, pt.Y, Global_variable.boundbound / 2);
                    //}
                    //else
                    //{
                        data[i, j] = new Point3D(pt.X, pt.Y, f(pt.X, pt.Y));
                    //}
                }
            return data;
        }

        // http://en.wikipedia.org/wiki/Numerical_differentiation
        public double[,] FindGradientY(Point3D[,] data)
        {
            int n = data.GetUpperBound(0) + 1;
            int m = data.GetUpperBound(0) + 1;
            var K = new double[n, m];
            for (int i = 0; i < n; i++)
                for (int j = 0; j < m; j++)
                {
                    // Finite difference approximation
                    var p10 = data[i + 1 < n ? i + 1 : i, j - 1 > 0 ? j - 1 : j];
                    var p00 = data[i - 1 > 0 ? i - 1 : i, j - 1 > 0 ? j - 1 : j];
                    var p11 = data[i + 1 < n ? i + 1 : i, j + 1 < m ? j + 1 : j];
                    var p01 = data[i - 1 > 0 ? i - 1 : i, j + 1 < m ? j + 1 : j];

                    double dx = p01.X - p10.X;
                    double dz = p01.Z - p10.Z;
                    double Fx = dz / dx;

                    //double dy = p10.Y - p00.Y;
                    //double dz = p10.Z - p00.Z;

                    //K[i, j] = dz / dy;
                    K[i, j] = Fx;
                }
            return K;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string property)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(property));
            }
        }

    }
}