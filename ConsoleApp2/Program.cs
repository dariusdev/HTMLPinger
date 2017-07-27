using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Timers;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace ConsoleApp2
{
    class Program
    {
        private static readonly IWebDriver driver = new ChromeDriver();
        static void Main(string[] args)
        {
           Console.WriteLine(DateTime.Now.ToString() + " Started");

            int timers = Int32.Parse(ConfigurationManager.AppSettings["TimerS"]);

            CheckWebPage();

            var aTimer = new System.Timers.Timer(timers*1000);
            aTimer.Elapsed += OnTimedEvent;
            aTimer.Start();
             
            Console.WriteLine(DateTime.Now.ToString() + " Click enter to escape ...");
            Console.ReadLine();

            driver.Close();
            driver.Quit();
        }
        


        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            Console.WriteLine(DateTime.Now.ToString() + " timer start");

            CheckWebPage();

            Console.WriteLine(DateTime.Now.ToString() + " timer finished");
        }




        private static void CheckWebPage()
        {
            int sleepAfterRequestMS = Int32.Parse(ConfigurationManager.AppSettings["SleepAfterRequestMS"]);
            string checkValue = ConfigurationManager.AppSettings["CheckPhrase"];
            string url = ConfigurationManager.AppSettings["Address"];

            driver.Navigate().GoToUrl(url);

            Thread.Sleep(sleepAfterRequestMS);

            String html = driver.PageSource;


            if (!html.Contains(checkValue))
            {
                SendEmailWithErro(driver.Url, html, checkValue);
            }
            else
            {
                Console.WriteLine(DateTime.Now + " PASS");
            }
        }




        private static void SendEmailWithErro(string url, string result, string phrase)
        {
            string destinationEmail = ConfigurationManager.AppSettings["DestinationEmail"];
            string fromEmail = ConfigurationManager.AppSettings["FromEmail"];
            string fromEmailPassword = ConfigurationManager.AppSettings["FromEmailPassword"];

            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(fromEmail, fromEmailPassword),
                EnableSsl = true
            };

            string message = "Link Address Was Not Found in:\r\n" + url + "\r\n \r\n"
                + "Phrase: \r\n" + phrase + " \r\n" + " \r\n"
                + "HTML: \r\n" + result;


            client.Send(fromEmail, destinationEmail, url, message );
            Console.WriteLine(DateTime.Now + " Email Sent");
        }
    }
}
