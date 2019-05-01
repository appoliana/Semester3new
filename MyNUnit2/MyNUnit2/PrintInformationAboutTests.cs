using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNUnit
{
    public class PrintInformationAboutTests
    {
        public void PrintInformation(TimeSpan ts, string testName, string message)
        {
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
            Console.WriteLine("Test " + testName + " " + message + " Time: " + elapsedTime);
            //Console.WriteLine("RunTime " + elapsedTime);
        }
    }
}
