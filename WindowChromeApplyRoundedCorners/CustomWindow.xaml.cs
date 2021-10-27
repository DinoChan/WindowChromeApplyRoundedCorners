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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Shell;

namespace WindowChromeApplyRoundedCorners
{
    /// <summary>
    /// CustomWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CustomWindow : Window
    {
        public CustomWindow(DWM_WINDOW_CORNER_PREFERENCE preference, WindowStyle windowStyle, double borderThickness = 1)
        {
            InitializeComponent();
            WindowHelper.SetCorners(this, preference);
            WindowChrome chrome = new WindowChrome
            {
                GlassFrameThickness = new Thickness(borderThickness, SystemParameters.WindowNonClientFrameThickness.Top + borderThickness, borderThickness, borderThickness),
            };

            switch (windowStyle)
            {
                case WindowChromeApplyRoundedCorners.WindowStyle.Ststem:
                    break;
                case WindowChromeApplyRoundedCorners.WindowStyle.WindowChrome:
                    WindowChrome.SetWindowChrome(this, chrome);
                    break;
                case WindowChromeApplyRoundedCorners.WindowStyle.StyledWindowChrom:
                    WindowChrome.SetWindowChrome(this, chrome);
                    this.Style = Resources["WindowStyle"] as Style;
                    break;
                default:
                    break;
            }
            this.BorderThickness = new Thickness(borderThickness);
           

        }

        private readonly IntPtr _trueValue = new IntPtr(1);

        private static readonly DependencyPropertyKey IsNonClientActivePropertyKey =
       DependencyProperty.RegisterReadOnly("IsNonClientActive", typeof(bool), typeof(CustomWindow), new FrameworkPropertyMetadata(false));

        public static readonly DependencyProperty IsNonClientActiveProperty = IsNonClientActivePropertyKey.DependencyProperty;

        public bool IsNonClientActive
        {
            get
            {
                return (bool)GetValue(IsNonClientActiveProperty);
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            if (SizeToContent == SizeToContent.WidthAndHeight && WindowChrome.GetWindowChrome(this) != null)
            {
                InvalidateMeasure();
            }

            IntPtr handle = new WindowInteropHelper(this).Handle;
            HwndSource.FromHwnd(handle).AddHook(new HwndSourceHook(WndProc));
        }


        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            SetValue(IsNonClientActivePropertyKey, true);
        }

        protected override void OnDeactivated(EventArgs e)
        {
            base.OnDeactivated(e);
            SetValue(IsNonClientActivePropertyKey, false);
        }


        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WindowNotifications.WM_NCACTIVATE)
                SetValue(IsNonClientActivePropertyKey, wParam == _trueValue);

            return IntPtr.Zero;
        }
    }
}
