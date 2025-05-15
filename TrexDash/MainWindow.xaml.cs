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
using System.IO;
using System.Text.Json;

namespace TrexDash
{

    public partial class MainWindow : Window
    {
        private List<IObstacleInteraction> activeObstacles = new List<IObstacleInteraction>();
        public int count = 1;
        int charCount = 2;
        private Func<Canvas, Character> characterFactory;
        private Character player;
        private DispatcherTimer collisionTimer;
        private Heart heart;
        private int currScore = 0;
        private int bestScore = 0;
        private DispatcherTimer scoreTimer;
        private const string ScoresFilePath = "scores.json";
        public class GameScores
        {
            public int BestScore { get; set; }
        }
        public MainWindow()
        {
            InitializeComponent();
            MainCanvas.Children.Clear();
            MainCanvas.Children.Add(StartThubmnail);
            MainCanvas.Children.Add(StartButton);
            this.KeyDown += MainWindow_KeyDown;

            collisionTimer = new DispatcherTimer();
            collisionTimer.Interval = TimeSpan.FromMilliseconds(20);
            collisionTimer.Tick += CollisionTimer_Tick;

            LoadBestScore();
            scoreTimer = new DispatcherTimer();
            scoreTimer.Interval = TimeSpan.FromMilliseconds(30);
            scoreTimer.Tick += ScoreTimer_Tick;
        }
        private void CollisionTimer_Tick(object sender, EventArgs e)
        {
            CheckCollisions();
            heart?.Update();
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
            currScore = 0;
            Rectangle line = new Rectangle
            {
                Width = 850,
                Height = 210,
                Fill = new SolidColorBrush(Color.FromArgb(30, 255, 255, 0)),
                Stroke = Brushes.Black,
                StrokeThickness = 2
            }; 
            Canvas.SetTop(line, 320);
            MainCanvas.Children.Add(line);
            
            TextBlock scoreText = new TextBlock
            {
                Name = "ScoreText",
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.Black,
                FontFamily = new FontFamily("Cascadia Code"),
                Text = $"{currScore.ToString("000000")}"
            };
            Canvas.SetRight(scoreText, 20);
            Canvas.SetTop(scoreText, 20);
            MainCanvas.Children.Add(scoreText);
            
            player = characterFactory(MainCanvas);
            heart = new Heart(MainCanvas, player);
            _ = SpawnObstacles();
            collisionTimer.Start();

            scoreTimer.Start();
            UpdateScoreDisplay();
        }
        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space && count == 1)
            {
                count = 0;
                Task.Run(async () =>
                {
                    await player.Jump();
                    count = 1; 
                });
            }
        }
        private async Task SpawnObstacles()
        {
            Random rand = new Random();

            while (player.health > 0)
            {
                IObstacleInteraction obstacle = null;
                int num = rand.Next(4);
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
                    activeObstacles.Add(obstacle);
                    (obstacle as MovingObject)?.StartMoving();
                }

                await Task.Delay(2000);
            }
            if (player.health <= 0) GameOver();
        }
        private void CheckCollisions()
        {
            if (player.health <= 0) return;
            for (int i = activeObstacles.Count - 1; i >= 0; i--)
            {
                var obstacle = activeObstacles[i];
                var movingObstacle = obstacle as MovingObject;

                if (movingObstacle != null && Canvas.GetLeft(movingObstacle.image) < -movingObstacle.image.ActualWidth)
                {
                    MainCanvas.Children.Remove(movingObstacle.image);
                    activeObstacles.RemoveAt(i);
                    continue;
                }
                obstacle.Interact(player);

                if (player.health <= 0)
                {
                    GameOver();
                    break;
                }
            }
        }
        private void GameOver()
        {
            scoreTimer.Stop();
            collisionTimer.Stop();

            if (currScore > bestScore)
            {
                bestScore = currScore;
                SaveBestScore();
            }
            
            foreach (var obstacle in activeObstacles)
            {
                if (obstacle is MovingObject movingObstacle)
                {
                    movingObstacle.StopMoving();
                }
            }
            TextBlock finalScoreText = new TextBlock()
            {
                Text = $"Your score: {currScore}\nBest score: {bestScore}",
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                FontFamily = new FontFamily("Cascadia Code"),
                TextAlignment = TextAlignment.Center
            };
            Canvas.SetLeft(finalScoreText, MainCanvas.ActualWidth / 2 - 100);
            Canvas.SetTop(finalScoreText, MainCanvas.ActualHeight / 2);

            TextBlock text = new TextBlock()
            {
                Text = "GAME OVER",
                FontSize = 64,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                FontFamily = new FontFamily("Bauhaus 93")
            };
            Canvas.SetLeft(text, 250);
            Canvas.SetTop(text, 150);
            Rectangle rect = new Rectangle()
            {
                Width = ActualWidth,
                Height = ActualHeight,
                Fill = Brushes.Black
            };
            Image sadCow = new Image
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/images/SadCow.png")),
                Height = 320,
            };
            Canvas.SetLeft(sadCow, -245);
            Canvas.SetTop(sadCow, 20);
            Image deadDino = new Image
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/images/DeadDino.png")),
                Height = 430,
            };
            Canvas.SetRight(deadDino, -245);
            Canvas.SetTop(deadDino, 230);
            MainCanvas.Children.Add(rect);
            MainCanvas.Children.Add(finalScoreText);
            MainCanvas.Children.Add(text);
            MainCanvas.Children.Add(sadCow);
            MainCanvas.Children.Add(deadDino);

            Button resetScoreButton = new Button
            {
                Content = "↺",
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                Background = Brushes.Transparent,
                FontSize = 20,
                Width = 30,
                Height = 30,
                FontWeight = FontWeights.Bold,
                Style = (Style)FindResource("StartButton")
            };
            resetScoreButton.Click += (s, e) =>
            {
                MainCanvas.Children.Remove(finalScoreText);
                bestScore = 0;
                try
                {
                    if (File.Exists(ScoresFilePath))
                    {
                        File.Delete(ScoresFilePath);
                    }
                }
                catch { }

                TextBlock tempScoreBlock = new TextBlock()
                {
                    Text = $"Your score: {currScore}\nBest score: {0}",
                    FontSize = 20,
                    FontWeight = FontWeights.Bold,
                    Foreground = Brushes.White,
                    FontFamily = new FontFamily("Cascadia Code"),
                    TextAlignment = TextAlignment.Center
                };
                Canvas.SetLeft(tempScoreBlock, MainCanvas.ActualWidth / 2 - 100);
                Canvas.SetTop(tempScoreBlock, MainCanvas.ActualHeight / 2);
                MainCanvas.Children.Add(tempScoreBlock);
            };
            Canvas.SetLeft(resetScoreButton, MainCanvas.ActualWidth / 2 - 142);
            Canvas.SetTop(resetScoreButton, MainCanvas.ActualHeight / 2 + 18);
            MainCanvas.Children.Add(resetScoreButton);

            Button playAgainButton = new Button
            {
                Content = "Play again",
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(3),
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF377D22")),
                FontSize = 16,
                FontFamily = new FontFamily("Consolas"),
                Width = 120,
                Height = 40,
                FontWeight = FontWeights.Bold,
                Style = (Style)FindResource("StartButton")
            };
            playAgainButton.Click += (s, e) =>
            {
                MainCanvas.Children.Clear();
                currScore = 0;
                player.health = 3;
                activeObstacles.Clear();
                PlayButtonClick(null, null);
            };
            Canvas.SetLeft(playAgainButton, MainCanvas.ActualWidth / 2 - 60);
            Canvas.SetTop(playAgainButton, MainCanvas.ActualHeight / 2 + 100);
            MainCanvas.Children.Add(playAgainButton);
        }
        private void ScoreTimer_Tick(object sender, EventArgs e)
        {
            if (player != null && player.health > 0)
            {
                currScore++;
                UpdateScoreDisplay();
            }
        }
        private void UpdateScoreDisplay()
        {
            var scoreText = MainCanvas.Children.OfType<TextBlock>()
        .FirstOrDefault(t => t.Name == "ScoreText");

            if (scoreText != null)
            {
                scoreText.Text = $"{currScore.ToString("000000")}";
            }
        }
        private void LoadBestScore()
        {
            try
            {
                if (File.Exists(ScoresFilePath))
                {
                    string json = File.ReadAllText(ScoresFilePath);
                    var scores = JsonSerializer.Deserialize<GameScores>(json);
                    bestScore = scores?.BestScore ?? 0;
                }
            }
            catch
            {
                bestScore = 0;
            }
        }

        private void SaveBestScore()
        {
            try
            {
                var scores = new GameScores { BestScore = bestScore };
                string json = JsonSerializer.Serialize(scores);
                File.WriteAllText(ScoresFilePath, json);
            }
            catch
            {
                
            }
        }
    }
}