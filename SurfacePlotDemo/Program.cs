using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
//using System.Windows.Forms;
using System.IO;
using System.Web.UI.DataVisualization.Charting;
using System.Windows.Threading;


namespace SurfacePlotDemo
{

    public class Individual
    {
        
        public List<string> string_value { get; set; }
        public List<double> decimal_value { get; set; }
        public double radius { get; set; }
        public double result_function { get; set; }
        public double vector_value { get; set; }
        public int cluster_numb { get; set; }
        public Individual()
        {
            //double[] radius = new double[2];
            List<string> string_value = new List<string>();
            List<double> decimal_value = new List<double>();
        }
    }

    public class Cluster // класс кластера
    {
        public List<Individual> ClusterIndiividual { get; set; }
        public double MiddleValueX { get; set; }
        public double MiddleValueY { get; set; }
        public Cluster()
        {
            ClusterIndiividual = new List<Individual>();
        }

    }


    //класс формирования популяции
    public class FormPopulation
    {
        //функция подсчета колличества разрядов в хромосомых (зависит от заданных границ)
        public static void AccuracyCalc()  
        {
            //for (int i = 0; i < Global_variable.params_number; i++)
            //{
            //    Global_variable.upper_bound_array[i] = Global_variable.boundbound;
            //    Global_variable.lower_bound_array[i] = -(Global_variable.boundbound);
            //}
            double number_degree = Math.Pow(10, Global_variable.accuracy);
            for (int i = 0; i < Global_variable.params_number; i++)
            {
                double temp = (Global_variable.upper_bound_array[i] - Global_variable.lower_bound_array[i]) * number_degree;
                Global_variable.bit_count_array[i] = Math.Log(Math.Abs(temp), 2);
                Global_variable.bit_count_array[i] = Math.Floor(Global_variable.bit_count_array[i]);
                Global_variable.bit_count_array[i]++;
            }
        }

        //функция перевода двоичного значения особи в промежуточное вещественное
        static public double ConvertToTempDouble(string Mas)
        {
            double Val = 0;
            //отрицательное число, если первый символ "1"
            if (Mas[0] == '1')
            {
                string temp = "";
                for (int k = 1; k < Mas.Length; k++)
                {
                    temp = temp + Mas[k];
                    Val = Convert.ToInt64(temp, 2);
                    Val = Val - Val * 2;
                }
            }
            //положительное число, если первый символ "0"
            if (Mas[0] == '0')
            {
                string temp = "";
                for (int k = 1; k < Mas.Length; k++)
                {
                    temp = temp + Mas[k];
                    Val = Convert.ToInt64(temp, 2);
                }
            }
            return Val;
        }

        //функция перевода промежуточного вещественного значения особи в конечное вещественное
        static public double ConvertTempDoubleToFinalDouble(double temp, double bit, double low, double hight)
        {
            double value;
            double a = temp;
            double stepen = Math.Pow(2, bit - 1);             //отнимать 1 от бита не надо, но уж как есть
            value = low + a * ((hight - low) / (stepen - 1)); //отнимать надо от степени, но было отмечено, что это ни на что особо не влияет
            return value;
        }
    }

    public class CreatePopulation
    {
        public static List<Individual> CreatedPopulation = new List<Individual>(); //создание списка особей
        public static List<Individual> SelectededPopulation = new List<Individual>(); //список особей, входящих в область поиска
        public void GenericPopulation(int size, List<Individual> UsingPopulation)
        {
            FormPopulation.AccuracyCalc();
            string[] StringMas = new string[Global_variable.params_number];
            double[] DoubleMas = new double[Global_variable.params_number];
            Random rnd = new Random();
            for (int i = 0; i < size; i++)
            {
                for (int k = 0; k < Global_variable.params_number; k++)
                {
                    StringMas[k] = "";
                    for (int j = 0; j < Global_variable.bit_count_array[k]; j++)
                    {
                        Char[] pwdChars = new Char[2] { '0', '1' };
                        StringMas[k] = StringMas[k] + pwdChars[rnd.Next(0, 2)];
                    }
                    double temp_val = FormPopulation.ConvertToTempDouble(StringMas[k]);
                    double val = FormPopulation.ConvertTempDoubleToFinalDouble(temp_val, Global_variable.bit_count_array[k], Global_variable.lower_bound_array[k], Global_variable.upper_bound_array[k]);
                    DoubleMas[k] = Math.Round(val, Global_variable.accuracy);
                }
                UsingPopulation.Add(new Individual { string_value = StringMas.ToList(), decimal_value = DoubleMas.ToList() });
                Array.Clear(StringMas, 0, StringMas.Length);
                Array.Clear(DoubleMas, 0, DoubleMas.Length);
            }
        }

