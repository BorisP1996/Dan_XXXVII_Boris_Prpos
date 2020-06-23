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
        static Semaphore semaphore = new Semaphore(2, 2);
        static object lockObject = new object();
        static List<Thread> threadList = new List<Thread>();
        static int count = 0;
        static int waitingTime = 0;

        static void Main(string[] args)
        {
            GenerateRoutes();
            Thread RouteGenerator = new Thread(() => SelectRoutes());
            RouteGenerator.Start();
            RouteGenerator.Join();

            Thread TruckGenerator = new Thread(() => CreateTrucks());
            TruckGenerator.Start();
            TruckGenerator.Join();

            Thread DeliverThread = new Thread(() => Deliver());
            DeliverThread.Start();
            DeliverThread.Join();

            Thread ShipingThread = new Thread(() => Shiping());
            ShipingThread.Start();

            Console.ReadLine();
        }
        static void Shiping()
        {
            Random rnd = new Random();

            lock (lockObject)
            {
                Monitor.Wait(lockObject);

                for (int i = 0; i < threadList.Count; i++)
                {
                    Console.WriteLine("{0} is headed to unloading point.(Expected delay time: 0.5s-5s)", threadList[i].Name);
                    int unloadWait = rnd.Next(500, 5001);
                    Thread.Sleep(unloadWait);
                    if (unloadWait < 3000)
                    {
                        Console.WriteLine("{0} has arrived at unloading site.", threadList[i].Name);
                        Thread.Sleep((int)(waitingTime / 1.5));
                        Console.WriteLine("{0} has unloaded the cargo.", threadList[i].Name);
                    }
                    else
                    {
                        Console.WriteLine("{0} is late! Going back to starting point...(Delay time bigger than 3s)", threadList[i].Name);
                        Thread.Sleep(unloadWait);
                        Console.WriteLine("{0} returned to starting point.", threadList[i].Name);
                    }
                }
            }
        }
        static void Deliver()
        {
            for (int i = 0; i < threadList.Count; i++)
            {
                Console.WriteLine("Truck: {0}\tRoute: {1}", threadList[i].Name, Routes[i]);
            }
        }
        static void CreateTrucks()
        {
            for (int i = 0; i < 10; i++)
            {
                Thread t = new Thread(() => Loading(Thread.CurrentThread))
                {
                    Name = string.Format("Truck {0}", i)
                };
                t.Start();
                threadList.Add(t);
            }
        }
        static void Loading(Thread t)
        {
            Random rnd = new Random();
            semaphore.WaitOne();
            waitingTime = rnd.Next(500, 5001);
            Thread.Sleep(waitingTime);
            Console.WriteLine("{0} is loading.", t.Name);
            semaphore.Release();
            Console.WriteLine("{0} has left the loading place.{1}\n", t.Name, count++);

            if (count > 9)
            {
                lock (lockObject)
                {
                    Monitor.Pulse(lockObject);
                }
            }
        }
        static void GenerateRoutes()
        {
            Console.WriteLine("Manager is waiting for routes to generate...");
            Random rnd = new Random();
            Thread.Sleep(rnd.Next(1000, 3000));
            StreamWriter sw = new StreamWriter(path, false);

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

            int counter = 0;

            for (int i = 0; i < tempList.Count; i++)
            {
                if (tempList[i] % 3 == 0)
                {
                    if (Routes.Contains(tempList[i]))
                    {
                        continue;
                    }
                    else
                    {
                        Routes.Add(tempList[i]);
                        counter++;
                    }
                }
                if (Routes.Count() == 10)
                {
                    break;
                }
            }
            Console.WriteLine("Best routes:");
            foreach (int item in Routes)
            {
                Console.WriteLine("Route:" + item);
            }
            Console.WriteLine("\n Routes are chosen. Trucks can head to the loading site.\n");
        }
    }
}
