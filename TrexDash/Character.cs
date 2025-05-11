using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace TrexDash
{
    public abstract class Character
    {
        protected int x=40;
        protected int y=260;
        protected int height = 70;
        protected int health=3;
        protected double currentYPosition = 0;
        protected int jumpHeight = 25;
        protected int jumpSpeed = 5;
        public Image image;

        public int Health
        {
            get => health;
            internal set => health = value;
        }

        protected Character(Canvas mainCanvas, string imagePath)
        {
            currentYPosition = y;
            image = new Image
            {
                Source = new BitmapImage(new Uri(imagePath)),
                Height = height
            };
            Canvas.SetLeft(image, x);
            Canvas.SetTop(image, y);
            mainCanvas.Children.Add(image);
        }
        public virtual async Task Jump()
        {
            for (int i = 0; i < jumpHeight; i++)
            {
                currentYPosition -= jumpSpeed;
                image.Dispatcher.Invoke(() =>
                {
                    image.SetValue(Canvas.TopProperty, currentYPosition);
                });

                await Task.Delay(jumpSpeed*3);
            }
            for (int i = 0; i < jumpHeight; i++)
            {
                currentYPosition += jumpSpeed;

                image.Dispatcher.Invoke(() =>
                {
                    image.SetValue(Canvas.TopProperty, currentYPosition);
                });

                await Task.Delay(jumpSpeed * 3);
            }
        }
        protected void DecreaseHealth() => health--;
        protected void IncreaseHealth() { if (health < 3) health++; }
        protected void SetHealth(int value) => health = value;
    }
    public class Dino : Character
    {
        public Dino(Canvas mainCanvas)
        : base(mainCanvas, "C:\\Users\\Legion\\source\\repos\\TrexDash\\TrexDash\\Final Coloured Dino.png") { }
    }
    public class Cow : Character
    {
        public Cow(Canvas mainCanvas)
        : base(mainCanvas, "C:\\Users\\Legion\\source\\repos\\TrexDash\\TrexDash\\Cow.png") { }
    }
    public class UFO : Character
    {
        public UFO(Canvas mainCanvas)
        : base(mainCanvas, "C:\\Users\\Legion\\source\\repos\\TrexDash\\TrexDash\\UFO.png") { }
    }
}