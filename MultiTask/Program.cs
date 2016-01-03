using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiTask
{
    class Program
    {
        static void Main(string[] args)
        {
            // Seconds to wait
            int secondsWait = 2;
            // Cancellation Token to Throw an Excepcion
            CancellationTokenSource cts = new CancellationTokenSource();
            List<Task> listTasks = new List<Task>();
            listTasks.Add(Task<string>.Factory.StartNew(() => ProcessLoop(cts, 1000, "loop1")));
            listTasks.Add(Task<string>.Factory.StartNew(() => ProcessLoop(cts, 100000, "loop2")));
            try
            {
                Task.WaitAll(listTasks.ToArray(), secondsWait * 1000);
                cts.Cancel();
            }
            catch (AggregateException)
            {
                throw;
            }
            Console.ReadLine();
        }

        /// <summary>
        /// Process a simple loop
        /// </summary>
        /// <param name="cts">CancellationToken</param>
        /// <param name="number">Number Loops</param>
        /// <param name="name">Name Loop</param>
        /// <returns></returns>
        private static string ProcessLoop(CancellationTokenSource cts, int number, string name)
        {
            Stopwatch watch = new Stopwatch();
            try
            {
                watch.Start();
                for (int i = 0; i < number; i++)
                {
                    cts.Token.ThrowIfCancellationRequested();
                    Console.WriteLine(string.Format("Name: {0} - Number: {1}", name, i));
                }
                watch.Stop();
                Console.WriteLine(string.Format("*** OK: Total {0}: {1} seconds ***", name, watch.Elapsed.Seconds));
                return string.Format("************************ OK: Total {0}: {1} seconds ***", name, watch.Elapsed.Seconds);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine(string.Format("*** KO: Total {0}: {1} seconds ***", name, watch.Elapsed.Seconds));
                return string.Format("*** KO: Total {0}:qweqwe {1} seconds ***", name, watch.Elapsed.Seconds);
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("**** Exception {0}, Total :{1} seconds ***", ex.Message, watch.Elapsed.Seconds));
                throw;
            }
        }

    }
}