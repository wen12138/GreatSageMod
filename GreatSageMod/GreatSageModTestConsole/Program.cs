using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using b1;

namespace GreatSageModTestConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var worldType = typeof(BGW_ECSWorld);

            var nonPublicFields = worldType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);

            foreach (var field in nonPublicFields)
            {
                Console.WriteLine($"{field.Name}");
            }

            Console.ReadKey();
        }
    }
}
