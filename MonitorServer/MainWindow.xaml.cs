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

namespace MonitorServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _started = false;

        public MainWindow()
        { 
            InitializeComponent();
        }

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
