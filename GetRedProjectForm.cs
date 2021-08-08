using System;
using System.Threading;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace GetRedProject
{
    public partial class GetRedProjectForm : Form
    {
        private IWebDriver driver;

        const string enterButton = "/html/body/div/div/div/form/div[3]/button";
        const string cardButton = "#miEvents > a:nth-child(1)";
        const string table = "#div-Student__RecordBook > div.table-responsive > table";

        public GetRedProjectForm()
        {
            InitializeComponent();
        }

        private void StartButton_Click(object sender, EventArgs e) // TODO: переделать в метод init, кнопку убрать
        {
            var process = Process.GetProcessesByName("chrome");
            if (process.Length != 0)
            {
                if (MessageBox.Show("Google Chrome будет закрыт.", "Предупреждение", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    foreach (var proc in Process.GetProcessesByName("chrome"))
                        proc.Kill();
                }
                else
                {
                    Environment.Exit(0);
                }
            }

            var options = new ChromeOptions();
            options.AddArgument("user-data-dir=C:\\Users\\" + Environment.UserName + "\\AppData\\Local\\Google\\Chrome\\User Data");
            driver = new ChromeDriver(options);

            driver.Navigate().GoToUrl(@"https://bars.mpei.ru/bars_web");
            try
            {
                driver.FindElement(By.XPath(enterButton)).Click();
            }
            catch (Exception)
            {
                driver.FindElement(By.CssSelector(cardButton)).Click();
            }
            
            IList<IWebElement> elements = driver.FindElements(By.ClassName("page-item"));
            SortedList<string, string> subjects = new SortedList<string, string>();
            for (int i = 0; i < elements.Count-1; i++)
            {
                if (Regex.IsMatch(elements.ElementAt(i).Text, @"\d семестр"))
                {
                    IWebElement tbl = driver.FindElement(By.CssSelector(table));
                    IList<IWebElement> rows = tbl.FindElements(By.TagName("tr"));
                    foreach (var row in rows.Skip(1))
                    {
                        IList<IWebElement> cols = row.FindElements(By.TagName("td"));
                        if (!Regex.IsMatch(cols.ElementAt(0).Text, "зачёты") && !Regex.IsMatch(cols.ElementAt(0).Text, "экзамены"))
                        {
                            if (!Regex.IsMatch(cols.ElementAt(3).Text, "Зачтено"))
                            {
                                if (subjects.ContainsKey(cols.ElementAt(0).Text))
                                {
                                    subjects.Remove(cols.ElementAt(0).Text);
                                    subjects.Add(cols.ElementAt(0).Text, cols.ElementAt(3).Text);
                                }     
                                else
                                {
                                    subjects.Add(cols.ElementAt(0).Text, cols.ElementAt(3).Text);
                                }
                            }                
                        }
                    }
                }
                else
                {
                    
                }
                
                
                int btnNum = i + 1;
                Thread.Sleep(300);
                driver.FindElement(By.CssSelector("#recordBook__Pager > li:nth-child(" + btnNum + ")")).Click();
            }


            Thread.Sleep(4000);
            driver.Quit();
            Environment.Exit(0);
        }
    }
}