        //Проверка попадния в область поиска
        public void SelectionPopulation(List<Individual> UsingPopulation)
        {
            SelectededPopulation.Clear();
            int kol = UsingPopulation.Count - 1;
            for (int i = 0; i < kol; i++)
            {
                int jkol = 0;
                for (int j = 0; j < Global_variable.params_number; j++)
                {
                    if (UsingPopulation[i].decimal_value[j] < Global_variable.lower_bound_array[j] || UsingPopulation[i].decimal_value[j] > Global_variable.upper_bound_array[j])
                    {
                        UsingPopulation.Remove(UsingPopulation[i]);
                        kol--;
                        break;
                    }
                    else
                    {
                        jkol++;
                    }
                    if (jkol == Global_variable.params_number)
                    {
                        if (SelectededPopulation.Count == Global_variable.population_count)
                            break;
                        else
                        {
                            SelectededPopulation.Add(UsingPopulation[i]);
                        }
                        jkol = 0;
                    }
                }
            }
            while (SelectededPopulation.Count < Global_variable.population_count)
            {
                CreatePopulation i = new CreatePopulation();
                i.GenericPopulation(Global_variable.population_count, CreatePopulation.CreatedPopulation);
                i.SelectionPopulation(CreatePopulation.CreatedPopulation);
                CreatePopulation.CreatedPopulation.Clear();
            }
        }

        public void PrintStartPopulation(List<Individual> UsingPopulation)
        {
            StreamWriter sw;
            sw = File.AppendText("StartPop.txt");
            int kol = 0;
            sw.WriteLine("КУ-КУ ЕПТА %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%");
            foreach (Individual k in UsingPopulation)
            {
                sw.WriteLine("Generation {0}", kol++);
                for (int j = 0; j < Global_variable.params_number; j++)
                {
                    sw.WriteLine("Param #{0}={1} & {2}", j, k.string_value[j], k.decimal_value[j]);
                    sw.WriteLine("");
                }
            }
            sw.Close();
        }
    }

    public class EvolutionaryGeneticAlg
    {
        public static List<Individual> TempPopulation = new List<Individual>(); //1-ый вспомогательный список для хранения популяций во время ЭГА
        public static List<Individual> TempPopulation1 = new List<Individual>();//2-ой вспомогательный список для хранения популяций во время ЭГА

        public static List<Individual> AllPopulation = new List<Individual>();//Список со всеми особями (всех поколений)

        public static string[] ChildList_1 = new string[Global_variable.params_number];
        public static string[] ChildList_2 = new string[Global_variable.params_number];


        public void Crossingover()
        {
            StreamWriter sw;
            Random rnd = new Random();
            for (int a1 = 0; a1 < Global_variable.population_count; a1++)
            {
                int veroyatnost_cross = rnd.Next(0, 100);
                if (veroyatnost_cross < Global_variable.crossingover_probability)
                {
                    int a2 = rnd.Next(1, Global_variable.population_count);
                    while (a1 == a2)
                    {
                        a2 = rnd.Next(1, Global_variable.population_count);
                    }
                    int rand1;
                    if (CreatePopulation.SelectededPopulation[a1].string_value.Count == 0 || CreatePopulation.SelectededPopulation[a2].string_value.Count == 0)
                    {
                        sw = File.AppendText("Mytext.txt");
                        sw.WriteLine("The end of evolution");
                        sw.Close();
                    }
                    //классический одноточечный кроссинговер
                    for (int i = 0; i < Global_variable.params_number; i++)
                    {
                        rand1 = rnd.Next(1, CreatePopulation.SelectededPopulation[a1].string_value[i].Length);
                        string allel_part1 = CreatePopulation.SelectededPopulation[a1].string_value[i].Substring(0, rand1);
                        string allel_part2 = CreatePopulation.SelectededPopulation[a2].string_value[i].Substring(rand1);
                        ChildList_1[i] = allel_part1 + allel_part2;
                        allel_part1 = CreatePopulation.SelectededPopulation[a1].string_value[i].Substring(rand1);
                        allel_part2 = CreatePopulation.SelectededPopulation[a2].string_value[i].Substring(0, rand1);
                        ChildList_2[i] = allel_part1 + allel_part2;
                    }
                    int veroyatnost_mut = rnd.Next(0, 100);
                    if (veroyatnost_mut < Global_variable.mutation_probability)
                    {
                        Mutation();
                    }
                    else
                    {
                        TempPopulation.Add(new Individual { string_value = ChildList_1.ToList() });
                        TempPopulation.Add(new Individual { string_value = ChildList_2.ToList() });
                    }
                }
            }
        }

