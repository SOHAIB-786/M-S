using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace QueueSimulatorApp.Services
{
    /// <summary>Small helpers shared by every input box.</summary>
    public static class InputHelper
    {
        private static readonly Regex NumericRegex = new Regex(@"^[0-9]*\.?[0-9]*$");

        /// <summary>
        /// Hook this to PreviewTextInput of a TextBox: only digits and one decimal point are allowed.
        /// </summary>
        public static void NumericOnly(object sender, TextCompositionEventArgs e)
        {
            var tb = (TextBox)sender;
            string current = tb.Text ?? string.Empty;
            string proposed = current.Remove(tb.SelectionStart, tb.SelectionLength)
                                     .Insert(tb.SelectionStart, e.Text);
            e.Handled = !NumericRegex.IsMatch(proposed);
        }

        public static bool TryParse(string text, out double value)
        {
            return double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
        }
    }
}
