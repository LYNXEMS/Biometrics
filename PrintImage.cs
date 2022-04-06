using System.Drawing;

namespace Biometrics
{
    internal class PrintImage
    {

        public int ResolutionX;
        public int ResolutionY;
        public string name;
        public Bitmap image;
        public bool[][] binarized;

        public PrintImage(int resolutionX, int resolutionY, string name)
        {
            ResolutionX = resolutionX;
            ResolutionY = resolutionY;
            this.name = name;

            image = new Bitmap(resolutionY, resolutionX);
        }

        public PrintImage(string file)
        {
            image = Helpers.CreateNonIndexedImage(Image.FromFile(file));
            image = Helpers.Binarization(image);
            binarized = Helpers.Image2Bool(image);
            ResolutionX = image.Width;
            ResolutionY = image.Height;

            this.name = file;
        }

        public PrintImage(Image img)
        {
            image = (Bitmap?)img;
            ResolutionX = img.Width;
            ResolutionY= img.Height;
        }

        public void SaveImage(string named = "")
        {
            if (string.IsNullOrEmpty(named))
            {
                named = name;
            }

            image.Save(named);
        }

        public void SaveImageBinarized(string named = "")
        {
            if (string.IsNullOrEmpty(named))
            {
                named = name;
            }
            var toSave = Helpers.Bool2Image(binarized);
            toSave.Save(named);
        }
    }
}
