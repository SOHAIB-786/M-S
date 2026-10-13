using System;
using System.Windows.Controls;
using System.Windows.Input;
using QueueSimulatorApp.Services;

namespace QueueSimulatorApp.Controls
{
    /// <summary>
    /// "Mean time OR rate" input (no unit - the unit is picked later in the Results panel).
    /// TryGetMean() always returns the MEAN TIME:
    ///     mean  v  ->  v
    ///     rate  v  ->  1 / v
    /// </summary>
    public partial class RateTimeInput : UserControl
    {
        private bool _ready;
        private string _title = string.Empty;
        private string _errorName = null;
        private string _meanLabel = "Mean time";
        private string _rateLabel = "Rate";

        /// <summary>Raised when the Mean/Rate drop-down changes.</summary>
        public event EventHandler SettingsChanged;

        public RateTimeInput()
        {
            InitializeComponent();
            _ready = true;
            UpdateHint();
        }

        /// <summary>Optional heading above the row (e.g. "Arrival"). Empty = no heading.</summary>
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value ?? string.Empty;
                lblTitle.Text = _title;
                lblTitle.Visibility = _title.Length == 0 ? System.Windows.Visibility.Collapsed
                                                         : System.Windows.Visibility.Visible;
            }
        }

        /// <summary>Name used at the start of validation messages. Defaults to Title.</summary>
        public string ErrorName
        {
            get { return _errorName ?? _title; }
            set { _errorName = value; }
        }

        public string MeanLabel
        {
            get { return _meanLabel; }
            set { _meanLabel = value; UpdateHint(); }
        }

        public string RateLabel
        {
            get { return _rateLabel; }
            set { _rateLabel = value; UpdateHint(); }
        }

        public bool IsRate { get { return cmbMode.SelectedIndex == 1; } }

        public void Reset()
        {
            txtValue.Text = string.Empty;
            cmbMode.SelectedIndex = 0;
        }

        /// <summary>Reads the box and returns the MEAN time (rate is converted with 1 / rate).</summary>
        public bool TryGetMean(out double mean, out string error)
        {
            mean = 0;
            error = null;

            double v;
            if (!InputHelper.TryParse(txtValue.Text, out v) || v <= 0)
            {
                string label = IsRate ? RateLabel : MeanLabel;
                string prefix = string.IsNullOrEmpty(ErrorName) ? string.Empty : ErrorName + " - ";
                error = prefix + "please enter a valid " + label + " (greater than 0).";
                return false;
            }

            mean = IsRate ? 1.0 / v : v;
            return true;
        }

        private void Option_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (!_ready) return;
            txtValue.Text = string.Empty;   // old number meant a mean (or a rate) - don't reuse it by mistake
            UpdateHint();
            var handler = SettingsChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        private void UpdateHint()
        {
            if (!_ready) return;
            lblHint.Text = IsRate ? RateLabel : MeanLabel;
        }

        private void Numeric_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            InputHelper.NumericOnly(sender, e);
        }
    }
}
