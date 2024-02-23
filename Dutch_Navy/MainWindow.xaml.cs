using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Keysight.Ccl.Wsl.UI;
using System;
using System.IO;
using System.Runtime.InteropServices;
using Ivi.Visa.Interop;
using System.Threading;

namespace Keysight.Ccl.Wsl.Samples.SplashScreenSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : WslMainWindow
    {

        public MainWindow()
        {
            UXManager.Initialize(StandardSkinNames.Bistra);
            UXManager.ColorScheme = "Caranu Dark";
           
            InitializeComponent();

        }
        private void FileExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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

        private void AddTexToLogPanel(string logLine)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {

                LogPanel.AppendText(logLine + Environment.NewLine);

            });
           
        }
        private void button_Click(object sender, RoutedEventArgs e)
        {
            string VISA_Address =     VISA_AddressTextBox.Text;
            string  LO1_Freq = LO1.Text;

            LogPanel.Text = string.Empty;
           
            ResourceManager ioMgr = new ResourceManager();

            FormattedIO488 instrument = new FormattedIO488();

            //Open the VISA session using a socket port:
            //  VSA on a PC:                        Port=5025 
            //  VSA on an X-Series Signal Analyzer: Port=5024
            IVisaSession session = null;
            try
            {
                session = ioMgr.Open((VISA_Address), AccessMode.NO_LOCK, 3000, "");
            }
            catch (COMException ex)
            {
               AddTexToLogPanel("Failed to connect to the VSA SCPI interface.");
               AddTexToLogPanel("Check Network Analyser Application Is Running");
               AddTexToLogPanel("Ensure there is a connection TCPIP0::LOCALHOST::5025:SOCKET");
               AddTexToLogPanel("Open Keysight Connection Expert");
               AddTexToLogPanel("Add LAN Interface");
               AddTexToLogPanel("Select Add Address");
               AddTexToLogPanel("Input LocalHost as Host Name");
               AddTexToLogPanel("Select Socket Radio Button");
               AddTexToLogPanel("Verify Connection");
               AddTexToLogPanel(ex.Message);

               AddTexToLogPanel("Press enter to exit demo");
                Console.ReadLine();

                return;
            }

            instrument.IO = (IMessage)session;
            instrument.IO.SendEndEnabled = false;
            instrument.IO.Timeout = 5000;                       //in milliseconds
            instrument.IO.TerminationCharacterEnabled = true;   //Defaults to false            

            //Print the Identification string of the VSA program instance
            instrument.WriteString("*IDN?");
            string idnString = instrument.ReadString();
            AddTexToLogPanel("Network Analyser: " + idnString);

            instrument.WriteString("SYST:FPReset");
            AddTexToLogPanel("Factory Preset Completed");

            Thread.Sleep(100);

            instrument.WriteString("DISPlay:WINDow1:STATE ON");
            AddTexToLogPanel("Display Window 1 On");

            Thread.Sleep(100);

            instrument.WriteString("DISPlay:WINDow2:STATE ON");
            AddTexToLogPanel("Display Window 2 On");

            Thread.Sleep(200);

            instrument.WriteString("DISP:ARR LTOR");
            AddTexToLogPanel("Windows Displayed Side By Side");

            double L01_Freqd = double.Parse(LO1_Freq);

            double LO1Stop_ch1 = 0;  
            double L01Start_ch1 = 0;
            double L01Start_ch2 = 0;
            double L01Stop_ch2 = 0;
            double LO2 = 60000000;
            L01_Freqd = L01_Freqd * 1000000;


            if (L01_Freqd == 9350000000)        

            {
                AddTexToLogPanel("XBand Selected");
                 LO1Stop_ch1 = L01_Freqd - 9000;
                 L01Start_ch1 = L01_Freqd - 20000000;
                 L01Start_ch2 = L01_Freqd + 9000;
                 L01Stop_ch2 = L01_Freqd + 20000000;
            }

            else

            {
                AddTexToLogPanel("KaBand Selected");
                L01_Freqd = L01_Freqd / 2;
                LO1Stop_ch1 = L01_Freqd - 9000;
                L01Start_ch1 = L01_Freqd - 20000000;
                L01Start_ch2 = L01_Freqd + 9000;
                L01Stop_ch2 = L01_Freqd + 20000000;
            }

            instrument.WriteString("SYST:CONF:EDEV:EXIS? 'N5171B'");
            string N5171B_Status = instrument.ReadString();
            int N5171B_Statusi = int.Parse(N5171B_Status);


            if (N5171B_Statusi == 0)
            {
                AddTexToLogPanel("N5171B Does Not Exist");
                instrument.WriteString("SYSTem:CONFigure:EDEVice:ADD 'N5171B'");
                instrument.WriteString("SYST:CONF:EDEV:DTYP 'N5171B','Source'");
                instrument.WriteString("SYST:CONF:EDEV:IOEN 'N5171B', 0");
                instrument.WriteString("SYST:CONF:EDEV:STAT 'N5171B', 1");
                instrument.WriteString("Syst:conf:edev:ioconfig 'N5171B', 'gpib0::16::instr'");
                instrument.WriteString("SYST:CONF:EDEV:STAT 'N5171B', 1");

            }
            else
            {
                AddTexToLogPanel("N5171B Does Exist");
                instrument.WriteString("SYST:CONF:EDEV:IOEN 'N5171B', 0");
                instrument.WriteString("SYST:CONF:EDEV:STAT 'N5171B', 1");
                instrument.WriteString("Syst:conf:edev:ioconfig 'N5171B', 'gpib0::16::instr'");
                instrument.WriteString("SYST:CONF:EDEV:IOEN 'N5171B', 1");
            }


            //Channel1

            //Define a measurement name, parameter
            instrument.WriteString("CALCulate1:PARameter:DEFine:EXT 'ch1A4','A,4'");
            instrument.WriteString("CALCulate1:PARameter:DEFine:EXT 'ch1B4','B,4'");
            instrument.WriteString("CALCulate1:PARameter:DEFine:EXT 'ch1C4','C,4'");

            instrument.WriteString("DISPlay:WINDow1:TRACe1:FEED 'ch1A4'");
            instrument.WriteString("DISPlay:WINDow1:TRACe2:FEED 'ch1B4'");
            instrument.WriteString("DISPlay:WINDow1:TRACe3:FEED 'ch1C4'");
            instrument.WriteString("CALC1:PAR:SEL 'ch1A4'");
            instrument.WriteString("Display:window1:trace1:y:coup:method window");
            instrument.WriteString("DISP:WIND:TRAC1:Y:COUP ON");
            instrument.WriteString("DISP:WIND:TRAC1:Y:PDIV 5");
            instrument.WriteString("DISP:WIND:TRAC1:Y:RLEV -5");

            AddTexToLogPanel("Channel 1 Measurement Name and Parameter Implemented");

            //the segment to set the receiver frequency        ;
            instrument.WriteString("SENSe1:SWEep:TYPE SEGM");
            instrument.WriteString("SENS1:SEGM:ARB ON");
            instrument.WriteString("SENS1:SEGM:FREQ:STAR 20MHZ");
            instrument.WriteString("SENS1:SEGM:FREQ:STOP 9kHZ");
            instrument.WriteString("SENS1:SEGM:SWE:POIN 51");
            instrument.WriteString("SENS1:BWID 100000");
            instrument.WriteString("SOUR1:POW:COUP OFF");
            instrument.WriteString("source1:power4:mode ON");
            instrument.WriteString("source1:power5:mode ON");
            instrument.WriteString("sour1:pow 0, 'N5171B'");

            AddTexToLogPanel("Channel 1 Segment Set To Receive Frequency");

            // N5171B Configuration

            // Set the frequency offset for the internal and external source

            instrument.WriteString("SENS1:FOM:STAT ON");
            instrument.WriteString("SENS1:FOM:display:select 'Receivers'");
            instrument.WriteString("sense1:fom:rnum? 'Source P3-4'");
            string RangeInternalSource = instrument.ReadString();
            instrument.WriteString("sense1:fom:rnum? 'N5171B'");
            string Source60M = instrument.ReadString();
            char ris = RangeInternalSource[1];
            char s60m = Source60M[1];

            AddTexToLogPanel("Channel 1 Set the frequency offset for the internal and external source");

            // LO Frequency Setting

            instrument.WriteString("SENS1:FOM:RANGe"+ris+":COUP 0");
            instrument.WriteString("SENS1:FOM:RANG"+ris+":sweep:type linear");

            AddTexToLogPanel("Channel 1 LO Frequency Set");

            // LO Xband

            instrument.WriteString("SENS1:FOM:RANG" + ris + ":FREQUENCY:START " + L01Start_ch1);
            instrument.WriteString("SENS1:FOM:RANG" + ris + ":FREQUENCY:STop " + LO1Stop_ch1);
            instrument.WriteString("SENS1:FOM:RANGe" + s60m + ":COUP 0");
            instrument.WriteString("SENS1:FOM:RANGe" + s60m + ":sweep:type CW");
            instrument.WriteString("SENS1:FOM:RANGe" + s60m + ":frequency:CW " + LO2);

            AddTexToLogPanel("Channel 1 LO XBand Frequency Set");

            // Channel 2

            // Define a measurement name, parameter

            instrument.WriteString("CALCulate2:PARameter:DEFine:EXT 'ch2A4','A,4'");
            instrument.WriteString("CALCulate2:PARameter:DEFine:EXT 'ch2B4','B,4'");
            instrument.WriteString("CALCulate2:PARameter:DEFine:EXT 'ch2C4','C,4'");
            Thread.Sleep(500);
            instrument.WriteString("DISPlay:WINDow2:TRACe4:FEED 'ch2A4'");
            Thread.Sleep(500);
            instrument.WriteString("DISPlay:WINDow2:TRACe5:FEED 'ch2B4'");
            Thread.Sleep(500);
            instrument.WriteString("DISPlay:WINDow2:TRACe6:FEED 'ch2C4'");
            instrument.WriteString("CALC2:PAR:SEL 'ch2A4'");
            instrument.WriteString("Display:window2:trace4:y:coup:method window");
            instrument.WriteString("DISP:WIND2:TRAC4:Y:COUP ON");
            instrument.WriteString("DISP:WIND2:TRAC4:Y:PDIV 5");
            instrument.WriteString("DISP:WIND2:TRAC4:Y:RLEV -5");

            AddTexToLogPanel("Channel 2 Measurement Name and Parameter Implemented");

            // configure the segment to set the receiver frequency
            instrument.WriteString("SENSe2:SWEep:TYPE SEGM");
            instrument.WriteString("SENS2:SEGM:ARB ON");
            instrument.WriteString("SENS2:SEGM:FREQ:STAR 9kHz");
            instrument.WriteString("SENS2:SEGM:FREQ:STOP 20MHz");
            instrument.WriteString("SENS2:SEGM:SWE:POIN 51");
            instrument.WriteString("SENS2:BWID 100000");
            instrument.WriteString("SOUR2:POW:COUP OFF");
            instrument.WriteString("source2:power4:mode ON");
            instrument.WriteString("source2:power5:mode ON");
            instrument.WriteString("sour2:pow 0, 'N5171B'");

            AddTexToLogPanel("Channel 2 Segment Set To Receive Frequency");

            // Set the frequency offset for the internal and external source

            instrument.WriteString("SENS2:FOM:STAT ON");
            instrument.WriteString("SENS2:FOM:display:select 'Receivers'");
            instrument.WriteString("sense2:fom:rnum? 'Source P3-4'");
            RangeInternalSource = instrument.ReadString();
            instrument.WriteString("sense2:fom:rnum? 'N5171B'");
            Source60M = instrument.ReadString();
            ris = RangeInternalSource[1];
            s60m = Source60M[1];

            AddTexToLogPanel("Channel 2 Set the frequency offset for the internal and external source");

            // LO Frequency Setting

            instrument.WriteString("SENS2:FOM:RANGe" + ris + ":COUP 0");
            instrument.WriteString("SENS2:FOM:RANG" + ris + ":sweep:type linear");

            AddTexToLogPanel("Channel 2 LO Frequency Set");

            // LO Xband

            instrument.WriteString("SENS2:FOM:RANG" + ris + ":FREQUENCY:START " + L01Start_ch2);
            instrument.WriteString("SENS2:FOM:RANG" + ris + ":FREQUENCY:STop " + L01Stop_ch2);
            instrument.WriteString("SENS2:FOM:RANGe" + s60m + ":COUP 0");
            instrument.WriteString("SENS2:FOM:RANGe" + s60m + ":sweep:type CW");
            instrument.WriteString("SENS2:FOM:RANGe" + s60m + ":frequency:CW " + LO2);

            AddTexToLogPanel("Channel 2 LO XBand Frequency Set");

            instrument.WriteString("SYST:ERR?");
            string ERRString = instrument.ReadString();
            char character = ERRString[1];

            do
            {

                if (character == 48)
                {
                    AddTexToLogPanel("System OK " + ERRString);
                }

                else
                {
                    AddTexToLogPanel("System Error " + ERRString);
                }

            }

            while (character != 48);

            try
            {
                    //string result = instrument.ReadString();
                    //Sweep completed successfully
                }
                catch (COMException ex)
                {
                    if (ex.Message.Contains("timeout"))
                    {
                        //A timeout occurred prior to the sweep's completion
                        //Consider setting a longer timeout period or avoid using *OPC? and
                        //instead use :MEAS:DONE? in a polling loop to poll when the measurement is done.
                        AddTexToLogPanel("Timeout occurred while waiting for measurement results.");
                        AddTexToLogPanel("Timeout period was set to " + instrument.IO.Timeout + " milliseconds");
                    }
                    else
                    {
                        AddTexToLogPanel("An unhandled IO exception occurred while waiting for measurement results.");

                    }
                    instrument.IO.Close();
                    return;
                }     

            AddTexToLogPanel("Select File Exit to exit demo");       

            //Close the connection         
            instrument.IO.Close();


        

    }


    }

}




