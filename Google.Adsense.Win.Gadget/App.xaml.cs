using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace Google.Adsense.Win.Gadget
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void ApplicationStartup(object sender, StartupEventArgs e)
        {
            MainWindow = new Window();
            MainWindow.Background = System.Windows.Media.Brushes.Transparent;
            MainWindow.AllowsTransparency = true;
            MainWindow.WindowStyle = WindowStyle.None;
            MainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            MainWindow.Content = new MainWindow();
            MainWindow.SizeToContent = SizeToContent.WidthAndHeight;
            MainWindow.MouseLeftButtonDown += delegate
             {
                 MainWindow.DragMove();
             };
            MainWindow.ShowDialog();
        }
    }
}
