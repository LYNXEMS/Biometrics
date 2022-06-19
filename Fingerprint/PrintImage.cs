using System.Drawing;

namespace Biometrics;

internal class PrintImage
{

    public int ResolutionX;
    public int ResolutionY;
    public string? name;
    private Bitmap image;
    private bool[,]? binarized;

    public bool[,] Binarized
    {
        get => binarized ?? new bool[0, 0];
        set
        {
            binarized = value;
            image = (Bitmap)Helpers.Bool2Image(value);
        }
    }

    public Bitmap Image
    {
        get => image;
        set
        {
            image = value;
            binarized = Helpers.Image2Bool(value);
        }
    }

    public PrintImage(int resolutionX, int resolutionY, string name)
    {
        ResolutionX = resolutionX;
        ResolutionY = resolutionY;
        this.name = name;

        image = new Bitmap(resolutionY, resolutionX);
    }

    public PrintImage(string file)
    {
        image = (Bitmap)System.Drawing.Image.FromFile(file);
        binarized = Helpers.Image2Bool(Image);
        ResolutionX = Image.Width;
        ResolutionY = Image.Height;

        name = file;
    }

    public PrintImage(Image img)
    {
        image = (Bitmap)img;
        binarized = Helpers.Image2Bool(Image);
        ResolutionX = img.Width;
        ResolutionY = img.Height;
    }

    public void SaveImage(string? named = "")
    {
        if (string.IsNullOrWhiteSpace(named))
        {
            named = name;
        }

        if (named != null)
            Image.Save(named);
    }

    public void SaveImageBinarized(string? named = "")
    {
        if (string.IsNullOrEmpty(named))
        {
            named = name;
        }

        if (named != null)
        {
            var toSave = Helpers.Bool2Image(Binarized);
            toSave.Save(named);
        }
    }
}