//-----------------------------------------------------------------------
// <copyright file="App.xaml.cs" company="Keysight Technologies">
//      Copyright (C) 2011-2014 Keysight Technologies
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using Keysight.Ccl.Wsl.UI.Dialogs;


namespace Keysight.Ccl.Wsl.Samples.SplashScreenSample
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private static ISplashScreen SplashScreen { get; set; }

        /// <summary>
        /// Application entry point.
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            try
            {
                var app = new App();
                var mainWindow = new MainWindow();

                // Set up event handler to show simple usage of how to bring down the splash screen
                mainWindow.ContentRendered += new EventHandler(MainWindow_ContentRendered);

                InitializeAndShowSplashScreen();

                // Select the various demo types to see the three different levels of information the SplashScreen can display.
                SplashScreenDemoType splashScreenDemoType;
                //splashScreenDemoType = SplashScreenDemoType.Simple;
                //splashScreenDemoType = SplashScreenDemoType.Intermediate;
                splashScreenDemoType = SplashScreenDemoType.Advanced;

                DemonstrateSplashScreenFeatures(splashScreenDemoType);

                app.Run(mainWindow);
            }
            catch (Exception)
            {
                if (App.SplashScreen != null)
                {
                    App.SplashScreen.CloseSplashScreen(false);
                }
                throw;
            }
        }

        private static void MainWindow_ContentRendered(object sender, EventArgs e)
        {
            // The main window is up and raring to go. Let's take down the splash screen.
            if (App.SplashScreen != null)
            {
                App.SplashScreen.CloseSplashScreen(true);
            }

            //Since the Main window is not setup as the parent of the splash screen may need to retop/activate it. This is because in a normal situation the Keysight app may be doing significant processing before the main window is even created or known and so there is no opportunity to parent things
            App.Current.MainWindow.Activate();
        }

        /// <summary>
        /// This code demonstrates the use of the WSL SDK SplashScreen.
        /// </summary>
        /// <remarks>
        /// It is meant to take care of all your needs as long as you provide XAML UserControl defining your product-specific 
        /// area (including the graphic you'd like to use) and use the ISplashScreen interface to set appropriate properties 
        /// and control behavior.
        /// 
        /// As per the Industrial Design standard, the maximum area available for the product identity information is
        /// 250px wide x 220px high.  The product name text is 38px, the product id text is 24px, and the additional information
        /// text is 12px.  The MaxHeight of each of the product identity elements is 220px to provide flexibility in how the
        /// product information can be displayed.
        /// 
        /// The topping of the mainwindow after the splashscreen closes may not work properly. According to Microsoft, the "owning" 
        /// window is not guaranteed to top after a child windows closes. You may have to re-"Activate" your mainwindow after the 
        /// splashscreen closes.
        /// 
        /// GenericProductSplashScreen.xaml is a simple sample of what you may want to do from your user control.
        /// </remarks>
        private static void InitializeAndShowSplashScreen()
        {
            //Pass in the type of your XAML-based object to be instantiated as the background/product specific area of the splash.
            //The product name and copyright date(s) are not optional and must be passed into the constructor.
            App.SplashScreen = SplashScreenClassFactory.CreateSplashScreen(typeof(GenericProductSplashScreen), "Antenna Test", "2020");
            App.SplashScreen.ProductId = "VNA Test BAE System ";
            App.SplashScreen.AdditionalProductInfo = "Build Version: 1.2.3.4\nVersion String: Vector Network Analyser";

            // Sample showing how to register for the cancel request event. This will also cause the cancel button to show.
            // NOTE: Since we show various uses of it in other areas of these sample let's not set it here.
            //this.SplashScreen.CancelRequested += (sender, e) => Environment.Exit(1);

            // We require the application to explicitly show the splash screen at a time that works for them.
            App.SplashScreen.ShowSplashScreen();
        }

        private static void DemonstrateSplashScreenFeatures(SplashScreenDemoType splashScreenDemoType)
        {
            switch (splashScreenDemoType)
            {
                case SplashScreenDemoType.Simple:
                    // This is a simple usage scenario:
                    // This shows a bare-bones splash screen where all we do is wait for 5 seconds and then close it.
                    // There is no use of the status, progress, or cancel request features.
                    Thread.Sleep(5000);
                    break;

                case SplashScreenDemoType.Intermediate:
                    // This is an intermediate usage scenario:
                    // This is also a simple scenario but we do show the "contentrendered" event on the main window to trigger 
                    // the closing of the splash screen. We also show the usage of the SetStatusMessage() call to completely "set" 
                    // the text in the status window. This scenario shows simple use of the progressbar as well.

                    // Just show something in the status window as an example
                    App.SplashScreen.SetStatusMessage("Starting application...");
                    App.SplashScreen.ProgressBarPercentage = .5;

                    // We then simulate the main app startup "work" by putting a sleep in
                    Thread.Sleep(5000);

                    // Using SetStatusMessage will set the contents of the entire window to this. No appending of text.
                    App.SplashScreen.SetStatusMessage("Application startup finished...");
                    App.SplashScreen.ProgressBarPercentage = 1;

                    Thread.Sleep(2000);
                    break;

                case SplashScreenDemoType.Advanced:
                    //Here we are going to simulate real work and use of the progress bar & status window.
                    //mainWindow.Loaded += DemonstrateAdvancedSplashScreenProgress;
                    DemonstrateAdvancedSplashScreenProgress(null, null);
                    break;
            }
        }

        private static void DemonstrateAdvancedSplashScreenProgress(object sender, RoutedEventArgs e)
        {
            // Exit will be called when the cancel requested feature is invoked (usually the consumer just clicking on the cancel button).
            // The only guarantee is that the splash screen window will be closed and disposed of but any other handling, including 
            // shutting down the application, is to be handled by you (the caller).
            App.SplashScreen.CancelRequested += (senderobject, sourceargs) => Environment.Exit(1);

            // Some sample code of how to move the progress bar and the status window
            // NOTE: The visibility of both the progress bar and the status window will remain as "Collapsed" until you set them to something -
            // in other words, neither will be "present" in the layout or render pass until you use them.
            const int sleepTime = 100;
            const int total = 5000;
            const int maxCount = total / 100;
            int count = 0;
            short skip = 0;
            StringReader sampleBootText = GetSampleBootText();

            while (count < maxCount)
            {
                count++;

                // Read in some sample text
                if (skip++ >= 4)
                {
                    skip = 0;

                    // Just a sample of how to send additional text to the status window. Just set the property to some text and it will be appended.
                    // Let's throw up some sample text until we run out and then throw up the percentage
                    string statusMessage = null;
                    if (sampleBootText != null && ((statusMessage = sampleBootText.ReadLine()) != null))
                    {
                        App.SplashScreen.AppendStatusMessage(statusMessage);
                    }
                    else
                    {
                        App.SplashScreen.AppendStatusMessage(((count * sleepTime / (double)total) * 100).ToString("F0") + " %");
                    }
                }
                
                // Set the progress bar to some absolute value
                App.SplashScreen.ProgressBarPercentage = count/50.0;
                
                // We're doing a thread sleep here to simulate work but you'd be doing real startup work.
                Thread.Sleep(sleepTime);
            }
        }

        private static StringReader GetSampleBootText()
        {
            string bootLogText = "";
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Keysight.Ccl.Wsl.Samples.SplashScreenSample.VNABootLog.txt"))
            using (var streamReader = new StreamReader(stream))
            {
                bootLogText = streamReader.ReadToEnd();
            }

            var reader = new StringReader(bootLogText);
            return reader;
        }
    }
}
