using System;
using System.Threading.Tasks;

namespace SaiCore
{
    class Program
    {
        public static Saiko Saiko;
        private static void Main(string[] args)
        {
            Saiko = new Saiko();

            Saiko.RunAsync().GetAwaiter().GetResult();
        }
    }
}
