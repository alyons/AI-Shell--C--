using AI_Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Testing...");
            string testString = "8.0564";
            Console.WriteLine("Is " + testString + " a number of some sort? " + AIShellMathHelper.IsNumeric(testString));
            Console.WriteLine("Any key to quit...");
            Console.Read();
        }
    }
}
