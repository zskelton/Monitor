using System;
using System.Windows;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Windows.Media;
using System.Net.NetworkInformation;
using System.IO.Ports;

// TODO: Better handle USB Disconnect/Connects

namespace MonitorServer
{
    /// <summary>
    /// Main Window for Performance Counter.
    /// </summary>
    public partial class MainWindow : Window, IDisposable
    {
        // Class Variables
        private bool _connected = false;
        private PerformanceCounter cpuCounter;
        private PerformanceCounter memCounter;
        private NetworkInterface netCounter;
        private static System.Windows.Threading.DispatcherTimer clock;
        private string portName;
        private SerialPort serialPort;
        private bool portOpen;

        // Init
        public MainWindow()
        {
            InitializeComponent();

            // Set Class Variables
            portOpen = false;

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
                lbl_netReceived.Content = String.Format("Received - {0:0.0} MBs", netCounter.GetIPv4Statistics().BytesReceived/1024/1024);
                lbl_netSent.Content = String.Format("Sent - {0:0.0} MBs", netCounter.GetIPv4Statistics().BytesSent/1024/1024);
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
                    lbl_netIP.Content = String.Format("IP - {0}", localIPAddress());
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
                if(portOpen)
                {
                    portOpen = false;
                    serialPort.Close();
                    serialPort = null;
                }
                portName = null;
                lbl_usbStatus.Content = "No USB Connection.";
                bar_usb.Value = 0;
            } else
            {
                if(!portOpen && portName != null)
                {
                    serialPort = new SerialPort(portName, 9600, Parity.None, 8, StopBits.One);
                    serialPort.Open();
                    portOpen = true;
                }
                lbl_usbStatus.Content = String.Format("Connected on {0}", portName);
                bar_usb.Value = 100;
            }

            // Send Stats if Port is Open
            if(portOpen)
            {
                // Format:
                // 0 - [barCpu]                  : 0-100
                // 1 - [barMem]                  : 0-100
                // 2 - [barNet]                  : 0|100
                // 3 - [lblNetConnect]           : "" | Type
                // 4 - [lblNetIP]                : "" | IP
                // 5 - [lblNetBytesReceivedData] : Number
                // 6 - [lblNetBytesSentData]       Number
                // End \n
                // Example
                // 50:50:100:Ethernet:192.168.1.10:Received          10 MBs:Sent             100 MBs
                int con = 0;
                if(_connected)
                {
                    con = 100;
                }
                String type = lbl_netType.Content.ToString();
                String ip = lbl_netIP.Content.ToString();
                String rec = lbl_netReceived.Content.ToString();
                String sent = lbl_netSent.Content.ToString();
                String line = String.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}\n", (int)cpuVal, (int)memVal, con, type, ip, rec, sent);
                try
                {
                    serialPort.Write(line);
                }
                catch
                {
                    Console.WriteLine("Disconnected USB.");
                }
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

        private void OnWindowClose(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Dispose();
        }

        public void Dispose()
        {
            // Kill Connections
            if(portOpen)
            {
                serialPort.Close();
            }
            cpuCounter.Close();
            memCounter.Close();
        }
    }
}
