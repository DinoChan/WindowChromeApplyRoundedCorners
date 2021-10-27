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

namespace WindowChromeApplyRoundedCorners
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnShowWindow(object sender, RoutedEventArgs e)
        {
            new CustomWindow((DWM_WINDOW_CORNER_PREFERENCE)CornerSelector.SelectedIndex, (WindowStyle)StyleSelector.SelectedIndex,
                BorderSelector.SelectedIndex switch
                {
                    0 => 1d,
                    1 => 3d,
                    2 => 20d,
                    _ => 0d
                }).Show();
        }
    }
}
