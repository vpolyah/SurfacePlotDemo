namespace SurfacePlotDemo
{
    using System.Windows;
    using System;
    using System.Collections.Generic;

    using System.Windows.Media;
    using System.Windows.Media.Media3D;
    using HelixToolkit.Wpf;
    using System.Windows.Threading;

    //using ExampleBrowser;

    ///// <summary>
    ///// Interaction logic for MainWindow.xaml
    ///// </summary>
    //[Example(null, "Plotting a surface in 3D.")]
    public partial class Window1 : Window
    {

        public Window1()
        {
            //Window1 ww = new Window1();
            //ww.Show();
            this.InitializeComponent();
            this.DataContext = new MainViewModel();

            double screenHeight = SystemParameters.FullPrimaryScreenHeight;
        double screenWidth = SystemParameters.FullPrimaryScreenWidth;
 
 
            //this.Top = (screenHeight - this.Height) / 0x00000002;
            //this.Left = (screenWidth - this.Width) / 0x00000001; 

            //this.

        }

        private void FileExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {

            //if (Global.a == false)
            //{
            //    Global.a = true;

            //}
            //else if (Global.a == true)
            //{
            //    Global.a = false;
            //}
        }
    }
}