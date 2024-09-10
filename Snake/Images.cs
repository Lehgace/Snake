using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Snake
{
    // Container for all image assets
    public static class Images
    {
        //public readonly static ImageSource Empty = LoadImage("Empty.png");
        //public readonly static ImageSource Body = LoadImage("Body.png");
        //public readonly static ImageSource Head = LoadImage("Head.png");
        //public readonly static ImageSource Food = LoadImage("Food.png");
        //public readonly static ImageSource DeadBody = LoadImage("DeadBody.png");
        //public readonly static ImageSource DeadHead = LoadImage("DeadHead.png");

        public readonly static ImageSource Empty = LoadImage("EmptyWhite.png");
        public readonly static ImageSource Food = LoadImage("RedPokerChip.png");
        public readonly static ImageSource Body = LoadImage("HandBody.png");
        public readonly static ImageSource Head = LoadImage("BlueCardBackground.png");
        public readonly static ImageSource DeadBody = LoadImage("BlueCardBackground.png");
        public readonly static ImageSource DeadHead = LoadImage("BlueCardBackground.png");

        private static ImageSource LoadImage(string fileName)
        {
            // Load image with given filename and return as image source
            return new BitmapImage(new Uri($"Assets/{fileName}", UriKind.Relative));
        }
    }
}
