//-----------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Keysight Technologies">
//      Copyright (C) 2010-2014 Keysight Technologies
// </copyright>
//-----------------------------------------------------------------------
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Keysight.Ccl.Wsl.UI;
using OpenTap;

namespace Keysight.Ccl.Wsl.Samples.SplashScreenSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : WslMainWindow
    {
        UI_Data container;

        public MainWindow()
        {
            UXManager.Initialize();
            InitializeComponent();
            

            container = new UI_Data();
            DataContext = container;
        }

        private void FileExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        

        public void Log_In_Click(object sender, RoutedEventArgs e)
        {
            if (textBox_pw.Text == "999")

            {
                button_VNA_A.IsEnabled = IsEnabled;

                button_VNA_G.IsEnabled = IsEnabled;

                button_VNA_W.IsEnabled = IsEnabled;

                textBox_pw.Text = "";

            }

            else

            {
                MessageBox.Show("You Must Enter Correct Password");

            }
        }

        private void Log_In_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void button_VNA_A_Click(object sender, RoutedEventArgs e)
        {
             Task.Factory.StartNew(() =>
            {
                // Start finding plugins.

                PluginManager.DirectoriesToSearch.Add(@"C:\Program Files\Keysight\Test Automation\Packages\");

                PluginManager.SearchAsync();

                // Maps the directory of the bench to be used
                ComponentSettings.SettingsDirectoryRoot = "C:\\Program Files\\Keysight\\Test Automation\\Settings\\";

                // In which folder the bench should be found in this case this is the Default one
                ComponentSettings.SetSettingsProfile("Bench", "Default");

                TestPlan myTestPlan = TestPlan.Load(@"C:\Program Files\Keysight\Test Automation\VNA.TapPlan");

                

                foreach (TestStep testStep in myTestPlan.Steps)
                {

                    //Console.WriteLine(testStep.ToString() + " : Type   " + testStep.GetType());
                    //Console.WriteLine(testStep.Name);

                    if (testStep is OpenTap.Plugins.VNA.VNA_Control)
                    {

                        (testStep as OpenTap.Plugins.VNA.VNA_Control).Start_Freq = "10000000";


                        (testStep as OpenTap.Plugins.VNA.VNA_Control).Stop_Freq = "20000000";

                        (testStep as OpenTap.Plugins.VNA.VNA_Control).Operator = container.OperatorName;

                        (testStep as OpenTap.Plugins.VNA.VNA_Control).Sn = container.SerialNumber;

                    }
                    else
                    {

                    }



                }

                TestPlanRun myTestPlanRun = myTestPlan.Execute();
            });
        }


        // This below helps in having Enter Key validating the value sets
        private void WslMainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            EventManager.RegisterClassHandler(typeof(TextBox), TextBox.KeyDownEvent, new KeyEventHandler(TextBox_KeyDown));
        }

        void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter & (sender as TextBox).AcceptsReturn == false)
                MoveToNext(e);
        }

        void MoveToNext(KeyEventArgs e)
        {
            FocusNavigationDirection focusDirection = FocusNavigationDirection.Next;

            TraversalRequest request = new TraversalRequest(focusDirection);

            if (Keyboard.FocusedElement is UIElement elementWithFocus)
            {
                if (elementWithFocus.MoveFocus(request)) e.Handled = true;
            }
        }

        private void button_VNA_G_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                // Start finding plugins.

                PluginManager.DirectoriesToSearch.Add(@"C:\Program Files\Keysight\Test Automation\Packages\");

                PluginManager.SearchAsync();

                // Maps the directory of the bench to be used
                ComponentSettings.SettingsDirectoryRoot = "C:\\Program Files\\Keysight\\Test Automation\\Settings\\";

                // In which folder the bench should be found in this case this is the Default one
                ComponentSettings.SetSettingsProfile("Bench", "Default");

                TestPlan myTestPlan = TestPlan.Load(@"C:\Program Files\Keysight\Test Automation\VNA.TapPlan");



                foreach (TestStep testStep in myTestPlan.Steps)
                {

                    //Console.WriteLine(testStep.ToString() + " : Type   " + testStep.GetType());
                    //Console.WriteLine(testStep.Name);

                    if (testStep is OpenTap.Plugins.VNA.VNA_Control)
                    {

                        (testStep as OpenTap.Plugins.VNA.VNA_Control).Start_Freq = "20000000";


                        (testStep as OpenTap.Plugins.VNA.VNA_Control).Stop_Freq = "40000000";

                        (testStep as OpenTap.Plugins.VNA.VNA_Control).Operator = container.OperatorName;

                        (testStep as OpenTap.Plugins.VNA.VNA_Control).Sn = container.SerialNumber;

                    }
                    else
                    {

                    }



                }

                TestPlanRun myTestPlanRun = myTestPlan.Execute();
            });
        }

        private void button_VNA_W_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                // Start finding plugins.

                PluginManager.DirectoriesToSearch.Add(@"C:\Program Files\Keysight\Test Automation\Packages\");

                PluginManager.SearchAsync();

                // Maps the directory of the bench to be used
                ComponentSettings.SettingsDirectoryRoot = "C:\\Program Files\\Keysight\\Test Automation\\Settings\\";

                // In which folder the bench should be found in this case this is the Default one
                ComponentSettings.SetSettingsProfile("Bench", "Default");

                TestPlan myTestPlan = TestPlan.Load(@"C:\Program Files\Keysight\Test Automation\VNA.TapPlan");



                foreach (TestStep testStep in myTestPlan.Steps)
                {

                    //Console.WriteLine(testStep.ToString() + " : Type   " + testStep.GetType());
                    //Console.WriteLine(testStep.Name);

                    if (testStep is OpenTap.Plugins.VNA.VNA_Control)
                    {

                        (testStep as OpenTap.Plugins.VNA.VNA_Control).Start_Freq = "30000000";


                        (testStep as OpenTap.Plugins.VNA.VNA_Control).Stop_Freq = "40000000";

                        (testStep as OpenTap.Plugins.VNA.VNA_Control).Operator = container.OperatorName;

                        (testStep as OpenTap.Plugins.VNA.VNA_Control).Sn = container.SerialNumber;

                    }
                    else
                    {

                    }



                }

                TestPlanRun myTestPlanRun = myTestPlan.Execute();
            });
        }

        private void Log_Off_Click(object sender, RoutedEventArgs e)
        {
            button_VNA_A.IsEnabled = false;

            button_VNA_G.IsEnabled = false;

            button_VNA_W.IsEnabled = false;

        }
    }




}
