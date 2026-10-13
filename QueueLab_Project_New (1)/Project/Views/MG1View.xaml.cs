using System.Windows;
using System.Windows.Controls;
using QueueSimulatorApp.Controls;
using QueueSimulatorApp.Models;
using QueueSimulatorApp.Services;

namespace QueueSimulatorApp.Views
{
    /// <summary>
    /// M/G/1 page.
    /// Service time defaults to Uniform(a, b) (mean = (a+b)/2, variance = (b-a)^2/12).
    /// Flow:  Calculate_Click -> arrival (mean/rate) + service (DistributionInput gives 1/μ and Var(S))
    ///        -> QueueCalculator.CalculateMG1 -> results.Show.
    /// </summary>
    public partial class MG1View : UserControl
    {
        public MG1View()
        {
            InitializeComponent();
        }

        private void Calculate_Click(object sender, RoutedEventArgs e)
        {
            txtError.Visibility = Visibility.Collapsed;
            string error;

            double arrivalMean;
            if (!arrivalInput.TryGetMean(out arrivalMean, out error)) { ShowError(error); return; }

            double serviceMean, serviceVar;
            if (!serviceInput.TryGet(out serviceMean, out serviceVar, out error)) { ShowError(error); return; }

            QueueMetrics m = QueueCalculator.CalculateMG1(arrivalMean, serviceMean, serviceVar);
            results.Show(m);
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            arrivalInput.Reset();
            serviceInput.Reset();
            serviceInput.SelectedMethod = DistributionInput.MethodUniform;
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
