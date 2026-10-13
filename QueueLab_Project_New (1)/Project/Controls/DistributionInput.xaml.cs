using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using QueueSimulatorApp.Services;

namespace QueueSimulatorApp.Controls
{
    /// <summary>
    /// SERVICE TIME input block (M/G/1 and G/G/1).
    /// Drop-down has only 3 choices: Uniform distribution / Mean / Rate.
    /// TryGet() always returns  mean service time  and  variance of service time (no unit).
    /// </summary>
    public partial class DistributionInput : UserControl
    {
        // Must match the item ORDER of cmbMethod in DistributionInput.xaml
        public const int MethodUniform = 0;
        public const int MethodMean = 1;
        public const int MethodRate = 2;

        private bool _ready;
        private int _prevMethod = MethodUniform;
        private string _title = string.Empty;
        private string _meanLabel = "Mean service time";
        private string _rateLabel = "Service rate μ";
        private string _variabilityName = "service time";

        public DistributionInput()
        {
            InitializeComponent();
            _ready = true;
            ApplyMethod();
        }

        /// <summary>Heading, e.g. "Service time". Also used in error messages.</summary>
        public string Title
        {
            get { return _title; }
            set { _title = value ?? string.Empty; lblTitle.Text = _title; }
        }

        public string MeanLabel
        {
            get { return _meanLabel; }
            set { _meanLabel = value; ApplyMethod(); }
        }

        public string RateLabel
        {
            get { return _rateLabel; }
            set { _rateLabel = value; ApplyMethod(); }
        }

        /// <summary>Text used in the "Variance of ..." label, e.g. "service time".</summary>
        public string VariabilityName
        {
            get { return _variabilityName; }
            set { _variabilityName = value; ApplyMethod(); }
        }

        /// <summary>Selected input method (use the Method* constants).</summary>
        public int SelectedMethod
        {
            get { return cmbMethod.SelectedIndex; }
            set { cmbMethod.SelectedIndex = value; }
        }

        public void Reset()
        {
            txtMin.Text = string.Empty;
            txtMax.Text = string.Empty;
            txtValue.Text = string.Empty;
            txtVar.Text = string.Empty;
        }

        // ------------------------------------------------------------------
        //  Read the input as (mean, variance)
        // ------------------------------------------------------------------
        public bool TryGet(out double mean, out double variance, out string error)
        {
            mean = 0;
            variance = 0;
            error = null;
            int method = cmbMethod.SelectedIndex;

            // ---- Condition 1: Uniform(a, b) -> mean AND variance both come from a & b ----
            if (method == MethodUniform)
            {
                double a, b;
                if (!TryParseUniform(out a, out b, out error)) return false;

                mean = (a + b) / 2.0;                          // E = (a + b) / 2
                variance = (b - a) * (b - a) / 12.0;           // Var = (b - a)^2 / 12

                if (mean <= 0)
                {
                    error = Prefix() + "the mean of the uniform distribution must be greater than 0.";
                    return false;
                }
                return true;
            }

            // ---- Condition 2 / 3: Mean or Rate (+ variance box) ----
            double v;
            bool isRate = method == MethodRate;
            string label = isRate ? RateLabel : MeanLabel;
            if (!InputHelper.TryParse(txtValue.Text, out v) || v <= 0)
            {
                error = Prefix() + "please enter a valid " + label + " (greater than 0).";
                return false;
            }
            mean = isRate ? 1.0 / v : v;                       // rate -> mean = 1 / rate

            double varValue;
            if (!InputHelper.TryParse(txtVar.Text, out varValue) || varValue < 0)
            {
                error = Prefix() + "please enter a valid variance of " + VariabilityName + " (0 or more).";
                return false;
            }
            variance = varValue;
            return true;
        }

        // ------------------------------------------------------------------
        //  UI behaviour
        // ------------------------------------------------------------------
        private void Method_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (!_ready) return;

            // Mean <-> Rate: the old number meant something else, so clear it instead of reusing it by mistake
            int now = cmbMethod.SelectedIndex;
            if ((_prevMethod == MethodMean && now == MethodRate) || (_prevMethod == MethodRate && now == MethodMean))
                txtValue.Text = string.Empty;
            _prevMethod = now;

            ApplyMethod();
        }

        private void Input_Changed(object sender, TextChangedEventArgs e)
        {
            if (!_ready) return;
            UpdatePreview();
        }

        private void ApplyMethod()
        {
            if (!_ready) return;
            int method = cmbMethod.SelectedIndex;

            pnlUniform.Visibility = method == MethodUniform ? Visibility.Visible : Visibility.Collapsed;
            pnlValue.Visibility = method == MethodUniform ? Visibility.Collapsed : Visibility.Visible;

            lblValue.Text = method == MethodRate ? RateLabel : MeanLabel;
            lblVar.Text = "Variance of " + VariabilityName + "  (time², same unit as the inputs)";

            UpdatePreview();
        }

        /// <summary>Live preview: mean and variance calculated from Min a and Max b.</summary>
        private void UpdatePreview()
        {
            if (!_ready) return;
            txtPreview.Text = string.Empty;
            if (cmbMethod.SelectedIndex != MethodUniform) return;

            double a, b;
            string err;
            if (!TryParseUniform(out a, out b, out err)) return;

            txtPreview.Text = "Mean = (a+b)/2 = " + F((a + b) / 2.0)
                            + "     Variance = (b−a)²/12 = " + F((b - a) * (b - a) / 12.0);
        }

        private static string F(double v)
        {
            return v.ToString("0.0000", CultureInfo.InvariantCulture);
        }

        private bool TryParseUniform(out double a, out double b, out string error)
        {
            error = null;
            b = 0;
            if (!InputHelper.TryParse(txtMin.Text, out a) || a < 0)
            {
                error = Prefix() + "please enter a valid Min a (0 or more).";
                return false;
            }
            if (!InputHelper.TryParse(txtMax.Text, out b) || b < a)
            {
                error = Prefix() + "please enter a valid Max b (must be at least Min).";
                return false;
            }
            return true;
        }

        private string Prefix()
        {
            return string.IsNullOrEmpty(_title) ? string.Empty : _title + " - ";
        }

        private void Numeric_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            InputHelper.NumericOnly(sender, e);
        }
    }
}
