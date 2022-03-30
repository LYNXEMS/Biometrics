using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biometrics
{
    internal class Image
    {

        public int ResolutionX;
        public int ResolutionY;
        public string name;
        public Bitmap image;

        public Image(int resolutionX, int resolutionY, string name)
        {
            ResolutionX = resolutionX;
            ResolutionY = resolutionY;
            this.name = name;

            image = new Bitmap(resolutionY, resolutionX);
        }

        public Image(string file)
        {
            image = Helpers.CreateNonIndexedImage(System.Drawing.Image.FromFile("your image.tif"));
            ResolutionX = image.Height;
            ResolutionY = image.Width;

            this.name = file;
        }

        public void SaveImage()
        {
            image.Save(name);
        }
    }
}
