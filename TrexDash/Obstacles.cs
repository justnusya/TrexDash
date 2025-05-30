﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Threading;
using System.Timers;
using System.Windows.Threading;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Documents;
using System.Windows.Shapes;
namespace TrexDash
{
    public abstract class MovingObject
    {
        public event Action<MovingObject> OnDestroyed;
        private bool isMoving = true;
        protected int x = 780;
        protected int y = 260;
        protected int height = 70;
        public Image image;
        protected Canvas mainCanvas;
        public int speed { get; set; } = 10;
        protected MovingObject(Canvas mainCanvas, string imagePath)
        {
            this.mainCanvas = mainCanvas;
            this.speed= speed;
            image = new Image
            {
                Source = new BitmapImage(new Uri(imagePath)),
                Height = height
            };
            Canvas.SetLeft(image, x);
            Canvas.SetTop(image, y);
            mainCanvas.Children.Add(image);
        }
        public void StartMoving()
        {
            Task.Run(async () => await MoveLeft());
        }
        public void StopMoving()
        {
            isMoving = false;
        }
        private async Task MoveLeft()
        {
            while (isMoving)
            {
                x -= 10;

                await image.Dispatcher.InvokeAsync(() =>
                {
                    Canvas.SetLeft(image, x);
                });

                if (x < -image.ActualWidth)
                {
                    
                    await image.Dispatcher.InvokeAsync(() =>
                    {
                        mainCanvas.Children.Remove(image);
                    });
                    OnDestroyed?.Invoke(this);
                    break;
                }
               
                await Task.Delay(20);
            }
        }
        protected bool isCollided(Character character)
        {
            double charLeft = Canvas.GetLeft(character.image);
            double charTop = Canvas.GetTop(character.image);
            double charRight = charLeft + character.image.ActualWidth;
            double charBottom = charTop + character.image.ActualHeight;

            double cactusLeft = Canvas.GetLeft(this.image);
            double cactusTop = Canvas.GetTop(this.image);
            double cactusRight = cactusLeft + this.image.ActualWidth;
            double cactusBottom = cactusTop + this.image.ActualHeight;

            bool isColliding = !(charRight < cactusLeft ||
                                charLeft > cactusRight ||
                                charBottom < cactusTop ||
                                charTop > cactusBottom);
            return isColliding;
        }
    }
    public class GhostCactus : MovingObject, IObstacleInteraction
    {
        public GhostCactus(Canvas mainCanvas)
        : base(mainCanvas, "pack://application:,,,/images/Coloured%20Ghost%20cactus%201.png") { }
        public void Interact(Character character)
        {
            if (isCollided(character))
            {
                var textBox = new TextBox
                {
                    Text = "Boo!",
                    FontSize = 44,
                    FontWeight = FontWeights.Bold,
                    Background = Brushes.Transparent,
                    Foreground = Brushes.White,
                    BorderThickness = new Thickness(0),
                    IsReadOnly = true
                };

                double left = mainCanvas.ActualWidth / 2 - 30;
                double top = 200;
                Canvas.SetLeft(textBox, left);
                Canvas.SetTop(textBox, top);

                Rectangle rect = new Rectangle
                {
                    Height = 560,
                    Width = 850,
                    Fill = Brushes.Black,
                    Opacity = 0.7
                };
                mainCanvas.Dispatcher.Invoke(() =>
                {
                    mainCanvas.Children.Add(rect);
                    mainCanvas.Children.Add(textBox);
                    
                });
                Task.Run(async () =>
                {
                    await Task.Delay(500);
                    textBox.Dispatcher.Invoke(() =>
                    {
                        mainCanvas.Children.Remove(textBox);
                        mainCanvas.Children.Remove(rect);
                    });
                });
            }
        }
    }
    public class HealingCactus : MovingObject, IObstacleInteraction
    {
        public HealingCactus(Canvas mainCanvas)
        : base(mainCanvas, "pack://application:,,,/images/Coloured%20healing%20cactus.png") { }
        public void Interact(Character character)
        {
            if (!character.IsVulnerable) return;

            if (isCollided(character))
            {
                character.IncreaseHealth();
            }
        }
    }
    public class BossCactus : MovingObject, IObstacleInteraction
    {
        public BossCactus(Canvas mainCanvas)
        : base(mainCanvas, "pack://application:,,,/images/Coloured%20sharp%20cactus%201.png") { }
        public void Interact(Character character)
        {
            if (isCollided(character))
            {
                character.SetHealth(0);
            }
        }
    }
    public class UsualCactus : MovingObject, IObstacleInteraction
    {

        private static Random rand = new Random();
        public UsualCactus(Canvas mainCanvas)
        : base(mainCanvas, GetImagePath()) { }
        private static string GetImagePath()
        {
            int choice = rand.Next(2);
            if(choice == 0)
                return "pack://application:,,,/images/Coloured%20sharp%20cactus.png";
            else
                return "pack://application:,,,/images/Coloured%20sharp%20cactus%202.png";
        }
        public void Interact(Character character)
        {
            if (!character.IsVulnerable) return;
            
            if (isCollided(character))
            {
                character.DecreaseHealth();
                RedFlashing();
            }
        }
        private async void RedFlashing()
        {
            Rectangle rect = new Rectangle
            {
                Height = 560,
                Width = 850,
                Fill = Brushes.Red,
                Opacity = 0.4
            };

            mainCanvas.Children.Add(rect);
            await Task.Delay(200);
            mainCanvas.Children.Remove(rect);
            await Task.Delay(150);
            mainCanvas.Children.Add(rect);
            await Task.Delay(200);
            mainCanvas.Children.Remove(rect);
        }
    }
    public interface IObstacleInteraction
    {
        void Interact(Character character);
    }
}