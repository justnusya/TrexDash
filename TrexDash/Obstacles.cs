using System;
using System.Collections.Generic;
using System.Linq;
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
    public class HealingCactus : MovingObject
    {
        public HealingCactus(Canvas mainCanvas)
        : base(mainCanvas, "C:\\Users\\Legion\\source\\repos\\TrexDash\\TrexDash\\Coloured healing cactus.png") { }
    }
    public class BossCactus : MovingObject
    {
        public BossCactus(Canvas mainCanvas)
        : base(mainCanvas, "C:\\Users\\Legion\\source\\repos\\TrexDash\\TrexDash\\Coloured sharp cactus 1.png") { }
    }
    public class UsualCactus : MovingObject
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
    }
}
