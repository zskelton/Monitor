using System;
using System.Windows;
using System.Diagnostics;
using System.Runtime;
using System.Runtime.InteropServices;

namespace MonitorServer
{
    /// <summary>
    /// Main Window for Performance Counter.
    /// </summary>
    public partial class MainWindow : Window
    {
        // Pull Methods
        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int description, int reservedValue);

        // Class Variables
        private bool _started = false;
        PerformanceCounter cpuCounter;
        PerformanceCounter memCounter;
        private static System.Windows.Threading.DispatcherTimer clock;
        
        // Init
        public MainWindow()
        { 
            InitializeComponent();
            // Start Performance Counters
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

            // Get CPU Temperature

            // Get Network at Hand
            if(IsInternetAvailable())
            {
                lbl_netStatus.Content = "Connected";
                bar_net.Value = 100;
            } else
            {
                lbl_netStatus.Content = "Not Connected";
                bar_net.Value = 0;
            }
            // Console.WriteLine(memVal.ToString());            
        }

        // Button Click
        private void btn_monitor_click(object sender, RoutedEventArgs e)
        {
            _started = !_started;
            if(!_started)
            {
                btn_monitor.Content = "Start";
            } else
            {
                btn_monitor.Content = "End";
            }
        }

        // Check Internet Connection
        private static bool IsInternetAvailable()
        {
            int description;
            return InternetGetConnectedState(out description, 0);
        }
    }
}
