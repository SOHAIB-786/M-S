using System.Windows;
using System.Windows.Controls;
using QueueSimulatorApp.Models;
using QueueSimulatorApp.Services;

namespace QueueSimulatorApp.Views
{
    /// <summary>
    /// M/M/1 page.
    /// Flow:  Calculate_Click -> read arrival + service (Mean or Rate, no unit) as mean times
    ///        -> QueueCalculator.CalculateMM1 -> results.Show (ResultsPanel converts to the chosen unit).
    /// </summary>
    public partial class MM1View : UserControl
    {
        public MM1View()
        {
            InitializeComponent();
        }

        private void Calculate_Click(object sender, RoutedEventArgs e)
        {
            txtError.Visibility = Visibility.Collapsed;
            string error;

            double arrivalMean;
            if (!arrivalInput.TryGetMean(out arrivalMean, out error)) { ShowError(error); return; }

            double serviceMean;
            if (!serviceInput.TryGetMean(out serviceMean, out error)) { ShowError(error); return; }

            QueueMetrics m = QueueCalculator.CalculateMM1(arrivalMean, serviceMean);
            results.Show(m);
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            arrivalInput.Reset();
            serviceInput.Reset();
            txtError.Visibility = Visibility.Collapsed;
            results.Hide();
        }

        private void ShowError(string message)
        {
            txtError.Text = message;
            txtError.Visibility = Visibility.Visible;
        }
    }
}
