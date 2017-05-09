using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Threading;

namespace SurfacePlotDemo
{
    class Global_variable
    {
        static public bool a = false;

        static public int accuracy = 5; //точность (колличество знаков после запятой) -> в дальнейшем добавить разные варианты задания точности
        static public int params_number = 2; //коллическтво параметров
        static public string chosen_function; //выбранная функция
        static public int population_count; //количество особей в одной популяции
        static public int genration_count; //количество поколений
        static public int crossingover_probability; //вероятность выполнения скрещивания
        static public int mutation_probability; //вероятность выполнения мутации
        static public double[] upper_bound_array = new double[params_number];//верхние границы поиска параметров
        static public double[] lower_bound_array = new double[params_number];//нижние границы поиска параметров
        static public double[] bit_count_array = new double[params_number];//колличество бит одной особи

        static public List<Cluster> ClusterGlobalList = new List<Cluster>();

        static public List<Cluster> LocalizeClusterGlobalList = new List<Cluster>();
        //static public int boundbound = 4;
        static public int find_max_or_min = 0; // Параметр искомых экстремумов (если равняется 1, то ищем минимумы, если 2, то ищем максимумы, если 3, то ищем и минимумы и максимумы)

        //static public List<double> tempbox = new List<double>();
        static public bool record = false;

        static public int kol_elem; //кол элементов в surface
        static public bool surface=true;
        static public int ti = -1;

        static public int clustering_iteration=0;

        static public bool for_clust = false;
        static public int individ_maks = 0;
        //static public Thread thread;
        static public bool sbrospbar = false;

        static public bool localizeSearch = false;



        //параметры для локализации
        public static bool KudaPisatCluster = false;

        //Номер уточнения
        static public int UtochnenNumb=1;
        
    }    
}
