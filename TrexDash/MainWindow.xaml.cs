using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TrexDash
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainGrid.Children.Clear();
            MainGrid.Children.Add(StartThubmnail);
            MainGrid.Children.Add(StartButton);
        }

        private void StartButtonClick(object sender, RoutedEventArgs e)
        {
            MainGrid.Children.Remove(StartThubmnail);
            MainGrid.Children.Remove(StartButton);
            MainGrid.Children.Add(ChoosingCharBackground);
            MainGrid.Children.Add(RightButton);
            MainGrid.Children.Add(LeftButton);
        }

        private void RightButtonClick(object sender, RoutedEventArgs e)
        {
            MainGrid.Children.Remove(ChoosingCharBackground);
        }

        private void LeftButtonClick(object sender, RoutedEventArgs e)
        {
            MainGrid.Children.Remove(ChoosingCharBackground);
        }
    }
}