        public void Mutation()
        {
            Random rnd = new Random();
            for (int i = 0; i < Global_variable.params_number; i++)
            {
                int rand1 = rnd.Next(1, ChildList_1[i].Length);
                if (ChildList_1[i][rand1 - 1] == '0')
                {
                    ChildList_1[i] = ChildList_1[i].Remove(rand1 - 1, 1);
                    ChildList_1[i] = ChildList_1[i].Insert(rand1 - 1, "1");
                }
                else
                {
                    ChildList_1[i] = ChildList_1[i].Remove(rand1 - 1, 1);
                    ChildList_1[i] = ChildList_1[i].Insert(rand1 - 1, "0");
                }
                int rand2 = rnd.Next(1, ChildList_2[i].Length);
                if (ChildList_2[i][rand2 - 1] == '0')
                {
                    ChildList_2[i] = ChildList_2[i].Remove(rand2 - 1, 1);
                    ChildList_2[i] = ChildList_2[i].Insert(rand2 - 1, "1");
                }
                else
                {
                    ChildList_2[i] = ChildList_2[i].Remove(rand2 - 1, 1);
                    ChildList_2[i] = ChildList_2[i].Insert(rand2 - 1, "0");
                }
            }
            TempPopulation.Add(new Individual { string_value = ChildList_1.ToList() });
            TempPopulation.Add(new Individual { string_value = ChildList_2.ToList() });
        }

        public void NewPopulationSynthesis()
        {
            TempPopulation1.Clear();
            double[] val = new double[Global_variable.params_number];
            foreach (Individual i in TempPopulation)
            {
                for (int j = 0; j < Global_variable.params_number; j++)
                {
                    double temp_val = FormPopulation.ConvertToTempDouble(i.string_value[j]);
                    val[j] = FormPopulation.ConvertTempDoubleToFinalDouble(temp_val, Global_variable.bit_count_array[j], Global_variable.lower_bound_array[j], Global_variable.upper_bound_array[j]);
                    val[j] = Math.Round(val[j], Global_variable.accuracy);
                }
                TempPopulation1.Add(new Individual { string_value = i.string_value, decimal_value = val.ToList() });
            }
            TempPopulation.Clear();
            int c = TempPopulation1.Count();
        }

        public void AllPopulationListAdd(List<Individual> UsingPopulation)
        {
            foreach (Individual ind in UsingPopulation)
            {
                AllPopulation.Add(ind);
            }
        }
    }

    public class FunctionDeal
    {
        //lambda-function
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

        static public double Function(List<double> UsingVariable)
        {
            double x = UsingVariable[0];
            double y = UsingVariable[1];
            //здесь добавлять новые переменные, если больше 2-х параметров
            double f;
            if (Global_variable.chosen_function == "System.Windows.Controls.ComboBoxItem: Test")
            {
                f = Math.Pow(Math.Sin(Math.Sqrt(Math.Abs(x - 1.3) + Math.Abs(y))) + Math.Cos(Math.Sqrt(Math.Abs(Math.Sin(x))) + Math.Sqrt(Math.Abs(Math.Sin(y)))), 4.0);
                return f = Math.Round(f, Global_variable.accuracy);
            }
            if (Global_variable.chosen_function == "System.Windows.Controls.ComboBoxItem: Lambda")
            {
                f = Lambda(x,y);//Лямбда
                return f = Math.Round(f, Global_variable.accuracy);
            }
            if (Global_variable.chosen_function == "System.Windows.Controls.ComboBoxItem: Himmelblau")
            {
                f = ((x * x + y - 11) * (x * x + y - 11)) + ((x + y * y - 7) * (x + y * y - 7));//Химмельблау
                return f = Math.Round(f, Global_variable.accuracy);
            }
            if (Global_variable.chosen_function == "System.Windows.Controls.ComboBoxItem: Rastrigin")
            {
                f = 20 + Math.Pow(x, 2) + Math.Pow(y, 2) - 10 * (Math.Cos(2 * Math.PI * x) + Math.Cos(2 * Math.PI * y));//Растригин
                return f = Math.Round(f, Global_variable.accuracy);
            }
            if (Global_variable.chosen_function == "System.Windows.Controls.ComboBoxItem: Rosenbrok")
            {
                f = Math.Pow(1 - x, 2) + 100 * Math.Pow(y - Math.Pow(x, 2), 2);//Розенброк
                return f = Math.Round(f, Global_variable.accuracy);
            }
            return 1000;
        }

        public List<Individual> ListForMax = new List<Individual>();
        public List<Individual> ListForMin = new List<Individual>();


        

