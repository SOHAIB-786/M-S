using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace QueueSimulatorApp
{
    /// <summary>
    /// The main window is created here (not through StartupUri) so that ANY startup error is shown
    /// in a message box and written to crash.log (next to the .exe) instead of the app just closing.
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            DispatcherUnhandledException += OnDispatcherUnhandledException;

            try
            {
                var window = new MainWindow();
                MainWindow = window;
                window.Show();
            }
            catch (Exception ex)
            {
                ReportError(ex);
                Shutdown(1);
            }
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            ReportError(e.Exception);
            e.Handled = true;
        }

        private static void ReportError(Exception ex)
        {
            string text = ex.ToString();
            try
            {
                File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "crash.log"), text);
            }
            catch { /* ignore logging problems */ }

            MessageBox.Show(text, "QueueLab - error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
