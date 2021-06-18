using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebasUnitarias
{
    class Program
    {
        static void Main(string[] args)
        {
            string rut = "17558736-5";
            Console.WriteLine(rut.Substring(0, rut.Length - 2));
            Console.ReadKey();
        }
    }
}
