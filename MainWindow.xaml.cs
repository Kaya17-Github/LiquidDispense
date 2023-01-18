using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Converters;
using System.Threading;
using System.IO;
using System.Data;
using System.Media;

using NationalInstruments;
using NationalInstruments.DAQmx;
using Task = System.Threading.Tasks.Task;
using System.Runtime.InteropServices;

namespace LiquidDispense
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport(@".\DLLs\DAQinterfaceForKaya17.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr verifyInput(double[] input);

        [DllImport(@".\DLLs\DAQinterfaceForKaya17.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern bool testSetSettings(double[] input);

        [DllImport(@".\DLLs\DAQinterfaceForKaya17.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern bool testInitializeBoard(StringBuilder str, int len);

        [DllImport(@".\DLLs\DAQinterfaceForKaya17.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr testGetBoardValue(StringBuilder str, int len, double excVoltage);

        [DllImport(@".\DLLs\DAQinterfaceForKaya17.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void testCloseTasksAndChannels();

        string[] devices; 
        string[] getChannels;
        string test;
        string test2;
        string test3;
        readonly string[] shiftAndScale;
        int wait;
        double raw_avg;

        public MainWindow()
        {
            devices = DaqSystem.Local.Devices;

            for (int i = 0; i < devices.Length; i++)
            {
                Console.WriteLine(devices[i]);
            }

            getChannels = DaqSystem.Local.GetPhysicalChannels(PhysicalChannelTypes.DOPort, PhysicalChannelAccess.External);

            for (int i = 0; i < getChannels.Length; i++)
            {
                Console.WriteLine(getChannels[i]);
            }

            test = getChannels[0];
            Console.WriteLine(test);

            test2 = getChannels[1];
            Console.WriteLine(test2);

            test3 = getChannels[2];
            Console.WriteLine(test3);

            shiftAndScale = File.ReadAllLines(@"C:\Users\Public\Documents\kaya17\bin\Kaya17_32Well_Shift_Scale.csv");

            wait = 1;

            InitializeComponent();
        }

        private void xStepForward_btn_Click(object sender, RoutedEventArgs e)
        {
            int numSteps = int.Parse(numSteps_tb.Text);

            for (int i = 0; i < numSteps; i++)
            {
                using (NationalInstruments.DAQmx.Task digitalWriteTask = new NationalInstruments.DAQmx.Task())
                {
                    //  Create an Digital Output channel and name it.
                    digitalWriteTask.DOChannels.CreateChannel(test, "port0",
                        ChannelLineGrouping.OneChannelForAllLines);

                    //  Write digital port data. WriteDigitalSingChanSingSampPort writes a single sample
                    //  of digital data on demand, so no timeout is necessary.
                    DigitalSingleChannelWriter writer = new DigitalSingleChannelWriter(digitalWriteTask.Stream);
                    writer.WriteSingleSamplePort(true, 32);
                    //Thread.Sleep(wait);
                    writer.WriteSingleSamplePort(true, 48);
                }
            }
        }

        private void xStepBack_btn_Click(object sender, RoutedEventArgs e)
        {
            int numSteps = int.Parse(numSteps_tb.Text);

            for (int i = 0; i < numSteps; i++)
            {
                using (NationalInstruments.DAQmx.Task digitalWriteTask = new NationalInstruments.DAQmx.Task())
                {
                    //  Create an Digital Output channel and name it.
                    digitalWriteTask.DOChannels.CreateChannel(test, "port0",
                        ChannelLineGrouping.OneChannelForAllLines);

                    //  Write digital port data. WriteDigitalSingChanSingSampPort writes a single sample
                    //  of digital data on demand, so no timeout is necessary.
                    DigitalSingleChannelWriter writer = new DigitalSingleChannelWriter(digitalWriteTask.Stream);
                    writer.WriteSingleSamplePort(true, 16);
                    //Thread.Sleep(wait);
                    writer.WriteSingleSamplePort(true, 0);
                }

                using (NationalInstruments.DAQmx.Task digitalReadTask = new NationalInstruments.DAQmx.Task())
                {
                    digitalReadTask.DIChannels.CreateChannel(
                        test2,
                        "port1",
                        ChannelLineGrouping.OneChannelForAllLines);

                    DigitalSingleChannelReader reader = new DigitalSingleChannelReader(digitalReadTask.Stream);
                    UInt32 data = reader.ReadSingleSamplePortUInt32();

                    //Update the Data Read box
                    string limitInputText = data.ToString();

                    if (limitInputText == "1" || limitInputText == "3" || limitInputText == "5" || limitInputText == "7")
                    {
                        //MessageBox.Show("X limit reached");
                        for (int j = 0; j < 100; j++)
                        {
                            using (NationalInstruments.DAQmx.Task digitalWriteTask = new NationalInstruments.DAQmx.Task())
                            {
                                //  Create an Digital Output channel and name it.
                                digitalWriteTask.DOChannels.CreateChannel(test, "port0",
                                    ChannelLineGrouping.OneChannelForAllLines);

                                //  Write digital port data. WriteDigitalSingChanSingSampPort writes a single sample
                                //  of digital data on demand, so no timeout is necessary.
                                DigitalSingleChannelWriter writer = new DigitalSingleChannelWriter(digitalWriteTask.Stream);
                                writer.WriteSingleSamplePort(true, 32);
                                //Thread.Sleep(wait);
                                writer.WriteSingleSamplePort(true, 48);
                            }
                        }
                        break;
                    }
                }
            }            
        }

        private void yStepForward_btn_Click(object sender, RoutedEventArgs e)
        {
            int numSteps = int.Parse(numSteps_tb.Text);

            for (int i = 0; i < numSteps; i++)
            {
                using (NationalInstruments.DAQmx.Task digitalWriteTask = new NationalInstruments.DAQmx.Task())
                {
                    //  Create an Digital Output channel and name it.
                    digitalWriteTask.DOChannels.CreateChannel(test, "port0",
                        ChannelLineGrouping.OneChannelForAllLines);

                    //  Write digital port data. WriteDigitalSingChanSingSampPort writes a single sample
                    //  of digital data on demand, so no timeout is necessary.
                    DigitalSingleChannelWriter writer = new DigitalSingleChannelWriter(digitalWriteTask.Stream);
                    writer.WriteSingleSamplePort(true, 12);
                    //Thread.Sleep(wait);
                    writer.WriteSingleSamplePort(true, 8);
                }
            }
        }

        private void yStepBack_btn_Click(object sender, RoutedEventArgs e)
        {
            int numSteps = int.Parse(numSteps_tb.Text);

            for (int i = 0; i < numSteps; i++)
            {
                using (NationalInstruments.DAQmx.Task digitalWriteTask = new NationalInstruments.DAQmx.Task())
                {
                    //  Create an Digital Output channel and name it.
                    digitalWriteTask.DOChannels.CreateChannel(test, "port0",
                        ChannelLineGrouping.OneChannelForAllLines);

                    //  Write digital port data. WriteDigitalSingChanSingSampPort writes a single sample
                    //  of digital data on demand, so no timeout is necessary.
                    DigitalSingleChannelWriter writer = new DigitalSingleChannelWriter(digitalWriteTask.Stream);
                    writer.WriteSingleSamplePort(true, 4);
                    //Thread.Sleep(wait);
                    writer.WriteSingleSamplePort(true, 0);
                }

                using (NationalInstruments.DAQmx.Task digitalReadTask = new NationalInstruments.DAQmx.Task())
                {
                    digitalReadTask.DIChannels.CreateChannel(
                        test2,
                        "port1",
                        ChannelLineGrouping.OneChannelForAllLines);

                    DigitalSingleChannelReader reader = new DigitalSingleChannelReader(digitalReadTask.Stream);
                    UInt32 data = reader.ReadSingleSamplePortUInt32();

                    //Update the Data Read box
                    string limitInputText = data.ToString();

                    if (limitInputText == "2" || limitInputText == "3" || limitInputText == "6" || limitInputText == "7")
                    {
                        //MessageBox.Show("Y limit reached");
                        for (int j = 0; j < 500; j++)
                        {
                            using (NationalInstruments.DAQmx.Task digitalWriteTask = new NationalInstruments.DAQmx.Task())
                            {
                                //  Create an Digital Output channel and name it.
                                digitalWriteTask.DOChannels.CreateChannel(test, "port0",
                                    ChannelLineGrouping.OneChannelForAllLines);

                                //  Write digital port data. WriteDigitalSingChanSingSampPort writes a single sample
                                //  of digital data on demand, so no timeout is necessary.
                                DigitalSingleChannelWriter writer = new DigitalSingleChannelWriter(digitalWriteTask.Stream);
                                writer.WriteSingleSamplePort(true, 12);
                                //Thread.Sleep(wait);
                                writer.WriteSingleSamplePort(true, 8);
                            }
                        }
                        break;
                    }
                }
            }
        }

        private void lowerZ_btn_Click(object sender, RoutedEventArgs e)
        {
            int numSteps = int.Parse(numSteps_tb.Text);

            for (int i = 0; i < numSteps; i++)
            {
                using (NationalInstruments.DAQmx.Task digitalWriteTask = new NationalInstruments.DAQmx.Task())
                {
                    //  Create an Digital Output channel and name it.
                    digitalWriteTask.DOChannels.CreateChannel(test, "port0",
                        ChannelLineGrouping.OneChannelForAllLines);

                    //  Write digital port data. WriteDigitalSingChanSingSampPort writes a single sample
                    //  of digital data on demand, so no timeout is necessary.
                    DigitalSingleChannelWriter writer = new DigitalSingleChannelWriter(digitalWriteTask.Stream);
                    writer.WriteSingleSamplePort(true, 64);
                    //Thread.Sleep(wait);
                    writer.WriteSingleSamplePort(true, 0);
                }
            }
        }

        private void raiseZ_btn_Click(object sender, RoutedEventArgs e)
        {
            int numSteps = int.Parse(numSteps_tb.Text);

            for (int i = 0; i < numSteps; i++)
            {
                using (NationalInstruments.DAQmx.Task digitalWriteTask = new NationalInstruments.DAQmx.Task())
                {
                    //  Create an Digital Output channel and name it.
                    digitalWriteTask.DOChannels.CreateChannel(test, "port0",
                        ChannelLineGrouping.OneChannelForAllLines);

                    //  Write digital port data. WriteDigitalSingChanSingSampPort writes a single sample
                    //  of digital data on demand, so no timeout is necessary.
                    DigitalSingleChannelWriter writer = new DigitalSingleChannelWriter(digitalWriteTask.Stream);
                    writer.WriteSingleSamplePort(true, 192);
                    //Thread.Sleep(wait);
                    writer.WriteSingleSamplePort(true, 128);
                }

                using (NationalInstruments.DAQmx.Task digitalReadTask = new NationalInstruments.DAQmx.Task())
                {
                    digitalReadTask.DIChannels.CreateChannel(
                        test2,
                        "port1",
                        ChannelLineGrouping.OneChannelForAllLines);

                    DigitalSingleChannelReader reader = new DigitalSingleChannelReader(digitalReadTask.Stream);
                    UInt32 data = reader.ReadSingleSamplePortUInt32();

                    //Update the Data Read box
                    string limitInputText = data.ToString();

                    if (limitInputText == "4" || limitInputText == "5" || limitInputText == "6" || limitInputText == "7")
                    {
                        //MessageBox.Show("Z limit reached");
                        for (int j = 0; j < 300; j++)
                        {
                            using (NationalInstruments.DAQmx.Task digitalWriteTask = new NationalInstruments.DAQmx.Task())
                            {
                                //  Create an Digital Output channel and name it.
                                digitalWriteTask.DOChannels.CreateChannel(test, "port0",
                                    ChannelLineGrouping.OneChannelForAllLines);

                                //  Write digital port data. WriteDigitalSingChanSingSampPort writes a single sample
                                //  of digital data on demand, so no timeout is necessary.
                                DigitalSingleChannelWriter writer = new DigitalSingleChannelWriter(digitalWriteTask.Stream);
                                writer.WriteSingleSamplePort(true, 64);
                                //Thread.Sleep(1);
                                writer.WriteSingleSamplePort(true, 0);
                            }
                        }
                        break;
                    }
                }
            }
        }

        private void drawLiquid_btn_Click(object sender, RoutedEventArgs e)
        {
            int numSteps = int.Parse(numSteps_tb.Text);

            for (int i = 0; i < numSteps; i++)
            {
                using (NationalInstruments.DAQmx.Task digitalWriteTask = new NationalInstruments.DAQmx.Task())
                {
                    //  Create an Digital Output channel and name it.
                    digitalWriteTask.DOChannels.CreateChannel(test, "port0",
                        ChannelLineGrouping.OneChannelForAllLines);

                    //  Write digital port data. WriteDigitalSingChanSingSampPort writes a single sample
                    //  of digital data on demand, so no timeout is necessary.
                    DigitalSingleChannelWriter writer = new DigitalSingleChannelWriter(digitalWriteTask.Stream);
                    writer.WriteSingleSamplePort(true, 1);
                    Thread.Sleep(wait);
                    writer.WriteSingleSamplePort(true, 0);
                }
            }
        }

        private void dispense_btn_Click(object sender, RoutedEventArgs e)
        {
            int numSteps = int.Parse(numSteps_tb.Text);

            for (int i = 0; i < numSteps; i++)
            {
                using (NationalInstruments.DAQmx.Task digitalWriteTask = new NationalInstruments.DAQmx.Task())
                {
                    //  Create an Digital Output channel and name it.
                    digitalWriteTask.DOChannels.CreateChannel(test, "port0",
                        ChannelLineGrouping.OneChannelForAllLines);

                    //  Write digital port data. WriteDigitalSingChanSingSampPort writes a single sample
                    //  of digital data on demand, so no timeout is necessary.
                    DigitalSingleChannelWriter writer = new DigitalSingleChannelWriter(digitalWriteTask.Stream);
                    writer.WriteSingleSamplePort(true, 3);
                    Thread.Sleep(wait);
                    writer.WriteSingleSamplePort(true, 2);
                }
            }
        }

        private void ReadInputBtn_Click(object sender, RoutedEventArgs e)
        {
            using (NationalInstruments.DAQmx.Task digitalReadTask = new NationalInstruments.DAQmx.Task())
            {
                digitalReadTask.DIChannels.CreateChannel(
                    test2,
                    "port1",
                    ChannelLineGrouping.OneChannelForAllLines);

                DigitalSingleChannelReader reader = new DigitalSingleChannelReader(digitalReadTask.Stream);
                UInt32 data = reader.ReadSingleSamplePortUInt32();

                //Update the Data Read box
                LimitInputText.Text = data.ToString();
            }
        }

        private void startPump_btn_Click(object sender, RoutedEventArgs e)
        {
            using (NationalInstruments.DAQmx.Task digitalWriteTask = new NationalInstruments.DAQmx.Task())
            {
                //  Create an Digital Output channel and name it.
                digitalWriteTask.DOChannels.CreateChannel(test3, "port2",
                    ChannelLineGrouping.OneChannelForAllLines);

                //  Write digital port data. WriteDigitalSingChanSingSampPort writes a single sample
                //  of digital data on demand, so no timeout is necessary.
                DigitalSingleChannelWriter writer = new DigitalSingleChannelWriter(digitalWriteTask.Stream);
                writer.WriteSingleSamplePort(true, 1);
            }
            Console.WriteLine("Switched banks");
            using (NationalInstruments.DAQmx.Task digitalWriteTask = new NationalInstruments.DAQmx.Task())
            {
                //  Create an Digital Output channel and name it.
                digitalWriteTask.DOChannels.CreateChannel(test, "port0",
                    ChannelLineGrouping.OneChannelForAllLines);

                //  Write digital port data. WriteDigitalSingChanSingSampPort writes a single sample
                //  of digital data on demand, so no timeout is necessary.
                DigitalSingleChannelWriter writer = new DigitalSingleChannelWriter(digitalWriteTask.Stream);
                writer.WriteSingleSamplePort(true, 12);
            }
            Console.WriteLine("Pump on");
        }

        private void stopPump_btn_Click(object sender, RoutedEventArgs e)
        {
            using (NationalInstruments.DAQmx.Task digitalWriteTask = new NationalInstruments.DAQmx.Task())
            {
                //  Create an Digital Output channel and name it.
                digitalWriteTask.DOChannels.CreateChannel(test, "port0",
                    ChannelLineGrouping.OneChannelForAllLines);

                //  Write digital port data. WriteDigitalSingChanSingSampPort writes a single sample
                //  of digital data on demand, so no timeout is necessary.
                DigitalSingleChannelWriter writer = new DigitalSingleChannelWriter(digitalWriteTask.Stream);
                writer.WriteSingleSamplePort(true, 8);
            }
            Console.WriteLine("Pump off");
            using (NationalInstruments.DAQmx.Task digitalWriteTask = new NationalInstruments.DAQmx.Task())
            {
                //  Create an Digital Output channel and name it.
                digitalWriteTask.DOChannels.CreateChannel(test3, "port2",
                    ChannelLineGrouping.OneChannelForAllLines);

                //  Write digital port data. WriteDigitalSingChanSingSampPort writes a single sample
                //  of digital data on demand, so no timeout is necessary.
                DigitalSingleChannelWriter writer = new DigitalSingleChannelWriter(digitalWriteTask.Stream);
                writer.WriteSingleSamplePort(true, 0);
            }
            Console.WriteLine("Switched banks back");
        }

        // x forward, y forward
        private void q1Diag_btn_Click(object sender, RoutedEventArgs e)
        {
            int numSteps = int.Parse(numSteps_tb.Text);

            for (int i = 0; i < numSteps; i++)
            {
                try
                {
                    using (NationalInstruments.DAQmx.Task digitalWriteTask = new NationalInstruments.DAQmx.Task())
                    {
                        //  Create an Digital Output channel and name it.
                        digitalWriteTask.DOChannels.CreateChannel(test, "port0",
                            ChannelLineGrouping.OneChannelForAllLines);

                        //  Write digital port data. WriteDigitalSingChanSingSampPort writes a single sample
                        //  of digital data on demand, so no timeout is necessary.
                        DigitalSingleChannelWriter writer = new DigitalSingleChannelWriter(digitalWriteTask.Stream);
                        writer.WriteSingleSamplePort(true, 32);
                        Thread.Sleep(wait);
                        writer.WriteSingleSamplePort(true, 48);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                try
                {
                    using (NationalInstruments.DAQmx.Task digitalWriteTask = new NationalInstruments.DAQmx.Task())
                    {
                        //  Create an Digital Output channel and name it.
                        digitalWriteTask.DOChannels.CreateChannel(test, "port0",
                            ChannelLineGrouping.OneChannelForAllLines);

                        //  Write digital port data. WriteDigitalSingChanSingSampPort writes a single sample
                        //  of digital data on demand, so no timeout is necessary.
                        DigitalSingleChannelWriter writer = new DigitalSingleChannelWriter(digitalWriteTask.Stream);
                        writer.WriteSingleSamplePort(true, 12);
                        Thread.Sleep(wait);
                        writer.WriteSingleSamplePort(true, 8);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        // x backward, y forward
        private void q2Diag_btn_Click(object sender, RoutedEventArgs e)
        {
            int numSteps = int.Parse(numSteps_tb.Text);

            for (int i = 0; i < numSteps; i++)
            {
                try
                {
                    using (NationalInstruments.DAQmx.Task digitalWriteTask = new NationalInstruments.DAQmx.Task())
                    {
                        //  Create an Digital Output channel and name it.
                        digitalWriteTask.DOChannels.CreateChannel(test, "port0",
                            ChannelLineGrouping.OneChannelForAllLines);

                        //  Write digital port data. WriteDigitalSingChanSingSampPort writes a single sample
                        //  of digital data on demand, so no timeout is necessary.
                        DigitalSingleChannelWriter writer = new DigitalSingleChannelWriter(digitalWriteTask.Stream);
                        writer.WriteSingleSamplePort(true, 16);
                        Thread.Sleep(wait);
                        writer.WriteSingleSamplePort(true, 0);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                try
                {
                    using (NationalInstruments.DAQmx.Task digitalWriteTask = new NationalInstruments.DAQmx.Task())
                    {
                        //  Create an Digital Output channel and name it.
                        digitalWriteTask.DOChannels.CreateChannel(test, "port0",
                            ChannelLineGrouping.OneChannelForAllLines);

                        //  Write digital port data. WriteDigitalSingChanSingSampPort writes a single sample
                        //  of digital data on demand, so no timeout is necessary.
                        DigitalSingleChannelWriter writer = new DigitalSingleChannelWriter(digitalWriteTask.Stream);
                        writer.WriteSingleSamplePort(true, 12);
                        Thread.Sleep(wait);
                        writer.WriteSingleSamplePort(true, 8);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        // x backward, y backward
        private void q3Diag_btn_Click(object sender, RoutedEventArgs e)
        {
            int numSteps = int.Parse(numSteps_tb.Text);

            for (int i = 0; i < numSteps; i++)
            {
                try
                {
                    using (NationalInstruments.DAQmx.Task digitalWriteTask = new NationalInstruments.DAQmx.Task())
                    {
                        //  Create an Digital Output channel and name it.
                        digitalWriteTask.DOChannels.CreateChannel(test, "port0",
                            ChannelLineGrouping.OneChannelForAllLines);

                        //  Write digital port data. WriteDigitalSingChanSingSampPort writes a single sample
                        //  of digital data on demand, so no timeout is necessary.
                        DigitalSingleChannelWriter writer = new DigitalSingleChannelWriter(digitalWriteTask.Stream);
                        writer.WriteSingleSamplePort(true, 16);
                        Thread.Sleep(wait);
                        writer.WriteSingleSamplePort(true, 0);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                try
                {
                    using (NationalInstruments.DAQmx.Task digitalWriteTask = new NationalInstruments.DAQmx.Task())
                    {
                        //  Create an Digital Output channel and name it.
                        digitalWriteTask.DOChannels.CreateChannel(test, "port0",
                            ChannelLineGrouping.OneChannelForAllLines);

                        //  Write digital port data. WriteDigitalSingChanSingSampPort writes a single sample
                        //  of digital data on demand, so no timeout is necessary.
                        DigitalSingleChannelWriter writer = new DigitalSingleChannelWriter(digitalWriteTask.Stream);
                        writer.WriteSingleSamplePort(true, 4);
                        Thread.Sleep(wait);
                        writer.WriteSingleSamplePort(true, 0);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        // x forward, y backward
        private void q4Diag_btn_Click(object sender, RoutedEventArgs e)
        {
            int numSteps = int.Parse(numSteps_tb.Text);

            for (int i = 0; i < numSteps; i++)
            {
                try
                {
                    using (NationalInstruments.DAQmx.Task digitalWriteTask = new NationalInstruments.DAQmx.Task())
                    {
                        //  Create an Digital Output channel and name it.
                        digitalWriteTask.DOChannels.CreateChannel(test, "port0",
                            ChannelLineGrouping.OneChannelForAllLines);

                        //  Write digital port data. WriteDigitalSingChanSingSampPort writes a single sample
                        //  of digital data on demand, so no timeout is necessary.
                        DigitalSingleChannelWriter writer = new DigitalSingleChannelWriter(digitalWriteTask.Stream);
                        writer.WriteSingleSamplePort(true, 32);
                        Thread.Sleep(wait);
                        writer.WriteSingleSamplePort(true, 48);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                try
                {
                    using (NationalInstruments.DAQmx.Task digitalWriteTask = new NationalInstruments.DAQmx.Task())
                    {
                        //  Create an Digital Output channel and name it.
                        digitalWriteTask.DOChannels.CreateChannel(test, "port0",
                            ChannelLineGrouping.OneChannelForAllLines);

                        //  Write digital port data. WriteDigitalSingChanSingSampPort writes a single sample
                        //  of digital data on demand, so no timeout is necessary.
                        DigitalSingleChannelWriter writer = new DigitalSingleChannelWriter(digitalWriteTask.Stream);
                        writer.WriteSingleSamplePort(true, 4);
                        Thread.Sleep(wait);
                        writer.WriteSingleSamplePort(true, 0);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        class AutoClosingMessageBox
        {
            System.Threading.Timer _timeoutTimer;
            string _caption;
            AutoClosingMessageBox(string text, string caption, int timeout)
            {
                _caption = caption;
                _timeoutTimer = new System.Threading.Timer(OnTimerElapsed,
                    null, timeout, System.Threading.Timeout.Infinite);
                MessageBox.Show(text, caption);
            }
            public static void Show(string text, string caption, int timeout)
            {
                new AutoClosingMessageBox(text, caption, timeout);
            }
            void OnTimerElapsed(object state)
            {
                IntPtr mbWnd = FindWindow(null, _caption);
                if (mbWnd != IntPtr.Zero)
                    SendMessage(mbWnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                _timeoutTimer.Dispose();
            }
            const int WM_CLOSE = 0x0010;
            [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
            static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
            [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
            static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);
        }

        private void soundTest_btn_Click(object sender, RoutedEventArgs e)
        {
            MediaPlayer sound1 = new MediaPlayer();
            sound1.Open(new Uri(@"C:\Users\seaot\Downloads\43536__mkoenig__ultra-dnb-loop-160bpm.wav"));
            sound1.Play();
        }

        private void readTest_btn_Click(object sender, RoutedEventArgs e)
        {
            //double[] testArray = new double[17];

            //// read parameter file and read in all necessary parameters
            //string[] parameters = File.ReadAllLines(@"C:\Users\Public\Documents\kaya17\bin\Kaya17Covi2V.txt");
            //double ledOutputRange = double.Parse(parameters[0].Substring(0, 3));
            //double numSamplesPerReading = double.Parse(parameters[1].Substring(0, 3));
            //double numTempSamplesPerReading = double.Parse(parameters[2].Substring(0, 2));
            //double samplingRate = double.Parse(parameters[3].Substring(0, 5));
            //double numSamplesForAvg = double.Parse(parameters[4].Substring(0, 3));
            //double errorLimitInMillivolts = double.Parse(parameters[5].Substring(0, 2));
            //double saturation = double.Parse(parameters[8].Substring(0, 8));
            //double expectedDarkRdg = double.Parse(parameters[6].Substring(0, 4));
            //double lowSignal = double.Parse(parameters[29].Substring(0, 5));
            //double readMethod = double.Parse(parameters[9].Substring(0, 1));
            //double ledOnDuration = double.Parse(parameters[9].Substring(4, 3));
            //double readDelayInMS = double.Parse(parameters[9].Substring(8, 1));
            //double excitationLedVoltage = double.Parse(parameters[33].Substring(0, 5));
            //double excMinVoltage = double.Parse(parameters[26].Substring(0, 3));
            //double excNomVoltage = double.Parse(parameters[25].Substring(0, 4));
            //double excMaxVoltage = double.Parse(parameters[24].Substring(0, 3));
            //double calMinVoltage = double.Parse(parameters[18].Substring(0, 3));
            //double calNomVoltage = double.Parse(parameters[17].Substring(0, 3));
            //double calMaxVoltage = double.Parse(parameters[16].Substring(0, 3));

            //testArray[0] = ledOutputRange;
            //testArray[1] = samplingRate;
            //testArray[2] = numSamplesPerReading;
            //testArray[3] = numSamplesForAvg;
            //testArray[4] = errorLimitInMillivolts;
            //testArray[5] = numTempSamplesPerReading;
            //testArray[6] = saturation;
            //testArray[7] = expectedDarkRdg;
            //testArray[8] = lowSignal;
            //testArray[9] = ledOnDuration;
            //testArray[10] = readDelayInMS;
            //testArray[11] = calMinVoltage;
            //testArray[12] = calNomVoltage;
            //testArray[13] = calMaxVoltage;
            //testArray[14] = excMinVoltage;
            //testArray[15] = excNomVoltage;
            //testArray[16] = excMaxVoltage;

            //string testString = "";
            //foreach (double value in testArray)
            //{
            //    testString += "Input: " + value + "\n";
            //}

            //MessageBox.Show(testString);

            //IntPtr testPtr = verifyInput(testArray);
            //double[] testArray2 = new double[17];
            //Marshal.Copy(testPtr, testArray2, 0, 17);
            //testString = "";
            //foreach (double value in testArray2)
            //{
            //    testString += "Verify input: " + value + "\n";
            //}

            //MessageBox.Show(testString);

            //bool settingsBool = testSetSettings(testArray);

            //MessageBox.Show("Test setSettings: " + settingsBool);

            //StringBuilder sb = new StringBuilder(5000);
            //bool initializeBoardBool = testInitializeBoard(sb, sb.Capacity);

            //MessageBox.Show("Test initializeBoard: " + initializeBoardBool + "\n" + sb.ToString());

            //MessageBox.Show("Insert Cartridge and then click ok");

            //StringBuilder sb2 = new StringBuilder(10000);

            //IntPtr testBoardValuePtr = testGetBoardValue(sb2, sb2.Capacity);
            //double[] testArray3 = new double[5];
            //Marshal.Copy(testBoardValuePtr, testArray3, 0, 5);
            //testString = "";
            //testString += "Return Value: m_dAvgValue = " + testArray3[0] + "\n";
            //testString += "Return Value: m_dCumSum = " + testArray3[1] + "\n";
            //testString += "Return Value: m_dLEDtmp = " + testArray3[2] + "\n";
            //testString += "Return Value: m_dPDtmp = " + testArray3[3] + "\n";
            //testString += "Return Value: testGetBoardValue = " + testArray3[4] + "\n";

            //MessageBox.Show(testString + sb2.ToString());

            //testCloseTasksAndChannels();
        }

        private void readNextTest_btn_Click(object sender, RoutedEventArgs e)
        {
            //string testString = "";

            //StringBuilder sb2 = new StringBuilder(10000);

            //IntPtr testBoardValuePtr = testGetBoardValue(sb2, sb2.Capacity);
            //double[] testArray3 = new double[5];
            //Marshal.Copy(testBoardValuePtr, testArray3, 0, 5);
            //testString = "";
            //testString += "Return Value: m_dAvgValue = " + testArray3[0] + "\n";
            //testString += "Return Value: m_dCumSum = " + testArray3[1] + "\n";
            //testString += "Return Value: m_dLEDtmp = " + testArray3[2] + "\n";
            //testString += "Return Value: m_dPDtmp = " + testArray3[3] + "\n";
            //testString += "Return Value: testGetBoardValue = " + testArray3[4] + "\n";

            //MessageBox.Show(testString + sb2.ToString());

            //testCloseTasksAndChannels();
        }

        private void centerTest_btn_Click(object sender, RoutedEventArgs e)
        {
            double[] testArray = new double[17];

            // read parameter file and read in all necessary parameters
            string[] parameters = File.ReadAllLines(@"C:\Users\Public\Documents\kaya17\bin\Kaya17Covi2V.txt");
            double ledOutputRange = double.Parse(parameters[0].Substring(0, 3));
            double numSamplesPerReading = double.Parse(parameters[1].Substring(0, 3));
            double numTempSamplesPerReading = double.Parse(parameters[2].Substring(0, 2));
            double samplingRate = double.Parse(parameters[3].Substring(0, 5));
            double numSamplesForAvg = double.Parse(parameters[4].Substring(0, 3));
            double errorLimitInMillivolts = double.Parse(parameters[5].Substring(0, 2));
            double saturation = double.Parse(parameters[8].Substring(0, 8));
            double expectedDarkRdg = double.Parse(parameters[6].Substring(0, 4));
            double lowSignal = double.Parse(parameters[29].Substring(0, 5));
            double readMethod = double.Parse(parameters[9].Substring(0, 1));
            double ledOnDuration = double.Parse(parameters[9].Substring(4, 3));
            double readDelayInMS = double.Parse(parameters[9].Substring(8, 1));
            double excitationLedVoltage = double.Parse(parameters[33].Substring(0, 5));
            double excMinVoltage = double.Parse(parameters[26].Substring(0, 3));
            double excNomVoltage = double.Parse(parameters[25].Substring(0, 4));
            double excMaxVoltage = double.Parse(parameters[24].Substring(0, 3));
            double calMinVoltage = double.Parse(parameters[18].Substring(0, 3));
            double calNomVoltage = double.Parse(parameters[17].Substring(0, 3));
            double calMaxVoltage = double.Parse(parameters[16].Substring(0, 3));
            double afeScaleFactor = double.Parse(parameters[35].Substring(0, 3));
            double afeShiftFactor = double.Parse(parameters[36].Substring(0, 4));
            double viralCountScaleFactor = double.Parse(parameters[45].Substring(0, 5));
            double viralCountOffsetFactor = double.Parse(parameters[46].Substring(0, 4));
            double antigenCutoffFactor = double.Parse(parameters[47].Substring(0, 1));
            double antigenNoiseMargin = double.Parse(parameters[48].Substring(0, 1));
            double antigenControlMargin = double.Parse(parameters[49].Substring(0, 2));

            testArray[0] = ledOutputRange;
            testArray[1] = samplingRate;
            testArray[2] = numSamplesPerReading;
            testArray[3] = numSamplesForAvg;
            testArray[4] = errorLimitInMillivolts;
            testArray[5] = numTempSamplesPerReading;
            testArray[6] = saturation;
            testArray[7] = expectedDarkRdg;
            testArray[8] = lowSignal;
            testArray[9] = ledOnDuration;
            testArray[10] = readDelayInMS;
            testArray[11] = calMinVoltage;
            testArray[12] = calNomVoltage;
            testArray[13] = calMaxVoltage;
            testArray[14] = excMinVoltage;
            testArray[15] = excNomVoltage;
            testArray[16] = excMaxVoltage;

            string testString = "";
            foreach (double value in testArray)
            {
                testString += "Input: " + value + "\n";
            }

            //MessageBox.Show(testString);

            IntPtr testPtr = verifyInput(testArray);
            double[] testArray2 = new double[17];
            Marshal.Copy(testPtr, testArray2, 0, 17);
            testString = "";
            foreach (double value in testArray2)
            {
                testString += "Verify input: " + value + "\n";
            }

            //MessageBox.Show(testString);

            bool settingsBool = testSetSettings(testArray);

            //MessageBox.Show("Test setSettings: " + settingsBool);

            double[] shiftFactors = new double[shiftAndScale.Length];
            double[] scaleFactors = new double[shiftAndScale.Length];

            for (int i = 1; i < shiftAndScale.Length; i++)
            {
                shiftFactors[i] = double.Parse(shiftAndScale[i].Split(',')[1]);
                scaleFactors[i] = double.Parse(shiftAndScale[i].Split(',')[2]);
            }

            StringBuilder sb = new StringBuilder(5000);
            bool initializeBoardBool = testInitializeBoard(sb, sb.Capacity);

            //MessageBox.Show("Test initializeBoard: " + initializeBoardBool + "\n" + sb.ToString());

            //MessageBox.Show("Insert Cartridge and then click ok");

            StringBuilder sbC1 = new StringBuilder(100000);

            IntPtr testBoardValuePtrC1 = testGetBoardValue(sbC1, sbC1.Capacity, excitationLedVoltage);
            double[] testArrayC1 = new double[5];
            Marshal.Copy(testBoardValuePtrC1, testArrayC1, 0, 5);
            string inputStringC1 = "";
            inputStringC1 += "Return Value: m_dAvgValue = " + testArrayC1[0] + "\n";
            inputStringC1 += "Return Value: m_dCumSum = " + testArrayC1[1] + "\n";
            inputStringC1 += "Return Value: m_dLEDtmp = " + testArrayC1[2] + "\n";
            inputStringC1 += "Return Value: m_dPDtmp = " + testArrayC1[3] + "\n";
            inputStringC1 += "Return Value: testGetBoardValue = " + testArrayC1[4] + "\n";

            raw_avg = (testArrayC1[0] - shiftFactors[11]) * scaleFactors[11];
            raw_avg = Math.Round(raw_avg, 3);

            MessageBox.Show("First reading value: " + raw_avg.ToString());

            //move right 20
            for (int i = 0; i < 20; i++)
            {
                using (NationalInstruments.DAQmx.Task digitalWriteTask = new NationalInstruments.DAQmx.Task())
                {
                    //  Create an Digital Output channel and name it.
                    digitalWriteTask.DOChannels.CreateChannel(test, "port0",
                        ChannelLineGrouping.OneChannelForAllLines);

                    //  Write digital port data. WriteDigitalSingChanSingSampPort writes a single sample
                    //  of digital data on demand, so no timeout is necessary.
                    DigitalSingleChannelWriter writer = new DigitalSingleChannelWriter(digitalWriteTask.Stream);
                    writer.WriteSingleSamplePort(true, 32);
                    //Thread.Sleep(wait);
                    writer.WriteSingleSamplePort(true, 48);
                }
            }

            StringBuilder sbC2 = new StringBuilder(100000);

            IntPtr testBoardValuePtrC2 = testGetBoardValue(sbC2, sbC2.Capacity, excitationLedVoltage);
            double[] testArrayC2 = new double[5];
            Marshal.Copy(testBoardValuePtrC2, testArrayC2, 0, 5);
            string inputStringC2 = "";
            inputStringC2 += "Return Value: m_dAvgValue = " + testArrayC2[0] + "\n";
            inputStringC2 += "Return Value: m_dCumSum = " + testArrayC2[1] + "\n";
            inputStringC2 += "Return Value: m_dLEDtmp = " + testArrayC2[2] + "\n";
            inputStringC2 += "Return Value: m_dPDtmp = " + testArrayC2[3] + "\n";
            inputStringC2 += "Return Value: testGetBoardValue = " + testArrayC2[4] + "\n";

            raw_avg = (testArrayC2[0] - shiftFactors[11]) * scaleFactors[11];
            raw_avg = Math.Round(raw_avg, 3);

            MessageBox.Show("Second reading value: " + raw_avg.ToString());

            for (int i = 0; i < 40; i++)
            {
                using (NationalInstruments.DAQmx.Task digitalWriteTask = new NationalInstruments.DAQmx.Task())
                {
                    //  Create an Digital Output channel and name it.
                    digitalWriteTask.DOChannels.CreateChannel(test, "port0",
                        ChannelLineGrouping.OneChannelForAllLines);

                    //  Write digital port data. WriteDigitalSingChanSingSampPort writes a single sample
                    //  of digital data on demand, so no timeout is necessary.
                    DigitalSingleChannelWriter writer = new DigitalSingleChannelWriter(digitalWriteTask.Stream);
                    writer.WriteSingleSamplePort(true, 16);
                    //Thread.Sleep(wait);
                    writer.WriteSingleSamplePort(true, 0);
                }
            }

            StringBuilder sbC3 = new StringBuilder(100000);

            IntPtr testBoardValuePtrC3 = testGetBoardValue(sbC3, sbC3.Capacity, excitationLedVoltage);
            double[] testArrayC3 = new double[5];
            Marshal.Copy(testBoardValuePtrC3, testArrayC3, 0, 5);
            string inputStringC3 = "";
            inputStringC3 += "Return Value: m_dAvgValue = " + testArrayC3[0] + "\n";
            inputStringC3 += "Return Value: m_dCumSum = " + testArrayC3[1] + "\n";
            inputStringC3 += "Return Value: m_dLEDtmp = " + testArrayC3[2] + "\n";
            inputStringC3 += "Return Value: m_dPDtmp = " + testArrayC3[3] + "\n";
            inputStringC3 += "Return Value: testGetBoardValue = " + testArrayC3[4] + "\n";

            raw_avg = (testArrayC3[0] - shiftFactors[11]) * scaleFactors[11];
            raw_avg = Math.Round(raw_avg, 3);

            MessageBox.Show("Third reading value: " + raw_avg.ToString());

            for (int i = 0; i < 20; i++)
            {
                using (NationalInstruments.DAQmx.Task digitalWriteTask = new NationalInstruments.DAQmx.Task())
                {
                    //  Create an Digital Output channel and name it.
                    digitalWriteTask.DOChannels.CreateChannel(test, "port0",
                        ChannelLineGrouping.OneChannelForAllLines);

                    //  Write digital port data. WriteDigitalSingChanSingSampPort writes a single sample
                    //  of digital data on demand, so no timeout is necessary.
                    DigitalSingleChannelWriter writer = new DigitalSingleChannelWriter(digitalWriteTask.Stream);
                    writer.WriteSingleSamplePort(true, 32);
                    //Thread.Sleep(wait);
                    writer.WriteSingleSamplePort(true, 48);
                }
            }

            for (int i = 0; i < 20; i++)
            {
                using (NationalInstruments.DAQmx.Task digitalWriteTask = new NationalInstruments.DAQmx.Task())
                {
                    //  Create an Digital Output channel and name it.
                    digitalWriteTask.DOChannels.CreateChannel(test, "port0",
                        ChannelLineGrouping.OneChannelForAllLines);

                    //  Write digital port data. WriteDigitalSingChanSingSampPort writes a single sample
                    //  of digital data on demand, so no timeout is necessary.
                    DigitalSingleChannelWriter writer = new DigitalSingleChannelWriter(digitalWriteTask.Stream);
                    writer.WriteSingleSamplePort(true, 12);
                    //Thread.Sleep(wait);
                    writer.WriteSingleSamplePort(true, 8);
                }
            }

            StringBuilder sbC4 = new StringBuilder(100000);

            IntPtr testBoardValuePtrC4 = testGetBoardValue(sbC4, sbC4.Capacity, excitationLedVoltage);
            double[] testArrayC4 = new double[5];
            Marshal.Copy(testBoardValuePtrC4, testArrayC4, 0, 5);
            string inputStringC4 = "";
            inputStringC4 += "Return Value: m_dAvgValue = " + testArrayC4[0] + "\n";
            inputStringC4 += "Return Value: m_dCumSum = " + testArrayC4[1] + "\n";
            inputStringC4 += "Return Value: m_dLEDtmp = " + testArrayC4[2] + "\n";
            inputStringC4 += "Return Value: m_dPDtmp = " + testArrayC4[3] + "\n";
            inputStringC4 += "Return Value: testGetBoardValue = " + testArrayC4[4] + "\n";

            raw_avg = (testArrayC4[0] - shiftFactors[4]) * scaleFactors[4];
            raw_avg = Math.Round(raw_avg, 3);

            MessageBox.Show("Fourth reading value: " + raw_avg.ToString());

            for (int i = 0; i < 20; i++)
            {
                using (NationalInstruments.DAQmx.Task digitalWriteTask = new NationalInstruments.DAQmx.Task())
                {
                    //  Create an Digital Output channel and name it.
                    digitalWriteTask.DOChannels.CreateChannel(test, "port0",
                        ChannelLineGrouping.OneChannelForAllLines);

                    //  Write digital port data. WriteDigitalSingChanSingSampPort writes a single sample
                    //  of digital data on demand, so no timeout is necessary.
                    DigitalSingleChannelWriter writer = new DigitalSingleChannelWriter(digitalWriteTask.Stream);
                    writer.WriteSingleSamplePort(true, 4);
                    //Thread.Sleep(wait);
                    writer.WriteSingleSamplePort(true, 0);
                }
            }

            StringBuilder sbC5 = new StringBuilder(100000);

            IntPtr testBoardValuePtrC5 = testGetBoardValue(sbC5, sbC5.Capacity, excitationLedVoltage);
            double[] testArrayC5 = new double[5];
            Marshal.Copy(testBoardValuePtrC5, testArrayC5, 0, 5);
            string inputStringC5 = "";
            inputStringC5 += "Return Value: m_dAvgValue = " + testArrayC5[0] + "\n";
            inputStringC5 += "Return Value: m_dCumSum = " + testArrayC5[1] + "\n";
            inputStringC5 += "Return Value: m_dLEDtmp = " + testArrayC5[2] + "\n";
            inputStringC5 += "Return Value: m_dPDtmp = " + testArrayC5[3] + "\n";
            inputStringC5 += "Return Value: testGetBoardValue = " + testArrayC5[4] + "\n";

            raw_avg = (testArrayC5[0] - shiftFactors[4]) * scaleFactors[4];
            raw_avg = Math.Round(raw_avg, 3);

            MessageBox.Show("Fifth reading value: " + raw_avg.ToString());
        }
    }
}
