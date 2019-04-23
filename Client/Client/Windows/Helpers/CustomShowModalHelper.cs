using Client.Windows.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;

namespace Client.Windows.Helpers
{
    public class CustomShowModalHelper
    {
        [DllImport("user32.dll")]
        private static extern bool EnableWindow(IntPtr hWnd, bool bEnable);

        bool? result;
        private readonly Window _window;
        private DispatcherFrame _dispatcherFrame;
        public CustomShowModalHelper(Window window)
        {
            if (window == null)
            {
                throw new ArgumentNullException("window");
            }

            if (window.Owner == null)
            {
                throw new ArgumentNullException("window.Owner cannot be null");
            }

            if ((window as ICustomDialog) == null)
            {
                throw new ArgumentException("window should implement ICustomDialog interface");
            }

            _window = window;

            IntPtr parentHandle = (new WindowInteropHelper(_window.Owner)).Handle;
            // disable parent window
            EnableWindow(parentHandle, false);
            // when the dialog is closing we want to re-enable the parent
            _window.Closing += SpecialDialogWindow_Closing;
        }
        internal void ShowAndWait()
        {
            if (_dispatcherFrame != null)
            {
                throw new InvalidOperationException("Cannot call ShowAndWait while waiting for a previous call to ShowAndWait to return.");
            }
            _window.Closed += OnWindowClosed;
            _window.Show();
            _dispatcherFrame = new DispatcherFrame();
            Dispatcher.PushFrame(_dispatcherFrame);

            _window.Owner.Activate();
        }

        private void SpecialDialogWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var win = (Window)sender;
            win.Closing -= SpecialDialogWindow_Closing;
            IntPtr winHandle = (new WindowInteropHelper(win)).Handle;
            EnableWindow(winHandle, false);

            if (win.Owner != null)
            {
                IntPtr parentHandle = (new WindowInteropHelper(win.Owner)).Handle;
                // reenable parent window
                EnableWindow(parentHandle, true);
            }
        }

        private void OnWindowClosed(object source, EventArgs eventArgs)
        {
            if (_dispatcherFrame == null)
            {
                return;
            }
            _window.Closed -= OnWindowClosed;
            _dispatcherFrame.Continue = false;
            _dispatcherFrame = null;
            result = (_window as ICustomDialog)?.CustomDialogResult;
        }

        internal bool GetResult()
        {
            return result ?? false;
        }
    }
}
