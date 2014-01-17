using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kernel.HttpServer.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var port = 10080;
            var server = new Kernel.HttpServer.Server(port);
            Console.WriteLine("Server running on http://localhost:" + port);
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
            server.Dispose();
        }
    }
}
