using System;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace Snake
{
    public static class Images
    {
        public readonly static ImageSource Empty = LoadImage("Empty");      
        public readonly static ImageSource Head = LoadImage("Head");
        public readonly static ImageSource Tail = LoadImage("Body");
        public readonly static ImageSource Food = LoadImage("Food");
        public readonly static ImageSource Wall = LoadImage("DeadBody");
        public readonly static ImageSource Snake = LoadImage("DeadHead");

        private static ImageSource LoadImage(string fileName)
        {
            return new BitmapImage(new Uri($"Assets/{fileName}.png", UriKind.Relative));
        }
    }
}
