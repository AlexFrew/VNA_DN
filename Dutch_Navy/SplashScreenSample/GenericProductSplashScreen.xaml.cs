//-----------------------------------------------------------------------
// <copyright file="GenericProductSplashScreen.cs" company="Keysight Technologies">
//      Copyright (C) 2014 Keysight Technologies
// </copyright>
//
// Description: Code to support the example splash screen.
//-----------------------------------------------------------------------
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Keysight.Ccl.Wsl.Samples.SplashScreenSample
{
    /// <summary>
    /// Interaction logic for GenericProductSplashScreen.xaml
    /// </summary>
    public partial class GenericProductSplashScreen : UserControl
    {
        public GenericProductSplashScreen()
        {
            InitializeComponent();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("The splash screen user control was unloaded.");
        }
    }
}
