using System.Windows;
using System.Windows.Controls;
using QueueSimulatorApp.Views;

namespace QueueSimulatorApp
{
    /// <summary>
    /// Shell of the app.
    ///   Start screen : project name + one centred drop-down (M/M/1, M/G/1, G/G/1).
    ///   Model screen : the page of the chosen model (MM1View / MG1View / GG1View) + a navbar with
    ///                  a drop-down to switch the model and a Back button.
    /// Each page is created once, so values typed on a page are kept if the user switches
    /// the model and comes back.
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MM1View _mm1 = new MM1View();
        private readonly MG1View _mg1 = new MG1View();
        private readonly GG1View _gg1 = new GG1View();

        private bool _syncing;   // stops the two drop-downs from re-triggering each other

        public MainWindow()
        {
            InitializeComponent();
        }

        // Drop-down index: 0 = M/M/1, 1 = M/G/1, 2 = G/G/1   (same order in both drop-downs)
        private void Model_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (_syncing || pageHost == null || cmbSwitch == null || cmbModel == null) return;

            int index = ((ComboBox)sender).SelectedIndex;
            ShowModel(index);
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            ShowModel(-1);
        }

        private void ShowModel(int index)
        {
            // keep both drop-downs on the same choice
            _syncing = true;
            cmbModel.SelectedIndex = index;
            cmbSwitch.SelectedIndex = index;
            _syncing = false;

            switch (index)
            {
                case 0: pageHost.Content = _mm1; break;
                case 1: pageHost.Content = _mg1; break;
                case 2: pageHost.Content = _gg1; break;
                default: pageHost.Content = null; break;
            }

            bool hasModel = pageHost.Content != null;
            landing.Visibility = hasModel ? Visibility.Collapsed : Visibility.Visible;
            navBar.Visibility = hasModel ? Visibility.Visible : Visibility.Collapsed;
            pageScroll.Visibility = hasModel ? Visibility.Visible : Visibility.Collapsed;
            txtPlaceholder.Visibility = index < 0 ? Visibility.Visible : Visibility.Collapsed;

            if (hasModel) pageScroll.ScrollToTop();
        }
    }
}
