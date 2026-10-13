using System.Windows;
using System.Windows.Controls;
using QueueSimulatorApp.Controls;
using QueueSimulatorApp.Models;
using QueueSimulatorApp.Services;

namespace QueueSimulatorApp.Views
{
    /// <summary>
    /// G/G/1 page.
    /// Flow:  Calculate_Click -> arrival (mean + variance box) and service (DistributionInput) each give (mean, variance)
    ///        -> QueueCalculator.CalculateGG1 -> results.Show.
    /// Arrival: Mean or Rate (RateTimeInput) + a separate "Variance of arrival time" box.
    /// Service: Uniform(a, b) / Mean / Rate (DistributionInput; Uniform gives the variance automatically).
    /// </summary>
    public partial class GG1View : UserControl
    {

        public GG1View()
        {
            InitializeComponent();
            ApplyDefaults();
        }

        private void ApplyDefaults()
        {
            serviceInput.SelectedMethod = DistributionInput.MethodUniform;
        }

        private void Calculate_Click(object sender, RoutedEventArgs e)
        {
            txtError.Visibility = Visibility.Collapsed;
            string error;

            double arrivalMean;
            if (!arrivalInput.TryGetMean(out arrivalMean, out error)) { ShowError(error); return; }

            double arrivalVar;
            if (!InputHelper.TryParse(txtArrivalVar.Text, out arrivalVar) || arrivalVar < 0)
            {
                ShowError("Arrival (inter-arrival time) - please enter a valid variance of arrival time (0 or more).");
                return;
            }

            double serviceMean, serviceVar;
            if (!serviceInput.TryGet(out serviceMean, out serviceVar, out error)) { ShowError(error); return; }

            QueueMetrics m = QueueCalculator.CalculateGG1(arrivalMean, arrivalVar, serviceMean, serviceVar);
            results.Show(m);
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            arrivalInput.Reset();
            txtArrivalVar.Text = string.Empty;
            serviceInput.Reset();
            ApplyDefaults();
            txtError.Visibility = Visibility.Collapsed;
            results.Hide();
        }

        private void Numeric_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            InputHelper.NumericOnly(sender, e);
        }

        private void ShowError(string message)
        {
            txtError.Text = message;
            txtError.Visibility = Visibility.Visible;
        }
    }
}
