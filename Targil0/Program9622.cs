using System;

namespace Targil0
{
    partial class Program
    {
        static void Main(string[] args)
        {
            Welcome9622();
            Welcome6784();
            Console.ReadKey();
           
        }
        static partial void Welcome6784();
        private static void Welcome9622()
        {
            Console.Write("Enter your name:");
            string userName = Console.ReadLine();
            Console.WriteLine("{0}, welcome to my first console application", userName);
        }
    }
}
