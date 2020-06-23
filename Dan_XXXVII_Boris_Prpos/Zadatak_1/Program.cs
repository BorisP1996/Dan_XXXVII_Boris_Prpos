using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Zadatak_1
{
    class Program
    {
        static string path = @"../../Routes.txt";
        static List<int> Routes = new List<int>();

        static void Main(string[] args)
        {
            GenerateRoutes();
            Thread RouteGenerator = new Thread(() => SelectRoutes());
            RouteGenerator.Start();

            Console.ReadLine();

        }
        static void GenerateRoutes()
        {
            StreamWriter sw = new StreamWriter(path, false);
            Random rnd = new Random();

            for (int i = 0; i < 1000; i++)
            {
                sw.WriteLine(rnd.Next(1, 5001));
            }
            sw.Close();
        }
        static void SelectRoutes()
        {
            List<int> tempList = new List<int>();
            StreamReader sr = new StreamReader(path);
            string line = "";
            while ((line = sr.ReadLine()) != null)
            {
                tempList.Add(Convert.ToInt32(line));
            }
            sr.Close();
            tempList.Sort();

            int counter=0;

            for (int i = 0; i < tempList.Count; i++)
            {
                if (tempList[i] % 3 == 0)
                {
                    Routes.Add(tempList[i]);
                    counter++;
                }
                if (Routes.Count == 10)
                {
                    break;
                }
            }
            Console.WriteLine("Best routes:");
            foreach (int item in Routes)
            {
                Console.WriteLine("Route:"+item);
            }
        }
    }
}
