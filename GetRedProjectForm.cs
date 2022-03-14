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
        const string tableSem = "#div-Student__RecordBook > div.table-responsive > table";
        const string tableOther = "#div-Student__RecordBook > div:nth-child(5) > table";
        const string activeButton = "#recordBook__Pager > li.page-item.active"; 

        public GetRedProjectForm()
        {
            InitializeComponent();
        }

        private void StartButton_Click(object sender, EventArgs e) // TODO: переделать в метод init, кнопку убрать
        {

            BrowserCheck();
            SiteEntry();
            ScoreCalc(DataGathering());

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Thread.Sleep(4000);
            driver.Quit();
            Environment.Exit(0);
        }

        public void BrowserCheck()
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
        }

        public void SiteEntry()
        {
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
            driver.FindElement(By.CssSelector(cardButton)).Click();
        }

        public SortedList<string, int> DataGathering()
        {
            IList<IWebElement> elements = driver.FindElements(By.ClassName("page-item"));
            SortedList<string, int> subjects = new SortedList<string, int>();
            int btnNum = 1;
            for (int i = 1; i < elements.Count; i++)
            {
                if (Regex.IsMatch(driver.FindElement(By.CssSelector(activeButton)).Text, @"\d семестр"))
                {
                    Thread.Sleep(100);
                    IWebElement tbl = driver.FindElement(By.CssSelector(tableSem));
                    IList<IWebElement> rows = tbl.FindElements(By.TagName("tr"));
                    foreach (var row in rows.Skip(1))
                    {
                        IList<IWebElement> cols = row.FindElements(By.TagName("td"));
                        if (!Regex.IsMatch(cols.ElementAt(0).Text, "зачёты") && !Regex.IsMatch(cols.ElementAt(0).Text, "экзамены"))
                        {
                            Thread.Sleep(100);
                            if (!Regex.IsMatch(cols.ElementAt(3).Text, "Зачтено"))
                            {
                                if (subjects.ContainsKey(cols.ElementAt(0).Text))
                                {
                                    subjects.Remove(cols.ElementAt(0).Text);
                                    switch (cols.ElementAt(3).Text)
                                    {
                                        case "Отлично":
                                            subjects.Add(cols.ElementAt(0).Text, 5);
                                            break;
                                        case "Хорошо":
                                            subjects.Add(cols.ElementAt(0).Text, 4);
                                            break;
                                        case "Удовл.":
                                            subjects.Add(cols.ElementAt(0).Text, 3);
                                            break;
                                        default:
                                            subjects.Add(cols.ElementAt(0).Text, 2);
                                            break;
                                    }
                                }
                                else
                                {
                                    switch (cols.ElementAt(3).Text)
                                    {
                                        case "Отлично":
                                            subjects.Add(cols.ElementAt(0).Text, 5);
                                            break;
                                        case "Хорошо":
                                            subjects.Add(cols.ElementAt(0).Text, 4);
                                            break;
                                        case "Удовл.":
                                            subjects.Add(cols.ElementAt(0).Text, 3);
                                            break;
                                        default:
                                            subjects.Add(cols.ElementAt(0).Text, 2);
                                            break;
                                    }
                                }
                            }
                            else
                            {
                                if (subjects.ContainsKey(cols.ElementAt(0).Text))
                                {
                                    subjects.Remove(cols.ElementAt(0).Text);
                                }
                            }
                        }
                    }
                }
                else if (Regex.IsMatch(driver.FindElement(By.CssSelector(activeButton)).Text, @"Факультативы"))
                {

                }
                else
                {
                    IWebElement tbl = driver.FindElement(By.CssSelector(tableOther));
                    IList<IWebElement> rows = tbl.FindElements(By.TagName("tr"));
                    foreach (var row in rows.Skip(1))
                    {
                        IList<IWebElement> cols = row.FindElements(By.TagName("td"));
                        if (!Regex.IsMatch(cols.ElementAt(4).Text, "Зачтено"))
                        {
                            if (subjects.ContainsKey(cols.ElementAt(0).Text))
                            {
                                switch (cols.ElementAt(4).Text)
                                {
                                    case "Отлично":
                                        subjects.Add(cols.ElementAt(0).Text + " (Курсовая)", 5);
                                        break;
                                    case "Хорошо":
                                        subjects.Add(cols.ElementAt(0).Text + " (Курсовая)", 4);
                                        break;
                                    case "Удовл.":
                                        subjects.Add(cols.ElementAt(0).Text + " (Курсовая)", 3);
                                        break;
                                    default:
                                        subjects.Add(cols.ElementAt(0).Text + " (Курсовая)", 2);
                                        break;
                                }
                            }
                            else
                            {
                                switch (cols.ElementAt(4).Text)
                                {
                                    case "Отлично":
                                        subjects.Add(cols.ElementAt(0).Text, 5);
                                        break;
                                    case "Хорошо":
                                        subjects.Add(cols.ElementAt(0).Text, 4);
                                        break;
                                    case "Удовл.":
                                        subjects.Add(cols.ElementAt(0).Text, 3);
                                        break;
                                    default:
                                        subjects.Add(cols.ElementAt(0).Text, 2);
                                        break;
                                }
                            }
                        }
                    }
                }

                Thread.Sleep(100);
                btnNum++;
                driver.FindElement(By.CssSelector("#recordBook__Pager > li:nth-child(" + btnNum + ")")).Click();
                Thread.Sleep(100);
            }

            return subjects;
        }

        public void ScoreCalc(SortedList<string, int> subjects)
        {
            double avg = 0;
            foreach (var mark in subjects.Values)
                avg += mark;
            avg /= subjects.Count;

            if (subjects.ContainsValue(3))
            {
                label1.Text = "Ваш средний балл: " + Math.Round(avg, 2) + "   (Оценка 3 за " + subjects.ElementAt(subjects.IndexOfValue(3)) + ")";
            }
            else
            {
                label1.Text = "Ваш средний балл: " + Math.Round(avg, 2);
            }
        }
    }
}
