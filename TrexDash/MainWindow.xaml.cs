using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using System.Windows.Threading;

namespace TrexDash
{
    public partial class MainWindow : Window
    {
        int charCount = 2;
        private Func<Canvas, Character> characterFactory;
        private Character player;
        public MainWindow()
        {
            InitializeComponent();
            MainCanvas.Children.Clear();
            MainCanvas.Children.Add(StartThubmnail);
            MainCanvas.Children.Add(StartButton);
            this.KeyDown += MainWindow_KeyDown;
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
                    characterFactory = canvas => new UFO(canvas);
                    break;
                case 1:
                    Cow.Opacity = 1;
                    characterFactory = canvas => new Cow(canvas);
                    break;
                case 2:
                    Dino.Opacity = 1;
                    characterFactory = canvas => new Dino(canvas);
                    break;
            }
        }
        private void PlayButtonClick(object sender, RoutedEventArgs e)
        {
            MainCanvas.Children.Clear();
            player = characterFactory(MainCanvas);
            _ = SpawnObstacles();
        }
        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                Task.Run(async () => await player.Jump());
            }
        }

        private async Task SpawnObstacles()
        {
            Random rand = new Random();

            while (player.Health > 0)
            {
                int num = rand.Next(4);
                IObstacleIteraction obstacle = null;

                switch (num)
                {
                    case 0:
                        obstacle = new UsualCactus(MainCanvas);
                        break;
                    case 1:
                        obstacle = new HealingCactus(MainCanvas);
                        break;
                    case 2:
                        obstacle = new BossCactus(MainCanvas);
                        break;
                    case 3:
                        obstacle = new GhostCactus(MainCanvas);
                        break;
                }
                if (obstacle != null)
                {
                    (obstacle as MovingObject)?.StartMoving();
                }
                await Task.Delay(3000);
            }
        }
    }
}