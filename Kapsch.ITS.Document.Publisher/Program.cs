using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kapsch.ITS.Document.Publisher
{
    class Program
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args)
        {
            Log.Info("Job started.");

            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;

            var publisher = new NoticePublisher();
            publisher.Execute(token);

            //char ch = Console.ReadKey().KeyChar;
            //if (ch == 'c' || ch == 'C')
            //{
            //    tokenSource.Cancel();
            //    Console.WriteLine("\nTask cancellation requested.");
            //}

            //Console.ReadKey();

            Log.Info("Job complete.");
        }
    }
}