        //Решение функции
        public void Solvefunction(List<Individual> UsingPopulations, int MaxMin)
        {
            
            List<Individual> UsingPopulations1 = new List<Individual>();

            for (int i = 0; i < UsingPopulations.Count; i++)
            {
                UsingPopulations[i].result_function = Function(UsingPopulations[i].decimal_value);
            }

            ListForMax = UsingPopulations1;
            ListForMin = UsingPopulations1;

            
            //блок сортировки для поиска минимумов или максимумов
            if (MaxMin == 1)
            {
                CompIndivMinToMax<Individual> ResSort = new CompIndivMinToMax<Individual>();
                UsingPopulations.Sort(ResSort);
                //TempRadiusSortList = new List<Individual>(UsingPopulations);
            }

            if (MaxMin == 2)
            {
                CompIndivMaxToMin<Individual> ResSort = new CompIndivMaxToMin<Individual>();
                UsingPopulations.Sort(ResSort);
                //TempRadiusSortList = new List<Individual>(UsingPopulations);
            }

            List<Individual> TempUsingPopulation = new List<Individual>();

            for (int i = 0; i < UsingPopulations.Count/10/**/; i++)
            {
                TempUsingPopulation.Add(UsingPopulations[i]);
            }
            //UsingPopulations.Clear();

            foreach (Individual ind in TempUsingPopulation)
            {
                UsingPopulations1.Add(ind);
            }
            //s.TempRadiusSortList = new List<Individual>(UsingPopulations1);
        }


        static public Student_tTest s = new Student_tTest();
        //Интерфейс для сортировки списка по полю result_function
        class CompIndivMinToMax<M> : IComparer<M>
        where M : Individual
        {
            // Реализуем интерфейс IComparer<T>
            public int Compare(M x, M y)
            {
                if (x.result_function > y.result_function)
                    return 1;
                if (x.result_function < y.result_function)
                    return -1;
                else return 0;
            }
        }

        class CompIndivMaxToMin<M> : IComparer<M>
        where M : Individual
        {
            // Реализуем интерфейс IComparer<T>
            public int Compare(M x, M y)
            {
                if (x.result_function < y.result_function)
                    return 1;
                if (x.result_function > y.result_function)
                    return -1;
                else return 0;
            }
        }


        public void PrintAllPopulation(List<Individual> UsingPopulation)
        {
            StreamWriter sw;
            sw = File.AppendText("AllPop.txt");
            int kol = 1;
            foreach (Individual k in UsingPopulation)
            {
                sw.WriteLine("Param #{0}={1}", kol, k.result_function);
                sw.WriteLine("");
                kol++;
            }
            sw.Close();
        }
    }

    public class Student_tTest
    {
        //SurfacePlotVisual3D mw = new SurfacePlotVisual3D();
        static public List<Cluster> ClusterList = new List<Cluster>(); //Список сформированных кластеров
        //public List<Individual> TempRadiusSortList = new List<Individual>();
        //SurfacePlotVisual3D mw = new SurfacePlotVisual3D();


        public void ClearClusterList()
        {
            ClusterList.Clear();
        }

