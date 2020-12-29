using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace FilterScript
{
    class DriverTest
    {
        //Driver Init
        ChromeDriver driver = DriverInit();
        //Email Input Locator
        By EmailInput = By.XPath("//*[@id='input-email-address']");
        //Go button
        By GoButton = By.XPath("/html/body/div[4]/div[1]/div[1]/form/div/div[2]/div/div[2]/button");
        //Excel Download Button
        By ExcelDownload = By.XPath("//*[@id='dt-grid1-buttons']/div/button[2]/span");
        //Iteration variable
        double NumIterations = 0;

       
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

            //Change Ip on init
            ChangeIpAddress(driver);

            //Launch Email Hippo website
            driver.Navigate().GoToUrl("https://tools.verifyemailaddress.io/");
            //Wait Until the page is loaded
            IWait<IWebDriver> wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(30.00));
            wait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));


            //check if access denied
            while (driver.Title != "Email Address Verifier - Validate and Check In Real Time | verifyemailaddress.io")
            {
                ChangeIpAddress(driver, true);

            }


            /*
             * Loop through the Emails file 
             * Check them out for validity
             * At the end Download Excel file
             */
            var docPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string fpath = $@"{docPath}/Filter/emails.txt";
            //string fpath = @"C:\Users\Administrateur\Downloads\Production\YoucodeC#\FilterScript\EmailLists\emails.txt";
            //string CheckedFile = @"C:\Users\Administrateur\Downloads\Production\YoucodeC#\FilterScript\EmailLists\Checkedemails.txt";
            if (File.Exists(fpath))
            {
                //FileInfo fi = new FileInfo(fpath);
                //Put All lines in a list

                List<string> AllEmails = File.ReadAllLines(fpath).ToList();
                double countPerhundred = AllEmails.Count / 99;
                NumIterations = Math.Floor(countPerhundred);


                //Duplicate the list
                List<string> OverRideList = new List<string>();
                OverRideList.AddRange(AllEmails);

                //Create the checked emails file
                //StreamWriter sw = File.AppendText(CheckedFile);
                // A random, to randomise the process
                Random rand = new Random(Guid.NewGuid().GetHashCode());

                while(NumIterations != 0)
                {
                    for (int i = 0; i < 99; i++)
                    {
                        //Check email for validity
                        driver.FindElement(EmailInput).SendKeys(AllEmails[i]);
                        System.Threading.Thread.Sleep(rand.Next(1000, 5000));
                        driver.FindElement(GoButton).Click();

                        //Put it in an Already checked file
                        //sw.WriteLine(AllEmails[i]);

                        //remove the email form list
                        OverRideList.Remove(AllEmails[i]);


                        //wait for page to load again then 
                        WaitUntilElementVisible(EmailInput, driver, 30);
                        System.Threading.Thread.Sleep(rand.Next(2000, 10000));

                    }


                    WaitUntilElementVisible(ExcelDownload, driver, 30);
                    //Download Excel File 
                    System.Threading.Thread.Sleep(rand.Next(1000, 2000));
                    var ele = driver.FindElement(ExcelDownload);
                    IJavaScriptExecutor jse = (IJavaScriptExecutor)driver;
                    jse.ExecuteScript("arguments[0].click()", ele);


                    //Override the Old file with the new list
                    File.WriteAllLines(fpath, OverRideList.ToArray());
                    AllEmails.Clear();
                    AllEmails.AddRange(OverRideList);

                    //Call ChangeIpAdress Method
                    ChangeIpAddress(driver);
                    driver.Navigate().Refresh();
                    //Wait Until the page is loaded
                    IWait<IWebDriver> waitAg = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(30.00));
                    waitAg.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));

                    //check if access denied
                    while (driver.Title != "Email Address Verifier - Validate and Check In Real Time | verifyemailaddress.io")
                    {
                        ChangeIpAddress(driver, true);

                    }

                    NumIterations--;
                }


            }





           

            //driver.Close();



        }

        //this will search for the element until a timeout is reached
        static IWebElement WaitUntilElementVisible(By EmailInput, IWebDriver Driver, int timeout = 10)
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

        static ChromeDriver DriverInit()
        {
            ChromeOptions opt = new ChromeOptions();
            //opt.AddExtensions(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)+"touchvpn.crx");
            opt.AddExtensions(@"C:\Users\Administrateur\source\repos\FilterScript\FilterScript\touchvpn.crx");
            ChromeDriver driver = new ChromeDriver(opt);
            return driver;
        }

         void ChangeIpAddress(ChromeDriver driver, bool exception = false)
        {
            var verifyEmailTab = driver.CurrentWindowHandle;
            //Execute some JavaScript to Open New page
            //driver.ExecuteScript("window.open();");
            // save a reference to our new tab's window handle, this would be the last entry in the WindowHandles collection
            var touchVpnPage = driver.WindowHandles[driver.WindowHandles.Count - 1];
            // switch our WebDriver to the new tab's window handle
            driver.SwitchTo().Window(touchVpnPage);

            //Go to extension Url 
            driver.Navigate().GoToUrl("chrome-extension://bihmplhobchoageeokmgbdihknkjbknd/panel/index.html");
            //Connection button Xpath
            By Connection = By.XPath("//*[@id='ConnectionButton']");
            //Wait until the button is visible on the page
            WaitUntilElementVisible(Connection, driver, 30);
            //Click the connection button
            driver.FindElement(Connection).Click();
            System.Threading.Thread.Sleep(5000);
            if (exception)
            {
                driver.FindElement(Connection).Click();
                System.Threading.Thread.Sleep(5000);
                driver.SwitchTo().Window(verifyEmailTab);
                driver.Navigate().Refresh();
                //Wait Until the page is loaded
                IWait<IWebDriver> wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(30.00));
                wait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
            }else
            {
                driver.SwitchTo().Window(verifyEmailTab);
            }
            //Close the touch vpn window
            //driver.ExecuteScript("window.close();");
            //Switch back to the original tab
            
        }
    }
}
