using System;
using System.Runtime.InteropServices;
using MathEvaluator.Utils;

namespace MathEvaluator.Tests
{
    internal static class ConsoleExtensions
    {
        public static void Output(this string arg)
        {
            Console.WriteLine(arg ?? "[null]");
        }
        public static void Output(this object arg)
        {
            if (arg.IsNull())
            {
                "[null]".Output();
            }
            else
            {
                arg.ToString().Output();
            }
        }
    }
}