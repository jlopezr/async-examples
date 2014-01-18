using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hiperion
{
    class Program
    {
        public void Run()
        {
            // Create data container
            Data d = new Data();

            // Application based interface 
            d.Put("altura", 10);
            int n1 = (int)d.Get("altura");

            // Contract based interface
            d.Put(new SpecificMetadata1(123));
            d.Put(new SpecificMetadata2(1, 2, 3));
            SpecificMetadata1 n2 = d.Get<SpecificMetadata1>();
            SpecificMetadata2 n3 = d.Get<SpecificMetadata2>();


            StorageService s = new StorageService();
            s.Put("0", "pepito", d);
            
            var result = from n in s.Dates orderby n.date select n;
            foreach (var e in result)
            {
                Console.WriteLine(e.date);
            }             

            s.Dispose();
        
        }

        static void Main(string[] args)
        {
            Program p = new Program();
            p.Run();
        }
    }
}
