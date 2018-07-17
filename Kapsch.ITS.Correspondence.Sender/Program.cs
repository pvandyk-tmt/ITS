using System;
using System.Threading;

namespace Kapsch.ITS.Correspondence.Sender
{
    class Program
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args)
        {
            Log.Info("Job started.");

            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;

            var sender = new FirstNoticeSender();
            sender.Execute(token);

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
