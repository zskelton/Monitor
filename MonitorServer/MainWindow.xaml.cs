using System;
using System.Windows;
using System.Diagnostics;

namespace MonitorServer
{
    /// <summary>
    /// Main Window for Performance Counter.
    /// </summary>
    public partial class MainWindow : Window
    {
        // Class Variables
        private bool _started = false;
        PerformanceCounter cpuCounter;
        PerformanceCounter ramCounter;
        private static System.Windows.Threading.DispatcherTimer counter;
        
        // Init
        public MainWindow()
        { 
            InitializeComponent();
            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            ramCounter = new PerformanceCounter("Memory", "Available MBytes");

            counter = new System.Windows.Threading.DispatcherTimer();
            counter.Tick += new EventHandler(OnTickEvent);
            counter.Interval = new TimeSpan(0, 0, 1);
            counter.Start();
        }

        // Run on Tick
        private void OnTickEvent(object sender, EventArgs e)
        {
            // Get Value of CPU
            float cpuVal = cpuCounter.NextValue();
            lbl_cpu.Content = String.Format("{0:0.00}%", cpuVal);
            bar_cpu.Value = cpuVal;

            float ramVal = ramCounter.NextValue();
            Console.WriteLine(ramVal.ToString());            
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
    }
}