        public void Student(Individual i)
        {
            if (ClusterList.Count == 0)
            {
                ClusterList.Add(new Cluster { });
            }

            int step = 0;

            foreach (Cluster k in ClusterList)
            {
                step++;
                if (k.ClusterIndiividual.Count != 0)
                {
                    if (k.ClusterIndiividual.Count == 1 || k.ClusterIndiividual.Count == 2)
                    {
                        double ResSumX = 0; //для первого параметра
                        double ResSumY = 0; //для второго параметра
                        double ResSumF = 0; //для значения функции

                        if (k.ClusterIndiividual.Count == 1)
                        {
                            ResSumX = k.ClusterIndiividual[0].decimal_value[0];
                            ResSumY = k.ClusterIndiividual[0].decimal_value[1];
                            ResSumF = k.ClusterIndiividual[0].result_function;
                        }
                        if (k.ClusterIndiividual.Count == 2)
                        {
                            ResSumX = k.ClusterIndiividual[0].decimal_value[0] + k.ClusterIndiividual[1].decimal_value[0];
                            ResSumY = k.ClusterIndiividual[0].decimal_value[1] + k.ClusterIndiividual[1].decimal_value[1];
                            ResSumF = k.ClusterIndiividual[0].result_function + k.ClusterIndiividual[1].result_function;
                        }
                        double ResKol = k.ClusterIndiividual.Count;
                        double ResX = ResSumX / ResKol - i.decimal_value[0];
                        double ResY = ResSumY / ResKol - i.decimal_value[1];
                        double ResF = ResSumF / ResKol - i.result_function;
                        if (Math.Abs(ResX) < 0.15 && Math.Abs(ResY) < 0.15 && Math.Abs(ResF)< 0.15)
                        {
                            k.ClusterIndiividual.Add(i);


                            //Action action = () => { mw.SphereShow(i.decimal_value[0], i.decimal_value[1], i.result_function); };
                            //Dispatcher.CurrentDispatcher.Invoke(action);
                            
                            //Global_variable.tempbox.Add(i.decimal_value[0]);
                            //Global_variable.tempbox.Add( i.decimal_value[1]);
                            //Global_variable.tempbox.Add( i.result_function);
                            //Global_variable.record = true;
                            break;
                        }
                    }
                    else
                    {
                        double yy1 = 0;
                        double yy2 = 0;
                        double ff3 = 0;
                        double sumX = 0;
                        double sumY = 0;
                        double sumF = 0;
                        foreach (Individual ind in k.ClusterIndiividual)
                        {
                            sumX += ind.decimal_value[0];
                            sumY += ind.decimal_value[1];
                            sumF += ind.result_function;
                        }
                        yy1 = sumX / k.ClusterIndiividual.Count;//среднее арифметическое списка X
                        yy2 = sumY / k.ClusterIndiividual.Count;//среднее арифметическое списка Y
                        ff3 = sumF / k.ClusterIndiividual.Count;//среднее арифметическое списка значений функции
                        double sX = 0;
                        double sY = 0;
                        double sF = 0;
                        int kolkol = 0;
                        for (int u = 0; u < k.ClusterIndiividual.Count; u++)
                        {
                            double s1 = 0;
                            double s2 = 0;
                            double f3 = 0;
                            s1 = k.ClusterIndiividual[u].decimal_value[0] - yy1;
                            s2 = k.ClusterIndiividual[u].decimal_value[1] - yy2;
                            f3 = k.ClusterIndiividual[u].result_function - ff3;
                            double ssX = 0;
                            double ssY = 0;
                            double ssF = 0;
                            ssX = s1 * s1;
                            ssY = s2 * s2;
                            ssF = f3 * f3;
                            sX = sX + ssX;
                            sY = sY + ssY;
                            sF = sF + ssF;
                            kolkol++;
                        }
                        double s3 = 0;
                        double s4 = 0;
                        double s5 = 0;
                        s3 = sX / (kolkol - 1);
                        s4 = sY / (kolkol - 1);
                        s5 = sF / (kolkol - 1);
                        double srsX = 0;
                        double srsY = 0;
                        double srsF = 0;
                        srsX = Math.Sqrt(s3);
                        srsY = Math.Sqrt(s4);
                        srsF = Math.Sqrt(s5);
                        double standX = 0;
                        double standY = 0;
                        double standF = 0;
                        standX = (i.decimal_value[0] - yy1) / (srsX);
                        standY = (i.decimal_value[1] - yy2) / (srsY);
                        standF = (i.result_function - ff3) / (srsF);

                        //standX = (i.decimal_value[0] - k.ClusterIndiividual[0].decimal_value[0]) / (srsX);
                        //standY = (i.decimal_value[1] - k.ClusterIndiividual[0].decimal_value[1]) / (srsY);
                        //standF = (i.result_function - k.ClusterIndiividual[0].result_function) / (srsF);

                        //standX = (yy1 - i.decimal_value[0]) / (srsX / Math.Sqrt(kolkol));
                        //standY = (yy2 - i.decimal_value[1]) / (srsY / Math.Sqrt(kolkol));
                        //standF = (ff3 - i.result_function) / (srsF / Math.Sqrt(kolkol));

                        if (standX < 0)
                        {
                            standX = standX - (standX * 2);
                        }
                        if (standY < 0)
                        {
                            standY = standY - (standY * 2);
                        }
                        if (standF < 0)
                        {
                            standF = standF - (standF * 2);
                        }
                        var chart = new System.Web.UI.DataVisualization.Charting.Chart();
                        double result = chart.DataManipulator.Statistics.InverseTDistribution(0.001, k.ClusterIndiividual.Count - 1);
                        if (standX < result && standY < result && standF<result)
                        {
                            k.ClusterIndiividual.Add(i);
                            break;
                        }
                    }
                }
                if (step == ClusterList.Count || step > ClusterList.Count)
                {
                   
                            ClusterList.Add(new Cluster { });
                            ClusterList[ClusterList.Count - 2].ClusterIndiividual.Add(i);
                            break;
                }
            }
        }

