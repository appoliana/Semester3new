using System;

namespace MyNUnit
{
    /// <summary>
    /// Класс, который печатает информацию о тесте.
    /// </summary>
    public class PrintInformationAboutTests
    {
        /// <summary>
        /// Метод, который печатает информацию о тесте.
        /// </summary>
        public void PrintInformation(TimeSpan ts, string testName, string message)
        {
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
              ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            Console.WriteLine($"Test {testName} {message} Time: {elapsedTime}");
        }
    }
}
