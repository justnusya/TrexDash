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
        public abstract void Jump();
        protected void DecreaseHealth()
        {
            health -= 1;
        }
        protected void IncreaseHealth()
        {
            if (health < 3)
            {
                health++;
            }
        }
        protected void SetHealth(int value)
        {
            health = value;
        }
    }
    public class Dino : Character
    {
        private Image image { get; set; }
        public Dino(Canvas MainCanvas)
        {
            image = new Image
            {
                Source = new BitmapImage(new Uri("C:\\Users\\Legion\\source\\repos\\TrexDash\\TrexDash\\Final Coloured Dino.png")),
                Height = height
            };
            Canvas.SetLeft(image, x);
            Canvas.SetTop(image, y);
            MainCanvas.Children.Add(image);
        }
        public override void Jump()
        {
            double jumpHeight = 20;
            double jumpSpeed = 2;

            for (int i = 0; i < jumpHeight; i++)
            {
                currentYPosition -= jumpSpeed;
                image.SetValue(Canvas.TopProperty, currentYPosition);
                Thread.Sleep(20);
            }
            for (int i = 0; i < jumpHeight; i++)
            {
                currentYPosition += jumpSpeed;
                image.SetValue(Canvas.TopProperty, currentYPosition);
                Thread.Sleep(20);
            }
        }
    }
}