        public void RadiusDetect(List<Individual> TempRadiusSortList)
        {
            int clusterkol = ClusterList.Count-1;
            for (int cl=0; cl<clusterkol;cl++)
            {                
                    if (Global_variable.find_max_or_min == 1)
                    {
                        double func_value = ClusterList[cl].ClusterIndiividual[0].result_function;
                        int kolvo = 0;
                        foreach (Individual cltr in TempRadiusSortList)
                        {
                            cltr.radius = Math.Sqrt(Math.Pow(ClusterList[cl].ClusterIndiividual[0].decimal_value[0] - cltr.decimal_value[0], 2) + Math.Pow(ClusterList[cl].ClusterIndiividual[0].decimal_value[1] - cltr.decimal_value[1], 2));

                        }
                        List<Individual> TempTempRadiusSortList = new List<Individual>(TempRadiusSortList);
                        List<Individual> RadiusSortList = new List<Individual>();

                        CompRadiusMinToMax<Individual> ResSort = new CompRadiusMinToMax<Individual>();
                        TempTempRadiusSortList.Sort(ResSort);

                        for (int tmp = 0; tmp < TempTempRadiusSortList.Count; tmp++)
                        {
                            if (TempTempRadiusSortList[tmp].radius < 0.5)
                            {
                                RadiusSortList.Add(TempTempRadiusSortList[tmp]);
                            }
                        }
                        for (int rsl = 0; rsl < RadiusSortList.Count; rsl++)
                        {
                            if (RadiusSortList[rsl].result_function > func_value || RadiusSortList[rsl].result_function == func_value)
                            {
                                kolvo++;
                            }
                        }
                        if (kolvo < RadiusSortList.Count)
                        {
                            ClusterList.Remove(ClusterList[cl]);
                            clusterkol--;
                            cl--;
                            RadiusSortList.Clear();
                            TempTempRadiusSortList.Clear();
                        }
                    }
                    if (Global_variable.find_max_or_min == 2)
                    {
                        double func_value = ClusterList[cl].ClusterIndiividual[0].result_function;
                        int kolvo = 0;
                        foreach (Individual cltr in TempRadiusSortList)
                        {
                            cltr.radius = Math.Sqrt(Math.Pow(ClusterList[cl].ClusterIndiividual[0].decimal_value[0] - cltr.decimal_value[0], 2) + Math.Pow(ClusterList[cl].ClusterIndiividual[0].decimal_value[1] - cltr.decimal_value[1], 2));
                        }
                        List<Individual> TempTempRadiusSortList = new List<Individual>(TempRadiusSortList);
                        List<Individual> RadiusSortList = new List<Individual>();

                        CompRadiusMaxToMin<Individual> ResSort = new CompRadiusMaxToMin<Individual>();
                        TempTempRadiusSortList.Sort(ResSort);

                        for (int tmp = 0; tmp < TempTempRadiusSortList.Count; tmp++)
                        {
                            if (TempTempRadiusSortList[tmp].radius < 0.5)
                            {
                                RadiusSortList.Add(TempTempRadiusSortList[tmp]);
                            }
                        }
                        for (int rsl = 0; rsl < RadiusSortList.Count; rsl++)
                        {
                            if (RadiusSortList[rsl].result_function < func_value || RadiusSortList[rsl].result_function == func_value)
                            {
                                kolvo++;
                            }
                        }
                        if (kolvo < RadiusSortList.Count)
                        {
                            ClusterList.Remove(ClusterList[cl]);
                            clusterkol--;
                            cl--;
                            RadiusSortList.Clear();
                        }
                    }                
            }
        }

        class CompRadiusMinToMax<M> : IComparer<M>
        where M : Individual
        {
            // Реализуем интерфейс IComparer<T>
            public int Compare(M x, M y)
            {
                if (x.radius > y.radius)
                    return 1;
                if (x.radius < y.radius)
                    return -1;
                else return 0;
            }
        }

        class CompRadiusMaxToMin<M> : IComparer<M>
        where M : Individual
        {
            // Реализуем интерфейс IComparer<T>
            public int Compare(M x, M y)
            {
                if (x.radius < y.radius)
                    return 1;
                if (x.radius > y.radius)
                    return -1;
                else return 0;
            }
        }

