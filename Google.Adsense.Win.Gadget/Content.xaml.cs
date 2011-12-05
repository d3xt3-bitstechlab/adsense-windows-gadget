using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Google.Adsense.Win.Gadget.ViewModel;

namespace Google.Adsense.Win.Gadget
{
    /// <summary>
    /// Interaction logic for Content.xaml
    /// </summary>
    public partial class Content : UserControl
    {
        public Content()
        {
            InitializeComponent();
        }

        private void RefreshExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            OverviewSummaryViewModel.GetInstance().RefreshReport();
        }

        private void CanRefresh(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = OverviewSummaryViewModel.GetInstance().CanRefreshReport;
        }
    }
}
