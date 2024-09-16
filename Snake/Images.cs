using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Snake
{
    // Container for all image assets
    public static class Images
    {
        public readonly static ImageSource Empty = LoadImage("EmptyGreen.png");
        public readonly static ImageSource Food = LoadImage("PurplePokerChip.png");
        public readonly static ImageSource Body = LoadImage("Spade.png");
        public readonly static ImageSource Body2 = LoadImage("Diamond.png");
        public readonly static ImageSource Body3 = LoadImage("Club.png");
        public readonly static ImageSource Body4 = LoadImage("Heart.png");
        public readonly static ImageSource Head = LoadImage("SimpleCard.png");
        public readonly static ImageSource DeadBody = LoadImage("SimpleCard.png");
        public readonly static ImageSource DeadHead = LoadImage("SimpleCard.png");

        private static ImageSource LoadImage(string fileName)
        {
            // Load image with given filename and return as image source
            return new BitmapImage(new Uri($"Assets/{fileName}", UriKind.Relative));
        }
    }
}
