using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace WindowChromeApplyRoundedCorners
{
    public class WindowService
    {
        public static readonly DependencyProperty IsBindingToSystemCommandsProperty =
            DependencyProperty.RegisterAttached("IsBindingToSystemCommands", typeof(bool), typeof(WindowService), new PropertyMetadata(default(bool), OnIsBindingToSystemCommandsChanged));

        public static bool GetIsBindingToSystemCommands(Window obj) => (bool)obj.GetValue(IsBindingToSystemCommandsProperty);

        public static void SetIsBindingToSystemCommands(Window obj, bool value) => obj.SetValue(IsBindingToSystemCommandsProperty, value);

        private static void OnIsBindingToSystemCommandsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var newValue = (bool)args.NewValue;
            if (obj is Window window && newValue)
            {
                var service = new WindowCommandHelper(window);
                service.ActiveCommands();
            }
        }

        private class WindowCommandHelper
        {
            private readonly Window _window;

            public WindowCommandHelper(Window window)
            {
                _window = window;
            }

            public void ActiveCommands()
            {
                _window.CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, CloseWindow));
                _window.CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand, MaximizeWindow, CanResizeWindow));
                _window.CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, MinimizeWindow, CanMinimizeWindow));
                _window.CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand, RestoreWindow, CanResizeWindow));
                _window.CommandBindings.Add(new CommandBinding(SystemCommands.ShowSystemMenuCommand, ShowSystemMenu));
            }

            private void CanMinimizeWindow(object sender, CanExecuteRoutedEventArgs e)
            {
                e.CanExecute = _window.ResizeMode != ResizeMode.NoResize;
            }

            private void CanResizeWindow(object sender, CanExecuteRoutedEventArgs e)
            {
                e.CanExecute = _window.ResizeMode == ResizeMode.CanResize || _window.ResizeMode == ResizeMode.CanResizeWithGrip;
            }
            private void CloseWindow(object sender, ExecutedRoutedEventArgs e)
            {
                _window.Close();
            }

            private void MaximizeWindow(object sender, ExecutedRoutedEventArgs e)
            {
                SystemCommands.MaximizeWindow(_window);
                e.Handled = true;
            }

            private void MinimizeWindow(object sender, ExecutedRoutedEventArgs e)
            {
                SystemCommands.MinimizeWindow(_window);
                e.Handled = true;
            }

            private void RestoreWindow(object sender, ExecutedRoutedEventArgs e)
            {
                SystemCommands.RestoreWindow(_window);
                e.Handled = true;
            }

            private void ShowSystemMenu(object sender, ExecutedRoutedEventArgs e)
            {
                Point point = _window.PointToScreen(new Point(0, 0));
                var dipScale = WindowParameters.GetDpi() / 96d;
                if (_window.WindowState == WindowState.Maximized)
                {
                    // 因为不想在最大化时改变标题高度，所以这里的计算方式和标准计算方式不一样
                    point.X += (SystemParameters.WindowNonClientFrameThickness.Left + WindowParameters.PaddedBorderThickness.Left) * dipScale;
                    point.Y += (SystemParameters.WindowNonClientFrameThickness.Top +
                                WindowParameters.PaddedBorderThickness.Top +
                                SystemParameters.WindowResizeBorderThickness.Top -
                                _window.BorderThickness.Top)
                                * dipScale;
                }
                else
                {
                    point.X += _window.BorderThickness.Left * dipScale;
                    point.Y += SystemParameters.WindowNonClientFrameThickness.Top * dipScale;
                }

                CompositionTarget compositionTarget = PresentationSource.FromVisual(_window).CompositionTarget;
                SystemCommands.ShowSystemMenu(_window, compositionTarget.TransformFromDevice.Transform(point));
                e.Handled = true;
            }
        }
    }
}