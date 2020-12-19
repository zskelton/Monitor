using System;
using System.Windows;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Windows.Media;
using System.Net.NetworkInformation;
using System.IO.Ports;

// TODO: Setup Serial Connection
// TODO: Send Data to Serial Connection
// TODO: Gracefully Kill Connection
// TODO: Gracefully Exit the Program

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
        NetworkInterface netCounter;
        private static System.Windows.Threading.DispatcherTimer clock;
        private string portName;

        // Init
        public MainWindow()
        {
            InitializeComponent();

            // Start  Other Performance Counters
            netCounter = getNetInterface();
            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            memCounter = new PerformanceCounter("Memory", "% Committed Bytes In Use", null);
            
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

            // Get Network Speed
            if(netCounter != null)
            {
                lbl_netReceived.Content = String.Format("Received: {0:0.0} MBs", netCounter.GetIPv4Statistics().BytesReceived/1024/1024);
                lbl_netSent.Content = String.Format("Sent: {0:0.0} MBs", netCounter.GetIPv4Statistics().BytesSent/1024/1024);
                lbl_netType.Content = netCounter.NetworkInterfaceType.ToString();
            } else
            {
                lbl_netReceived.Content = "0 Bytes";
                lbl_netSent.Content = "0 Bytes";
                lbl_netType.Content = "No Interface.";
            }

            // Get Network at Hand - Update only on Changes
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                    if (!_connected)
                {
                    lbl_netIP.Content = String.Format("IP: {0}", localIPAddress());
                    bar_net.Foreground = new SolidColorBrush(Colors.Green);
                    _connected = true;
                }
            }
            else
            {
                if (_connected)
                {
                    lbl_netIP.Content = "Not Connected.";
                    bar_net.Foreground = new SolidColorBrush(Colors.Red);
                    _connected = false;
                }
            }

            // Update Port Name
            if (!setUSB())
            {
                portName = null;
                lbl_usbStatus.Content = "No USB Connection.";
                bar_usb.Value = 0;
                btn_monitor.IsEnabled = false;
            } else
            {
                lbl_usbStatus.Content = String.Format("Connected on {0}", portName);
                bar_usb.Value = 100;
                btn_monitor.IsEnabled = true;
            }
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

        // Get Network Interface
        private NetworkInterface getNetInterface()
        {
            NetworkInterface _netCounter = null;
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    _netCounter = ni;
                }
            }
            return _netCounter;
        }

        // List and Assign Serial Port
        private bool setUSB()
        {
            string[] ports = SerialPort.GetPortNames();

            if(ports.Length == 0)
            {
                return false;
            }

            if(portName != null)
            {
                return true;
            }

            foreach (string port in ports)
            {
                portName = port;
            }
            return true;
        }
    }
}
