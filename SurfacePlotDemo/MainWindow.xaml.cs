using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Threading;
using System.Diagnostics;
using Xceed.Wpf.Toolkit;
using System.IO;
using System.Collections.ObjectModel;

namespace SurfacePlotDemo
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
   
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            this.InitializeComponent();
           
            InitializeComponent();

            

            //отключение недостроенных
            ReLocalize.IsEnabled = false;
            SearchResults.IsEnabled = false;
            LocalizeSearchResults.IsEnabled = false;
            LocalizeFinding.IsEnabled = false;
            ParamsTabControl.IsEnabled = false;
            Button1.IsEnabled = false;
            OstanovComboBox.IsEnabled = false;
            CrossingoverComboBox.IsEnabled = false;
            MutationComboBox.IsEnabled = false;
            ParentsFormComboBox.IsEnabled = false;
            SelectionComboBox.IsEnabled = false;
            AccuracyComboBox.IsEnabled = false;
            ExtremsVariantComboBox.IsEnabled = false;
           
            //for (int i=0;i<Global_variable.params_number-1;i++)
            //{
            //    TabItem item=new TabItem();
            //    string num = Convert.ToString(i + 2);
            //    item.Header = num + "param";
            //    tabcontrol1.Items.Add(item);
            //}
            //DispatcherTimer timer1 = new DispatcherTimer();
            //timer1.Interval = TimeSpan.FromSeconds(0.1);
            //timer1.Tick += timer_Tick1;
            //timer1.Start();

             

            
        }
        
        //public void timer_Tick1(object sender, EventArgs e)
        //{
        //    string chosen_func=Combo1.SelectedItem.ToString();
        //    if (chosen_func=="System.Windows.Controls.ComboBoxItem: Rastrigin")
        //    {
        //        funcfunc = true;
        //        if (funcfunc == true)
        //        {
        //            DecimalUpDown1min.Value = Convert.ToDecimal(-2.00000);
        //            DecimalUpDown1max.Value = Convert.ToDecimal(2.00000);
        //        }
        //    }

        //    if (chosen_func == "System.Windows.Controls.ComboBoxItem: Himmelblau")
        //    {
        //        DecimalUpDown1min.Value = Convert.ToDecimal(-4.00000);
        //        DecimalUpDown1max.Value = Convert.ToDecimal(4.00000);
        //    }

        //    if (chosen_func == "System.Windows.Controls.ComboBoxItem: Lambda")
        //    {
        //        DecimalUpDown1min.Value = Convert.ToDecimal(-400.00000);
        //        DecimalUpDown1max.Value = Convert.ToDecimal(400.00000);
        //    }

        //    if (chosen_func == "System.Windows.Controls.ComboBoxItem: Rosenbrok")
        //    {
        //        DecimalUpDown1min.Value = Convert.ToDecimal(-2.00000);
        //        DecimalUpDown1max.Value = Convert.ToDecimal(2.00000);
        //    }
        //}
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //SurfacePlotVisual3D sf = new SurfacePlotVisual3D();
            //sf.ClearSurface();

            //Gd.ItemsSource = null;
            Global_variable.UtochnenNumb = 1;
            for (int tb=1; tb<tabcontrol2.Items.Count;tb++)
            {
                tabcontrol2.Items.Remove(tabcontrol2.Items[tb]);
            }


            ReLocalize.IsEnabled = false;
            Global_variable.sbrospbar = false;
            Global_variable.record = false;


             Global_variable.chosen_function = Combo1.SelectedItem.ToString();

             Global_variable.lower_bound_array[0] = Convert.ToDouble(DecimalUpDown1min.Value);
             Global_variable.lower_bound_array[1] = Convert.ToDouble(DecimalUpDown2min.Value);
             Global_variable.upper_bound_array[0] = Convert.ToDouble(DecimalUpDown1max.Value);
             Global_variable.upper_bound_array[1] = Convert.ToDouble(DecimalUpDown2max.Value);
             Global_variable.genration_count = Convert.ToInt32(GenerationDecimalUpDown.Value);
             Global_variable.population_count = Convert.ToInt32(PopulationDecimalUpDown.Value);
             Global_variable.crossingover_probability = Convert.ToInt32(CrossingAccuracyDecimalUpDown.Value);
             Global_variable.mutation_probability = Convert.ToInt32(MutationAccuracyDecimalUpDown.Value);


            if (ExtremsVariantComboBox.SelectedIndex==0)
            {
                Global_variable.find_max_or_min = 1;
            }
            if (ExtremsVariantComboBox.SelectedIndex == 1)
            {
                Global_variable.find_max_or_min = 2;
            }
            if (ExtremsVariantComboBox.SelectedIndex == 2)
            {
                Global_variable.find_max_or_min = 3;
            }

             pbar.Maximum = Global_variable.genration_count-1;



             if (Global_variable.kol_elem > 3)
             {
                 Global_variable.surface = false;
                 this.DataContext = new MainViewModel();
             }

             
            
            MethodRun run = new MethodRun();
            run.ProcessChanged += run_ProcessChanged;

            //Action action = () => { pbar_cluster.Value = Global_variable.clustering_iteration; };
            //Dispatcher.Invoke(action);

            Thread thread = new Thread(delegate() { run.Init(); GridRecord(Gd); Dispatcher.Invoke(new Action(() => { SearchResults.IsEnabled = true; LocalizeFinding.IsEnabled = true; })); });

            thread.Start();

            Thread tt = new Thread(delegate() { Dispatcher.Invoke(new Action(() => { pbar_cluster.Value = 0; })); while (true) { run_ClusteringProcess(); if (Global_variable.sbrospbar == true) { break; } } });
            tt.Start();

            Thread LocalizeThread = new Thread(delegate() { LocalizeProcess(thread); tt.Abort(); });
            LocalizeThread.Start();

            new Thread(delegate() { run_ProcessStoped(LocalizeThread); tt.Abort(); }).Start();
            Global_variable.record = false;
            Global_variable.ti = -1;
            
            Global_variable.clustering_iteration = 0;
            Global_variable.for_clust = false;
            
        }


        


        
        private void run_ClusteringProcess()
        {

            Action action = () => { if (Global_variable.for_clust==true) { pbar_cluster.Maximum = Global_variable.individ_maks; } pbar_cluster.Value = Global_variable.clustering_iteration; };
            Dispatcher.Invoke(action);
        }

        private void run_ProcessChanged(int progress)
        {
            Action action = () => { pbar.Value = progress; Button1.IsEnabled = false; pbar_cluster.Value = Global_variable.clustering_iteration; };
                     
            Dispatcher.Invoke(action);
            
        }


        private void LocalizeProcess(Thread th)
        {

           
            th.Join();
            //double border = 0;
            int generation = 0;
            int population = 0;
            int crossingover = 0;
            int mutation = 0;
            Dispatcher.Invoke(new Action(() => {
                //border = Convert.ToDouble(BorderDecimalUpDown.Value);
                generation = Convert.ToInt32(LocalizeGenerationDecimalUpDown.Value);
                population = Convert.ToInt32(LocalizePopulationDecimalUpDown.Value);
                crossingover = Convert.ToInt32(LocalizePopulationCrossingAccuracyDecimalUpDown.Value);
                mutation = Convert.ToInt32(LocalizeMutationAccuracyDecimalUpDown.Value);
            }));
            
          
                                //MethodRun run = new MethodRun();
                                //run.LocalizeSearch();
                            

            if (Global_variable.localizeSearch == true)
            {
                Dispatcher.Invoke(new Action(() => {
                LocalizeSearchResults.IsEnabled = true;
                }));

                Global_variable.KudaPisatCluster = true;
                for (int cl = 0; cl < Global_variable.ClusterGlobalList.Count; cl++)
                {

                    Global_variable.lower_bound_array[0] = Global_variable.ClusterGlobalList[cl].ClusterIndiividual[0].decimal_value[0] - Global_variable.ClusterGlobalList[cl].MiddleValueX;
                    Global_variable.lower_bound_array[1] = Global_variable.ClusterGlobalList[cl].ClusterIndiividual[0].decimal_value[1] - Global_variable.ClusterGlobalList[cl].MiddleValueY;
                        Global_variable.upper_bound_array[0] = Global_variable.ClusterGlobalList[cl].ClusterIndiividual[0].decimal_value[0] + Global_variable.ClusterGlobalList[cl].MiddleValueX;
                        Global_variable.upper_bound_array[1] = Global_variable.ClusterGlobalList[cl].ClusterIndiividual[0].decimal_value[1] + Global_variable.ClusterGlobalList[cl].MiddleValueY;

                        Global_variable.genration_count = generation;
                        Global_variable.population_count = population;
                        Global_variable.crossingover_probability = crossingover;
                        Global_variable.mutation_probability = mutation;

                        MethodRun run = new MethodRun();
                        run.LocalizeSearch();

                        GridRecord(LocalizeGrid);
                        //Global_variable.LocalizeClusterGlobalList.Clear();
                }
                //GridRecord();
                
            }

            Dispatcher.Invoke(new Action(() => {
            ReLocalize.IsEnabled = true;
            }));

            Global_variable.KudaPisatCluster = false;
        }


        private void LocalizeProcess()
        {


            //th.Join();
            //double border = 0;
            int generation = 0;
            int population = 0;
            int crossingover = 0;
            int mutation = 0;
            Dispatcher.Invoke(new Action(() =>
            {
                //border = Convert.ToDouble(BorderDecimalUpDown.Value);
                generation = Convert.ToInt32(LocalizeGenerationDecimalUpDown.Value);
                population = Convert.ToInt32(LocalizePopulationDecimalUpDown.Value);
                crossingover = Convert.ToInt32(LocalizePopulationCrossingAccuracyDecimalUpDown.Value);
                mutation = Convert.ToInt32(LocalizeMutationAccuracyDecimalUpDown.Value);
            }));


            //MethodRun run = new MethodRun();
            //run.LocalizeSearch();


            //if (Global_variable.localizeSearch == true)
            //{
                Dispatcher.Invoke(new Action(() =>
                {
                    LocalizeSearchResults.IsEnabled = true;
                }));

                Global_variable.KudaPisatCluster = true;
                for (int cl = 0; cl < Global_variable.ClusterGlobalList.Count; cl++)
                {

                    Global_variable.lower_bound_array[0] = Global_variable.ClusterGlobalList[cl].ClusterIndiividual[0].decimal_value[0] - Global_variable.ClusterGlobalList[cl].MiddleValueX;
                    Global_variable.lower_bound_array[1] = Global_variable.ClusterGlobalList[cl].ClusterIndiividual[0].decimal_value[1] - Global_variable.ClusterGlobalList[cl].MiddleValueY;
                    Global_variable.upper_bound_array[0] = Global_variable.ClusterGlobalList[cl].ClusterIndiividual[0].decimal_value[0] + Global_variable.ClusterGlobalList[cl].MiddleValueX;
                    Global_variable.upper_bound_array[1] = Global_variable.ClusterGlobalList[cl].ClusterIndiividual[0].decimal_value[1] + Global_variable.ClusterGlobalList[cl].MiddleValueY;

                    Global_variable.genration_count = generation;
                    Global_variable.population_count = population;
                    Global_variable.crossingover_probability = crossingover;
                    Global_variable.mutation_probability = mutation;

                    MethodRun run = new MethodRun();
                    run.LocalizeSearch();

                    GridRecord(LocDatGrid);
                    //Global_variable.LocalizeClusterGlobalList.Clear();
                }
                //GridRecord();

            //}

            Dispatcher.Invoke(new Action(() =>
            {
                ReLocalize.IsEnabled = true;
            }));

            Global_variable.KudaPisatCluster = false;
        }


        private void run_ProcessStoped(Thread th)
        {
            th.Join();
            Action action = () => {  pbar.Value = 0; pbar_cluster.Value = 0; Button1.IsEnabled = true; };
            Dispatcher.Invoke(action);
        }

       

         
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (System.Windows.MessageBox.Show("Вы уверены, что хотите закрыть окно?",
                "Закрытие окна",
                MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                // воспрепятствовать закрытию окна
                e.Cancel = true;
            }
            else
            {
                // окно будет закрыто
                File.Delete("StartPop.txt");
                File.Delete("Mytext.txt");
                File.Delete("AllPop.txt");
                File.Delete("ClusteringResult.txt");
                File.Delete("Extrems.txt");
                Process.GetCurrentProcess().Kill();
            }
        }

        private void Combo1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            string chosen_func = "";


            Dispatcher.Invoke(new Action(() =>
                    {
                        chosen_func = Combo1.SelectedItem.ToString();
                        Button1.IsEnabled = true;
                        ParamsTabControl.IsEnabled = true;
                        ExtremsVariantComboBox.IsEnabled = true;
                    }));

            double DecimalUpDown11min = 0;
            double DecimalUpDown22min = 0;
            double DecimalUpDown11max = 0;
            double DecimalUpDown22max = 0;

            if (chosen_func == "System.Windows.Controls.ComboBoxItem: Test")
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    DecimalUpDown1min.Value = -10.00000m;
                    DecimalUpDown1max.Value = 10.00000m;
                    DecimalUpDown2min.Value = -10.00000m;
                    DecimalUpDown2max.Value = 10.00000m;
                    Global_variable.chosen_function = Combo1.SelectedItem.ToString();
                    DecimalUpDown11min = Convert.ToDouble(DecimalUpDown1min.Value); ;
                    DecimalUpDown22min = Convert.ToDouble(DecimalUpDown2min.Value);
                    DecimalUpDown11max = Convert.ToDouble(DecimalUpDown1max.Value);
                    DecimalUpDown22max = Convert.ToDouble(DecimalUpDown2max.Value);
                }));

                Global_variable.lower_bound_array[0] = DecimalUpDown11min;
                Global_variable.lower_bound_array[1] = DecimalUpDown22min;
                Global_variable.upper_bound_array[0] = DecimalUpDown11max;
                Global_variable.upper_bound_array[1] = DecimalUpDown22max;

                if (Global_variable.kol_elem > 0)
                {
                    Global_variable.surface = false;
                }

                MainViewModel MVM = new MainViewModel();
                Dispatcher.Invoke(new Action(() =>
                {
                    this.DataContext = MVM;
                }));
            }
           
                if (chosen_func == "System.Windows.Controls.ComboBoxItem: Rastrigin")
                {

                    Dispatcher.Invoke(new Action(() =>
                    {
                        DecimalUpDown1min.Value = -2.00000m;
                        DecimalUpDown1max.Value = 2.00000m;
                        DecimalUpDown2min.Value = -2.00000m;
                        DecimalUpDown2max.Value = 2.00000m;
                        Global_variable.chosen_function = Combo1.SelectedItem.ToString();
                        DecimalUpDown11min = Convert.ToDouble(DecimalUpDown1min.Value); ;
                        DecimalUpDown22min = Convert.ToDouble(DecimalUpDown2min.Value);
                        DecimalUpDown11max = Convert.ToDouble(DecimalUpDown1max.Value);
                        DecimalUpDown22max = Convert.ToDouble(DecimalUpDown2max.Value);
                    }));

                    Global_variable.lower_bound_array[0] = DecimalUpDown11min;
                    Global_variable.lower_bound_array[1] = DecimalUpDown22min;
                    Global_variable.upper_bound_array[0] = DecimalUpDown11max;
                    Global_variable.upper_bound_array[1] = DecimalUpDown22max;

                    if (Global_variable.kol_elem > 0)
                    {
                        Global_variable.surface = false;
                    }

                    MainViewModel MVM = new MainViewModel();
                    Dispatcher.Invoke(new Action(() =>
                    {
                    this.DataContext = MVM;
                    }));
                }

                if (chosen_func == "System.Windows.Controls.ComboBoxItem: Himmelblau")
                {
                    DecimalUpDown1min.Value = -4.00000m;
                    DecimalUpDown1max.Value = 4.00000m;
                    DecimalUpDown2min.Value = -4.00000m;
                    DecimalUpDown2max.Value = 4.00000m;

                    Global_variable.chosen_function = Combo1.SelectedItem.ToString();

                    Global_variable.lower_bound_array[0] = Convert.ToDouble(DecimalUpDown1min.Value);
                    Global_variable.lower_bound_array[1] = Convert.ToDouble(DecimalUpDown2min.Value);
                    Global_variable.upper_bound_array[0] = Convert.ToDouble(DecimalUpDown1max.Value);
                    Global_variable.upper_bound_array[1] = Convert.ToDouble(DecimalUpDown2max.Value);

                    if (Global_variable.kol_elem > 0)
                    {
                        Global_variable.surface = false;
                    }

                    this.DataContext = new MainViewModel();
                }

                if (chosen_func == "System.Windows.Controls.ComboBoxItem: Lambda")
                {
                    //ViewPort.Camera.LookDirection.=(0, 0, 5800);
                    DecimalUpDown1min.Value = -40.00000m;
                    DecimalUpDown1max.Value = 40.00000m;
                    DecimalUpDown2min.Value = -40.00000m;
                    DecimalUpDown2max.Value = 40.00000m;

                    Global_variable.chosen_function = Combo1.SelectedItem.ToString();

                    Global_variable.lower_bound_array[0] = Convert.ToDouble(DecimalUpDown1min.Value);
                    Global_variable.lower_bound_array[1] = Convert.ToDouble(DecimalUpDown2min.Value);
                    Global_variable.upper_bound_array[0] = Convert.ToDouble(DecimalUpDown1max.Value);
                    Global_variable.upper_bound_array[1] = Convert.ToDouble(DecimalUpDown2max.Value);

                    if (Global_variable.kol_elem > 0)
                    {
                        Global_variable.surface = false;
                    }

                    this.DataContext = new MainViewModel();
                }

                if (chosen_func == "System.Windows.Controls.ComboBoxItem: Rosenbrok")
                {
                    DecimalUpDown1min.Value = -2.00000m;
                    DecimalUpDown1max.Value = 2.00000m;
                    DecimalUpDown2min.Value = -2.00000m;
                    DecimalUpDown2max.Value = 2.00000m;

                    Global_variable.chosen_function = Combo1.SelectedItem.ToString();

                    Global_variable.lower_bound_array[0] = Convert.ToDouble(DecimalUpDown1min.Value);
                    Global_variable.lower_bound_array[1] = Convert.ToDouble(DecimalUpDown2min.Value);
                    Global_variable.upper_bound_array[0] = Convert.ToDouble(DecimalUpDown1max.Value);
                    Global_variable.upper_bound_array[1] = Convert.ToDouble(DecimalUpDown2max.Value);

                    if (Global_variable.kol_elem > 0)
                    {
                        Global_variable.surface = false;
                    }

                    this.DataContext = new MainViewModel();
                }
            
        }

        //Работа с таблицей
         public class DataItem
        {
            public double Номер_кластера { get; set; }
           // public double IndividualNumb { get; set; }
            public double Первый_параметр_функции { get; set; }
            public double Второй_параметр_функции { get; set; }
            public double Значение_целевой_функции { get; set; }
        }


         ObservableCollection<DataItem> collection = null;
         public void GridRecord(DataGrid temp_grid)
         {
             if (Global_variable.KudaPisatCluster == false)
             {
                 collection = null;
                 if (collection == null)
                 {
                     collection = new ObservableCollection<DataItem>();
                     Dispatcher.Invoke(new Action(() => { Gd.ItemsSource = collection; }));
                     //Gd.Columns[0].Header = "Интервал";
                     //Gd.Columns[0].Width = 60;
                     //Gd.Columns[1].Header = "Нижн.граница";
                     //Gd.Columns[2].Header = "Верх.граница";
                 }
                 Dispatcher.Invoke(new Action(() => { Gd.CanUserAddRows = false; }));
                 //Gd.CanUserAddRows = false;

                 for (int i = 0; i < Global_variable.ClusterGlobalList.Count; i++)
                 {

                     //for (int j = 0; j < Global_variable.ClusterGlobalList[i].ClusterIndiividual.Count - 1; j++)
                     //{
                     Dispatcher.Invoke(new Action(() =>
                     {
                         collection.Add(
                             new DataItem()
                             {
                                 Номер_кластера = i + 1,
                                 //IndividualNumb = j + 1,
                                 Первый_параметр_функции = Global_variable.ClusterGlobalList[i].ClusterIndiividual[0].decimal_value[0],
                                 Второй_параметр_функции = Global_variable.ClusterGlobalList[i].ClusterIndiividual[0].decimal_value[1],
                                 Значение_целевой_функции = Global_variable.ClusterGlobalList[i].ClusterIndiividual[0].result_function
                             }
                             );
                     }));

                     //}
                 }
             }

             if (Global_variable.KudaPisatCluster == true)
             {
                 collection = null;
                 if (collection == null)
                 {
                     collection = new ObservableCollection<DataItem>();
                     Dispatcher.Invoke(new Action(() => { temp_grid.ItemsSource = collection; }));
                     ////LocalizeGrid.ItemsSource = collection;
                     //Gd.Columns[0].Header = "Интервал";
                     ////Gd.Columns[0].Width = 60;
                     //Gd.Columns[1].Header = "Нижн.граница";
                     //Gd.Columns[2].Header = "Верх.граница";
                     //Gd.Columns[3].Header = "Верх.граница";
                 }
                 Dispatcher.Invoke(new Action(() => { temp_grid.CanUserAddRows = false; }));
                 //LocalizeGrid.CanUserAddRows = false;

                 for (int i = 0; i < Global_variable.LocalizeClusterGlobalList.Count; i++)
                 {

                     //for (int j = 0; j < Global_variable.LocalizeClusterGlobalList[i].ClusterIndiividual.Count - 1; j++)
                     //{
                     Dispatcher.Invoke(new Action(() =>
                     {
                         collection.Add(
                             new DataItem()
                             {
                                 Номер_кластера = i + 1,
                                 //IndividualNumb = j + 1,
                                 Первый_параметр_функции = Global_variable.LocalizeClusterGlobalList[i].ClusterIndiividual[0].decimal_value[0],
                                 Второй_параметр_функции = Global_variable.LocalizeClusterGlobalList[i].ClusterIndiividual[0].decimal_value[1],
                                 Значение_целевой_функции = Global_variable.LocalizeClusterGlobalList[i].ClusterIndiividual[0].result_function
                             }
                             );
                     }));

                     //}
                 }
             }
             
         }

         private void CheckBox_Checked(object sender, RoutedEventArgs e)
         {          
                 LocalizeFinding.IsEnabled = true;
                 Global_variable.localizeSearch = true;            
         }
         private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
         {             
                 LocalizeFinding.IsEnabled = false;
                 Global_variable.localizeSearch = false;
         }

         public DataGrid LocDatGrid = null;
         private void ReLocalize_Click(object sender, RoutedEventArgs e)
         {
             Global_variable.UtochnenNumb++;
             Global_variable.ClusterGlobalList.Clear();
             Global_variable.ClusterGlobalList = new List<Cluster>(Global_variable.LocalizeClusterGlobalList);
             Global_variable.LocalizeClusterGlobalList.Clear();
             
             //DataGrid LocDatGrid = null;
             
             Dispatcher.Invoke(new Action(() =>{

                 LocDatGrid = new DataGrid();
                 LocDatGrid.HorizontalAlignment = HorizontalAlignment.Stretch;
                 LocDatGrid.VerticalAlignment = VerticalAlignment.Stretch;

                 Grid Grd = new Grid();
                 Grd.HorizontalAlignment = HorizontalAlignment.Stretch;
                 Grd.VerticalAlignment = VerticalAlignment.Stretch;

                 Grd.Children.Add(LocDatGrid);
                 TabItem item = new TabItem();
                 string num = Convert.ToString(Global_variable.UtochnenNumb);
                 item.Header = "Уточнение #" + num;
                 tabcontrol2.Items.Add(item);
                 item.Content = Grd;

                 LocalizeSearchResults.IsEnabled = true;
            
            }));

             Thread NewLocalizeThread = new Thread(delegate()
             {
                 LocalizeProcess();
                 //Global_variable.KudaPisatCluster = true;
                 //GridRecord(LocDatGrid);
                 //Global_variable.KudaPisatCluster = false;
             });
             NewLocalizeThread.Start();
         }






        
       

   }

    
    
}