        public void t_TestInit(List<Individual> UsingPopulation)
        {
            //int i = ClusterList.Count;
            ClusterList.Clear();
            Global_variable.for_clust = true;

            Global_variable.individ_maks = UsingPopulation.Count - 1;
            for ( int i=0;i<UsingPopulation.Count-1;i++) 
            {
                Student(UsingPopulation[i]);
                Global_variable.clustering_iteration++;
            }
            RadiusDetect(UsingPopulation);

            foreach (Cluster cl in ClusterList)
            {
                double x = 0;
                double y = 0;
                foreach (Individual i in cl.ClusterIndiividual)
                {
                    x = +i.decimal_value[0];
                    y = +i.decimal_value[1];
                }
                cl.MiddleValueX =Math.Abs( x / cl.ClusterIndiividual.Count);
                cl.MiddleValueY =Math.Abs( y / cl.ClusterIndiividual.Count);
            }
            
            //int i = ClusterList.Count;
            //for (int i=0;i<10000;i++)
            //{
            //    Student(UsingPopulation[i]);
            //}
            //Global_variable.ClusterGlobalList.Clear();




            if (Global_variable.KudaPisatCluster == true)
            {
                ClusterList.Remove(ClusterList[ClusterList.Count-1]); //из-за своей тупости
                Global_variable.LocalizeClusterGlobalList.AddRange(ClusterList);
            }
            if (Global_variable.KudaPisatCluster == false)
            {
                ClusterList.Remove(ClusterList[ClusterList.Count - 1]); //из-за своей тупости
                Global_variable.ClusterGlobalList = new List<Cluster>(ClusterList);
            }

            Global_variable.record = true;

            
        }
        


        public void ClusteringResultToTxtFile()
        {
            StreamWriter Sw;
            int k = 0;
            foreach (Cluster c in ClusterList)
            {
                k++;
                Sw = File.AppendText("ClusteringResult.txt");
                Sw.WriteLine("Cluster # {0}", k);
                Sw.Close();
                foreach (Individual ind in c.ClusterIndiividual)
                {
                    Sw = File.AppendText("ClusteringResult.txt");
                    Sw.WriteLine("F({0};{1})={2}", ind.decimal_value[0], ind.decimal_value[1], ind.result_function);
                    Sw.Close();
                }
            }
        }

        public void PrintExtrems()
        {
            StreamWriter Sw;
            int k = 0;
            for (int i = 0; i < ClusterList.Count - 1; i++)
            {
                Sw = File.AppendText("Extrems.txt");
                Sw.WriteLine("Extremum # {0} -> f({1},{2})={3}", k, ClusterList[i].ClusterIndiividual[0].decimal_value[0], ClusterList[i].ClusterIndiividual[0].decimal_value[1], ClusterList[i].ClusterIndiividual[0].result_function);
                Sw.Close();
                k++;
            }
        }
    }

    public class MethodRun
    {
        
        static public CreatePopulation i = new CreatePopulation();
        static public EvolutionaryGeneticAlg e = new EvolutionaryGeneticAlg();
        static public FunctionDeal f = new FunctionDeal();
        static public Student_tTest s = new Student_tTest();
        public void Init()
        {
            //Global_variable.ClusterGlobalList.Clear();

            i.GenericPopulation(Global_variable.population_count, CreatePopulation.CreatedPopulation);
            i.SelectionPopulation(CreatePopulation.CreatedPopulation);
            e.AllPopulationListAdd(CreatePopulation.SelectededPopulation);
            i.PrintStartPopulation(CreatePopulation.SelectededPopulation);
            int kol = 0;
            for (int j = 0; j < Global_variable.genration_count; j++)
            {
                StreamWriter sw;
                sw = File.AppendText("StartPop.txt");
                sw.WriteLine("Generation {0}", kol++);
                sw.Close();
                e.Crossingover();
                e.NewPopulationSynthesis();
                i.SelectionPopulation(EvolutionaryGeneticAlg.TempPopulation1);
                e.AllPopulationListAdd(CreatePopulation.SelectededPopulation);
                i.PrintStartPopulation(CreatePopulation.SelectededPopulation);
                Thread.Sleep(50);
                ProcessChanged(j);
            }
            if (Global_variable.find_max_or_min == 1)
            {
                f.Solvefunction(EvolutionaryGeneticAlg.AllPopulation, 1);
                f.PrintAllPopulation(EvolutionaryGeneticAlg.AllPopulation);
                s.ClearClusterList();
                s.t_TestInit(f.ListForMin);
                s.ClusteringResultToTxtFile();
                s.PrintExtrems();
            }
            if (Global_variable.find_max_or_min == 2)
            {
                f.Solvefunction(EvolutionaryGeneticAlg.AllPopulation, 2);
                f.PrintAllPopulation(EvolutionaryGeneticAlg.AllPopulation);
                s.ClearClusterList();
                s.t_TestInit(f.ListForMax);
                s.ClusteringResultToTxtFile();
                s.PrintExtrems();
            }
            if (Global_variable.find_max_or_min ==3)
            {
                Global_variable.find_max_or_min = 1;
                f.Solvefunction(EvolutionaryGeneticAlg.AllPopulation, 1);
                //f.PrintAllPopulation(EvolutionaryGeneticAlg.AllPopulation);
                s.ClearClusterList();
                s.t_TestInit(f.ListForMin);
                s.ClusteringResultToTxtFile();
                s.PrintExtrems();

                while (Global_variable.record == true) { }
                Global_variable.ti = -1;

                Global_variable.find_max_or_min = 2;
                f.Solvefunction(EvolutionaryGeneticAlg.AllPopulation, 2);
                //f.PrintAllPopulation(EvolutionaryGeneticAlg.AllPopulation);
                s.ClearClusterList();
                s.t_TestInit(f.ListForMax);
                s.ClusteringResultToTxtFile();
                s.PrintExtrems();                
            }


            Global_variable.LocalizeClusterGlobalList.Clear();
            //Global_variable.LocalizeClusterGlobalList.Clear();
            
            CreatePopulation.SelectededPopulation.Clear();
            CreatePopulation.CreatedPopulation.Clear();
            EvolutionaryGeneticAlg.AllPopulation.Clear();
            Global_variable.sbrospbar = true;
                

        }
        

