using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MovingLogicToLibrary
{
    public class TransformationUtility
    {
        public static void PrintFailures(List<string> failures)
        {
            foreach (string failure in failures)
            {
                Console.WriteLine(failure);
            }
        }
    }
}
