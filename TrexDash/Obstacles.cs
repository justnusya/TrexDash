using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace TrexDash
{
    public abstract class MovingObject
    {
        protected int x = 500;
        protected int y = 260;
        protected int height = 70;
        protected Image image;
        protected Canvas mainCanvas;
        public int speed { get; set; } = 2;
        protected MovingObject(Canvas mainCanvas, string imagePath)
        {
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
        public void MoveLeft(int deltaTime)
        {
            x -= speed * deltaTime;
            Canvas.SetLeft(image, x);
        }
    }
    public class GhostCactus : MovingObject
    {
        public GhostCactus(Canvas mainCanvas)
        : base(mainCanvas, "C:\\Users\\Legion\\source\\repos\\TrexDash\\TrexDash\\Coloured Ghost cactus 1.png") { }
    }
    public class HealingCactus : MovingObject, IObstacleIteraction
    {
        public HealingCactus(Canvas mainCanvas)
        : base(mainCanvas, "C:\\Users\\Legion\\source\\repos\\TrexDash\\TrexDash\\Coloured healing cactus.png") { }
        public void Interact(Character character)
        {
            double charLeft = Canvas.GetLeft(character.image);
            double charTop = Canvas.GetTop(character.image);
            double charRight = charLeft + character.image.ActualWidth;
            double charBottom = charTop + character.image.ActualHeight;

            double cactusLeft = Canvas.GetLeft(this.image);
            double cactusTop = Canvas.GetTop(this.image);
            double cactusRight = cactusLeft + this.image.ActualWidth;
            double cactusBottom = cactusTop + this.image.ActualHeight;

            bool isColliding = !(charRight < cactusLeft || charLeft > cactusRight ||
                                 charBottom < cactusTop || charTop > cactusBottom);

            if (isColliding)
            {
                var decreaseHealthMethod = character.GetType().GetMethod("IncreaseHealth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                decreaseHealthMethod?.Invoke(character, null);
            }
        }
    }
    public class BossCactus : MovingObject, IObstacleIteraction
    {
        public BossCactus(Canvas mainCanvas)
        : base(mainCanvas, "C:\\Users\\Legion\\source\\repos\\TrexDash\\TrexDash\\Coloured sharp cactus 1.png") { }
        public void Interact(Character character)
        {
            double charLeft = Canvas.GetLeft(character.image);
            double charTop = Canvas.GetTop(character.image);
            double charRight = charLeft + character.image.ActualWidth;
            double charBottom = charTop + character.image.ActualHeight;

            double cactusLeft = Canvas.GetLeft(this.image);
            double cactusTop = Canvas.GetTop(this.image);
            double cactusRight = cactusLeft + this.image.ActualWidth;
            double cactusBottom = cactusTop + this.image.ActualHeight;

            bool isColliding = !(charRight < cactusLeft || charLeft > cactusRight ||
                                 charBottom < cactusTop || charTop > cactusBottom);

            if (isColliding)
            {
                var decreaseHealthMethod = character.GetType().GetMethod("SetHealth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                decreaseHealthMethod?.Invoke(character, new object[] { 0 });
            }
        }
    }
    public class UsualCactus : MovingObject, IObstacleIteraction
    {
        private static Random rand = new Random();
        public UsualCactus(Canvas mainCanvas)
        : base(mainCanvas, GetImagePath()) { }
        private static string GetImagePath()
        {
            int choice = rand.Next(2);
            if(choice == 0)
                return "C:\\Users\\Legion\\source\\repos\\TrexDash\\TrexDash\\Coloured sharp cactus.png";
            else
                return "C:\\Users\\Legion\\source\\repos\\TrexDash\\TrexDash\\Coloured sharp cactus 2.png";
        }
        public void Interact(Character character)
        {
            double charLeft = Canvas.GetLeft(character.image);
            double charTop = Canvas.GetTop(character.image);
            double charRight = charLeft + character.image.ActualWidth;
            double charBottom = charTop + character.image.ActualHeight;

            double cactusLeft = Canvas.GetLeft(this.image);
            double cactusTop = Canvas.GetTop(this.image);
            double cactusRight = cactusLeft + this.image.ActualWidth;
            double cactusBottom = cactusTop + this.image.ActualHeight;

            bool isColliding = !(charRight < cactusLeft || charLeft > cactusRight ||
                                 charBottom < cactusTop || charTop > cactusBottom);

            if (isColliding)
            {
                var decreaseHealthMethod = character.GetType().GetMethod("DecreaseHealth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                decreaseHealthMethod?.Invoke(character, null);
            }
        }
    }
    public interface IObstacleIteraction
    {
        void Interact(Character character);
    }
}
