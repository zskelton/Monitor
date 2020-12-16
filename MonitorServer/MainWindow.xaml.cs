using System;
using System.Windows;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Windows.Media;
using OpenHardwareMonitor.Hardware;

namespace MonitorServer
{
    /// <summary>
    /// Main Window for Performance Counter.
    /// </summary>
    public partial class MainWindow : Window
    {
        // Class Variables
        private bool _started = false;
        private bool _connected = false;
        PerformanceCounter cpuCounter;
        PerformanceCounter memCounter;
        private static System.Windows.Threading.DispatcherTimer clock;
        Computer cp;

        // Init
        public MainWindow()
        {
            InitializeComponent();
            // Start Performance Counters
            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            memCounter = new PerformanceCounter("Memory", "% Committed Bytes In Use", null);

            cp = new Computer() { CPUEnabled = true };
            cp.Open();

            // Start Timer Thread
            clock = new System.Windows.Threading.DispatcherTimer();
            clock.Tick += new EventHandler(OnTickEvent);
            clock.Interval = new TimeSpan(0, 0, 1);
            clock.Start();
        }

        // Run on Tick
        private void OnTickEvent(object sender, EventArgs e)
        {
            // Get Value of CPU
            float cpuVal = cpuCounter.NextValue();
            lbl_cpuStatus.Content = String.Format("{0:0.00}%", cpuVal);
            bar_cpu.Value = cpuVal;

            // Get Value of Ram
            float memVal = memCounter.NextValue();
            lbl_memStatus.Content = String.Format("{0:0.00}%", memVal);
            bar_mem.Value = memVal;

            // Get CPU Temperature - Maybe Not
            foreach (IHardware hardware in cp.Hardware)
            {
                hardware.Update();
                foreach (ISensor sensor in hardware.Sensors)
                {
                    if (sensor.Name == "CPU Total")
                    {
                        lbl_loadStatus.Content = String.Format("Current: {0:0.0}", sensor.Value);
                        bar_load.Value = (double)sensor.Value;
                    }

                }
                Console.WriteLine();
            }

            // Get Network at Hand - Update only on Changes
            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                if (!_connected)
                {
                    lbl_netStatus.Content = "IP: " + localIPAddress() + ".";
                    bar_net.Foreground = new SolidColorBrush(Colors.Green);
                    _connected = true;
                }
            }
            else
            {
                if (_connected)
                {
                    lbl_netStatus.Content = "Not Connected.";
                    bar_net.Foreground = new SolidColorBrush(Colors.Red);
                    _connected = false;
                }
            }
            // Console.WriteLine(memVal.ToString());            
        }

        // Button Click
        private void btn_monitor_click(object sender, RoutedEventArgs e)
        {
            _started = !_started;
            if (!_started)
            {
                btn_monitor.Content = "Start";
            }
            else
            {
                btn_monitor.Content = "End";
            }
        }

        // Get IP Address
        private static string localIPAddress()
        {
            IPHostEntry host;
            string localIP = "";
            host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (IPAddress ip in host.AddressList)
            {
                localIP = ip.ToString();

                string[] temp = localIP.Split('.');

                if (ip.AddressFamily == AddressFamily.InterNetwork && temp[0] == "192")
                {
                    break;
                }
                else
                {
                    localIP = null;
                }
            }

            return localIP;
        }
    }
}
