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
        int charCount = 2;
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
            Dino.Opacity = 1;
            Cow.Opacity = 0;
            UFO.Opacity = 0;
            MainGrid.Children.Add(Cow);
            MainGrid.Children.Add(UFO);
            MainGrid.Children.Add(Dino);
            MainGrid.Children.Add(PlayButton);
        }

        private void RightButtonClick(object sender, RoutedEventArgs e)
        {
            charCount++;
            if (charCount > 2) { charCount = 0; }
            SetCharacterVisibility(charCount);
        }

        private void LeftButtonClick(object sender, RoutedEventArgs e)
        {
            charCount--;
            if (charCount < 0) { charCount = 2; }
            SetCharacterVisibility(charCount);
        }
        private void SetCharacterVisibility(int index)
        {
            Cow.Opacity = 0;
            UFO.Opacity = 0;
            Dino.Opacity = 0;

            switch (index)
            {
                case 0:
                    UFO.Opacity = 1;
                    break;
                case 1:
                    Cow.Opacity = 1;
                    break;
                case 2:
                    Dino.Opacity = 1;
                    break;
            }
        }
    }
}
