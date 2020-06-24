using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Zadatak_1
{
    class Program
    {
        static string path = @"../../Routes.txt";
        //list contains routes
        static List<int> Routes = new List<int>();
        //list contains threads
        static List<Thread> threadList = new List<Thread>();
        static int waitingTime = 0;
        static object lockObject = new object();

        static void Main(string[] args)
        {
            GenerateRoutes();
            Thread RouteGenerator = new Thread(() => SelectRoutes());
            RouteGenerator.Start();
            RouteGenerator.Join();

            Thread TruckGenerator = new Thread(() => CreateTrucks());
            TruckGenerator.Start();
            TruckGenerator.Join();

            Thread ShipingThread = new Thread(() => Shiping());
            ShipingThread.Start();
            ShipingThread.Join();

            Thread DeliveringThread = new Thread(() => Delivering());
            DeliveringThread.Start();        

            Console.ReadLine();
        }
        /// <summary>
        /// Via this method, every truck starts at the same time
        /// </summary>
        static void Shiping()
        {
            Random rnd = new Random();

            for (int i = 0; i < threadList.Count; i++)
            {
                Console.WriteLine("{0} is headed to unloading point,using route {1}.(Expected delay time: 0.5s-5s)", threadList[i].Name, Routes[i]);        
            }
        }  
        /// <summary>
        /// Trucks (threads) deliver cargo, if they are late they return to starting point
        /// </summary>
        static void Delivering()
        {
            Random rnd = new Random();
            //going through static list that contains threads
            for (int i = 0; i < threadList.Count; i++)
            {
                lock (lockObject)
                {
                    //random waiting time
                    int unloadWait = rnd.Next(500, 5001);
                    Thread.Sleep(unloadWait);
                    //if time is under 3 sec, everything is ok
                    if (unloadWait< 3000)
                    {
                        Console.WriteLine("{0} has arrived at unloading site.", threadList[i].Name);
                        Thread.Sleep((int)(unloadWait / 1.5));
                        Console.WriteLine("{0} has unloaded the cargo.", threadList[i].Name);
                    }
                    //if time is above 3 sec, they return back
                    else
                    {
                        Console.WriteLine("{0} is late! Going back to starting point...(Delay time bigger than 3s)", threadList[i].Name);
                        Thread.Sleep(unloadWait);
                        Console.WriteLine("{0} returned to starting point.", threadList[i].Name);
                    }
                }
            }         
        }
        /// <summary>
        /// Creating threads 
        /// </summary>
        static void CreateTrucks()
        {
            for (int i = 0; i < 10; i++)
            {
                //forwarding method to thread
                Thread t = new Thread(() => Loading(Thread.CurrentThread))
                {
                    Name = string.Format("Truck {0}", i + 1)
                };
                //inserting them into list
                threadList.Add(t);
            }
            int a = 0;
            //starting threads two by two..1 iteration: 0,1; 2 iteration: 2,3 .....
            for (int i = 0; i < 5; i++)
            {               
                threadList[a].Start();
                int temp = ++a;
                threadList[temp].Start();
                threadList[a].Join();
                threadList[temp].Join();
                a++;
            }
        }
        /// <summary>
        /// threads (trucks) are loading cargo. Each one waits random amount of time (500-5000 ms)
        /// </summary>
        /// <param name="t"></param>
        static void Loading(Thread t)
        {
            Random rnd = new Random();
            waitingTime = rnd.Next(500, 5001);
            Console.WriteLine("{0} is loading.", t.Name);
            Thread.Sleep(waitingTime);         
            Console.WriteLine("\t{0} has left the loading place.", t.Name);
            
        }
        /// <summary>
        /// Method generates numbers, 10 smallest that can be divided with 3 will be selected
        /// </summary>
        static void GenerateRoutes()
        {
            Console.WriteLine("Manager is waiting for routes to generate...");
            Random rnd = new Random();
            Thread.Sleep(rnd.Next(1000, 3000));
            StreamWriter sw = new StreamWriter(path, false);
            //writing 1000 random numbers to file
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
            //reading numbers from file
            while ((line = sr.ReadLine()) != null)
            {
                tempList.Add(Convert.ToInt32(line));
            }
            sr.Close();
            //sorting list so that 10 smallest will be first
            tempList.Sort();

            int counter = 0;
            //collecting 10 numbers (that can be divided by 3) and puting them into list
            for (int i = 0; i < tempList.Count; i++)
            {
                if (tempList[i] % 3 == 0)
                {
                    //only distinct numbers
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
                //when there are 10 numbers => break
                if (Routes.Count() == 10)
                {
                    break;
                }
            }
            //displaying routes from list
            Console.WriteLine("Best routes:");
            foreach (int item in Routes)
            {
                Console.WriteLine("Route:" + item);
            }
            Console.WriteLine("\n Routes are chosen. Trucks can head to the loading site.\n");
        }
    }
}
