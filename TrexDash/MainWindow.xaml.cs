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
            MainCanvas.Children.Clear();
            MainCanvas.Children.Add(StartThubmnail);
            MainCanvas.Children.Add(StartButton);
        }

        private void StartButtonClick(object sender, RoutedEventArgs e)
        {
            MainCanvas.Children.Remove(StartThubmnail);
            MainCanvas.Children.Remove(StartButton);
            var elementsToAdd = new UIElement[] { ChoosingCharBackground, RightButton, LeftButton, Cow, UFO, Dino, PlayButton };
            foreach (var el in elementsToAdd)
                MainCanvas.Children.Add(el);
            SetCharacterVisibility(2);
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
        private void PlayButtonClick(object sender, RoutedEventArgs e)
        {
            MainCanvas.Children.Clear();
            Dino dino = new Dino(MainCanvas);
        }
    }
}
