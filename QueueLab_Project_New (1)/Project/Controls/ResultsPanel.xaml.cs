using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using QueueSimulatorApp.Models;

namespace QueueSimulatorApp.Controls
{
    /// <summary>One tile in the results grid.</summary>
    public class ResultItem
    {
        public string Symbol { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Unit { get; set; }
    }

    /// <summary>
    /// Shows a QueueMetrics object. The metrics arrive in the unit the user typed the inputs in
    /// ("Input values are in" drop-down); this panel converts them to the unit chosen in
    /// "Show results in" (Seconds / Minutes / Hours) and re-draws instantly when either changes.
    /// </summary>
    public partial class ResultsPanel : UserControl
    {
        private bool _ready;
        private QueueMetrics _last;

        public ResultsPanel()
        {
            InitializeComponent();
            _ready = true;
        }

        public void Show(QueueMetrics metrics)
        {
            _last = metrics;
            Render();
            pnlResults.Visibility = Visibility.Visible;
        }

        public void Hide()
        {
            _last = null;
            itemsResults.ItemsSource = null;
            pnlResults.Visibility = Visibility.Collapsed;
        }

        private void Unit_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (!_ready || _last == null) return;
            Render();
        }

        private void Render()
        {
            QueueMetrics m = _last;
            if (m == null) return;

            TimeUnit inUnit = TimeUnitInfo.FromIndex(cmbInUnit.SelectedIndex);
            TimeUnit outUnit = TimeUnitInfo.FromIndex(cmbOutUnit.SelectedIndex);
            string us = TimeUnitInfo.Short(outUnit);

            // r = how many OUTPUT units are in one INPUT unit (e.g. input hours -> output minutes: r = 60)
            //   time      : multiply by r        (0.5 hr -> 30 min)
            //   variance  : multiply by r^2
            //   rate      : divide by r          (6 per hr -> 0.1 per min)
            double r = TimeUnitInfo.MinutesPerUnit(inUnit) / TimeUnitInfo.MinutesPerUnit(outUnit);
            string T(double t) { return Fmt(t * r); }
            string V(double v) { return Fmt(v * r * r); }
            string R(double rate) { return Fmt(rate / r); }

            var items = new List<ResultItem>();

            items.Add(new ResultItem { Symbol = "L", Name = "Avg customers in system", Value = Fmt(m.L), Unit = "customers" });
            items.Add(new ResultItem { Symbol = "Lq", Name = "Avg customers in queue", Value = Fmt(m.Lq), Unit = "customers" });
            items.Add(new ResultItem { Symbol = "W", Name = "Avg time in system", Value = T(m.W), Unit = us });
            items.Add(new ResultItem { Symbol = "Wq", Name = "Avg time in queue", Value = T(m.Wq), Unit = us });
            items.Add(new ResultItem
            {
                Symbol = "ρ",
                Name = "Utilization (rho)",
                Value = Fmt(m.Rho),
                Unit = "(" + (m.Rho * 100).ToString("0.00", CultureInfo.InvariantCulture) + " %)"
            });
            items.Add(new ResultItem { Symbol = "μ", Name = "Service rate (mu)", Value = R(m.Mu), Unit = "per " + us });
            items.Add(new ResultItem
            {
                Symbol = "P0",
                Name = "Probability system is empty",
                Value = Fmt(m.P0),
                Unit = m.IsStable ? "(" + (m.P0 * 100).ToString("0.00", CultureInfo.InvariantCulture) + " %)" : string.Empty
            });
            items.Add(new ResultItem { Symbol = "λ", Name = "Arrival rate (lambda)", Value = R(m.Lambda), Unit = "per " + us });
            items.Add(new ResultItem { Symbol = "1/λ", Name = "Arrival time (mean inter-arrival)", Value = T(m.MeanArrival), Unit = us });
            items.Add(new ResultItem { Symbol = "1/μ", Name = "Service time (mean)", Value = T(m.MeanService), Unit = us });

            if (m.Model == QueueingModelType.MG1)
            {
                items.Add(new ResultItem { Symbol = "Var(S)", Name = "Variance of service time", Value = V(m.VarService), Unit = us + "²" });
            }

            if (m.Model == QueueingModelType.GG1)
            {
                items.Add(new ResultItem { Symbol = "Var(S)", Name = "Variance of service time", Value = V(m.VarService), Unit = us + "²" });
                items.Add(new ResultItem { Symbol = "Var(A)", Name = "Variance of arrival time", Value = V(m.VarArrival), Unit = us + "²" });
                items.Add(new ResultItem { Symbol = "Ca²", Name = "Squared coefficient of arrivals", Value = Fmt(m.Ca2), Unit = string.Empty });
                items.Add(new ResultItem { Symbol = "Cs²", Name = "Squared coefficient of service", Value = Fmt(m.Cs2), Unit = string.Empty });
            }

            itemsResults.ItemsSource = items;

            if (!m.IsStable)
            {
                txtNote.Text = "System is unstable (ρ ≥ 1): arrivals are faster than service, so the queue grows without limit.";
                txtNote.Visibility = Visibility.Visible;
            }
            else if (m.Model == QueueingModelType.GG1)
            {
                txtNote.Text = "Note: G/G/1 values are an approximation.";
                txtNote.Visibility = Visibility.Visible;
            }
            else
            {
                txtNote.Text = string.Empty;
                txtNote.Visibility = Visibility.Collapsed;
            }
        }

        private static string Fmt(double v)
        {
            if (double.IsNaN(v)) return "N/A";
            if (double.IsInfinity(v)) return "∞";
            return v.ToString("0.0000", CultureInfo.InvariantCulture);
        }
    }
}
