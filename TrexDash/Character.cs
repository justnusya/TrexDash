using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Threading;

namespace TrexDash
{
    public abstract class Character
    {
        protected int x=40;
        protected int y=260;
        protected int height = 70;
        public int health=3;
        protected double currentYPosition = 0;
        protected int jumpHeight = 25;
        protected int jumpSpeed = 5;
        public Image image;
        protected Heart heart;
        protected bool isVulnerable = true;
        protected DispatcherTimer invulnerabilityTimer;

        public bool IsVulnerable => isVulnerable;
        protected Character(Canvas mainCanvas, string imagePath)
        {
            invulnerabilityTimer = new DispatcherTimer();
            invulnerabilityTimer.Interval = TimeSpan.FromSeconds(1); 
            invulnerabilityTimer.Tick += (s, e) =>
            {
                isVulnerable = true;
                invulnerabilityTimer.Stop();
            };

            currentYPosition = y;
            image = new Image
            {
                Source = new BitmapImage(new Uri(imagePath)),
                Height = height
            };
            Canvas.SetLeft(image, x);
            Canvas.SetTop(image, y);
            mainCanvas.Children.Add(image);
            heart = new Heart(mainCanvas, this);
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
        public void DecreaseHealth()
        {
            if (!isVulnerable) return;

            health--;
            heart?.Update();

            isVulnerable = false;
            invulnerabilityTimer.Start();
        }
        public void IncreaseHealth()
        {
            if (health < 3) health++;
            heart?.Update();
        }
        public void SetHealth(int value)
        {
            health = value;
            heart?.Update();
        }
    }
    public class Dino : Character
    {
        public Dino(Canvas mainCanvas)
        : base(mainCanvas, "pack://application:,,,/images/Final%20Coloured%20Dino.png") { }
    }
    public class Cow : Character
    {
        public Cow(Canvas mainCanvas)
        : base(mainCanvas, "pack://application:,,,/images/Cow.png") { }
    }
    public class UFO : Character
    {
        public UFO(Canvas mainCanvas)
        : base(mainCanvas, "pack://application:,,,/images/UFO.png") { }
    }
    public class Heart
    {
        private int x = 15;
        private int y = 15;
        public Image image;
        private Canvas mainCanvas;
        private Character character;

        private string[] heartImages =
        {
            "pack://application:,,,/images/0hearts.png",
            "pack://application:,,,/images/1heart.png",
            "pack://application:,,,/images/2hearts.png",
            "pack://application:,,,/images/3hearts.png"
        };
        public Heart(Canvas mainCanvas, Character charac)
        {
            this.mainCanvas = mainCanvas;
            this.character = charac;

            image = new Image()
            {
                Height = 30
            };
            Canvas.SetLeft(image, x);
            Canvas.SetTop(image, y);
            mainCanvas.Children.Add(image);
            Update();
        }
        public void Update()
        {
            int health = character.health;
            if (health < 0)
                health = 0;
            else if (health >= heartImages.Length)
                health = heartImages.Length - 1;
            image.Source = new BitmapImage(new Uri(heartImages[health], UriKind.RelativeOrAbsolute));
        }
    }
}