using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace Async2
{

    class Program
    {

        async Task<int> DoWithAsync()
        {
            Console.WriteLine("DoWithAsync Begin.");
            await Task.Delay(5000);
            Console.WriteLine("DoWithAsync End.");
            return 1024;
        }

        async Task<int> Run()
        {
            Console.WriteLine("Run Begin.");
            var t = DoWithAsync();
            Console.WriteLine("Awaiting.");
            for (int i = 0; i < 5; i++)
            {
                Console.Write(i + " ");
                Thread.Sleep(500);
            }
            Console.WriteLine();
            int n = await t;
            Console.WriteLine("Result is {0}", n);
            return n;
        }


        static int Main(string[] args)
        {
            try
            {
                return AsyncContext.Run(() => MainAsync(args));
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                return -1;
            }
        }

        static async Task<int> MainAsync(string[] args)
        {
            Program p = new Program();
            await p.Run();
            return 0;
        }
    }
}