        public void LocalizeSearch()
        {
                         
                    //ParamsChange(Global_variable.ClusterGlobalList[cl].ClusterIndiividual[0].decimal_value[0], Global_variable.ClusterGlobalList[cl].ClusterIndiividual[0].decimal_value[1]);

                    i.GenericPopulation(Global_variable.population_count, CreatePopulation.CreatedPopulation);
                    i.SelectionPopulation(CreatePopulation.CreatedPopulation);
                    e.AllPopulationListAdd(CreatePopulation.SelectededPopulation);
                    i.PrintStartPopulation(CreatePopulation.SelectededPopulation);
                    int kol1 = 0;
                    for (int j = 0; j < Global_variable.genration_count; j++)
                    {
                        StreamWriter sw;
                        sw = File.AppendText("StartPop.txt");
                        sw.WriteLine("Generation {0}", kol1++);
                        sw.Close();
                        e.Crossingover();
                        e.NewPopulationSynthesis();
                        i.SelectionPopulation(EvolutionaryGeneticAlg.TempPopulation1);
                        e.AllPopulationListAdd(CreatePopulation.SelectededPopulation);
                        i.PrintStartPopulation(CreatePopulation.SelectededPopulation);
                        Thread.Sleep(50);                        
                    }
                    if (Global_variable.find_max_or_min == 1)
                    {
                        f.Solvefunction(EvolutionaryGeneticAlg.AllPopulation, 1);
                        f.PrintAllPopulation(EvolutionaryGeneticAlg.AllPopulation);
                        s.ClearClusterList();
                        s.t_TestInit(f.ListForMin);
                        s.ClusteringResultToTxtFile();
                        s.PrintExtrems();
                    }
                    if (Global_variable.find_max_or_min == 2)
                    {
                        f.Solvefunction(EvolutionaryGeneticAlg.AllPopulation, 2);
                        f.PrintAllPopulation(EvolutionaryGeneticAlg.AllPopulation);
                        s.ClearClusterList();
                        s.t_TestInit(f.ListForMax);
                        s.ClusteringResultToTxtFile();
                        s.PrintExtrems();
                    }
                    if (Global_variable.find_max_or_min == 3)
                    {
                        Global_variable.find_max_or_min = 1;
                        f.Solvefunction(EvolutionaryGeneticAlg.AllPopulation, 1);
                        //f.PrintAllPopulation(EvolutionaryGeneticAlg.AllPopulation);
                        s.ClearClusterList();
                        s.t_TestInit(f.ListForMin);
                        s.ClusteringResultToTxtFile();
                        s.PrintExtrems();

                        while (Global_variable.record == true) { }
                        Global_variable.ti = -1;

                        Global_variable.find_max_or_min = 2;
                        f.Solvefunction(EvolutionaryGeneticAlg.AllPopulation, 2);
                        //f.PrintAllPopulation(EvolutionaryGeneticAlg.AllPopulation);
                        s.ClearClusterList();
                        s.t_TestInit(f.ListForMax);
                        s.ClusteringResultToTxtFile();
                        s.PrintExtrems();
                    }

                    
                    CreatePopulation.SelectededPopulation.Clear();
                    CreatePopulation.CreatedPopulation.Clear();
                    EvolutionaryGeneticAlg.AllPopulation.Clear();
                    
        }

        public event Action<int> ProcessChanged;
    }
}
