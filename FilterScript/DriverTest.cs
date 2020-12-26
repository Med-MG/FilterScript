using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace FilterScript
{
    class DriverTest
    {

        [Test]
        public void Test()
        {

            /*
             * Load The website
             * Fetch Emails From a Test File
             * Put email in Input
             * Click Go button
             * Wait for page to load again
             * Repeat Action 100 time
             * Download Excel File.
             * Close Page And Repeat Action.
             */

            //Driver Init
            IWebDriver driver = new ChromeDriver(@"C:\Users\Administrateur\Downloads\Production\YoucodeC#\FilterScript\ChromeDriver");
            //Email Input Locator
            var EmailInput = By.XPath("//*[@id='input-email-address']");
            //Excel Download Button
            var ExcelDownload = By.XPath("//*[@id='dt-grid1-buttons']/div/button[2]");
            //Launch Email Hippo website
            driver.Navigate().GoToUrl("https://tools.verifyemailaddress.io/");
            //Wait Until the page is loaded
            IWait<IWebDriver> wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(50.00));

            wait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));

            /*
             * Loop through the Emails file 
             * Check them out for validity
             * At the end Download Excel file
             */
            string fpath = @"C:\Users\Administrateur\Downloads\Production\YoucodeC#\FilterScript\EmailLists\emails.txt";
            if (File.Exists(fpath))
            {
                FileInfo fi = new FileInfo(fpath);
                //Open the file to read text
                using(StreamReader sr = fi.OpenText()) 
                {
                    string txt;
                    //Read the data from file, until the end of file is reached
                    while((txt = sr.ReadLine()) != null)
                    {
                        //Check email for validity
                        driver.FindElement(EmailInput).SendKeys(txt);

                        driver.FindElement(By.XPath("/html/body/div[4]/div[1]/div[1]/form/div/div[2]/div/div[2]/button")).Click();
                        //wait for page to load agin then 
                        WaitUntilElementVisible(EmailInput, driver, 30);
                    }

                    WaitUntilElementVisible(ExcelDownload, driver, 30);
                    //Download Excel File 


                }
            }


            //this will search for the element until a timeout is reached
            static IWebElement WaitUntilElementVisible(By EmailInput, IWebDriver  Driver, int timeout = 10)
            {
                try
                {
                    var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeout));
                    return wait.Until(ExpectedConditions.ElementIsVisible(EmailInput));
                }
                catch (NoSuchElementException)
                {
                    Console.WriteLine("Element with locator: '" + EmailInput + "' was not found.");
                    throw;
                }
            }


            //driver.Close();



        }

    }
}
