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
        image = new Bitmap(file);
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
    
    public int RidgeCount(Minutia first, Minutia second)
    {
        int count = 0;
        var points = Bresenham(first.X, first.Y, second.X, second.Y);
        int i = 0;
        
        while (i < points.Count)
        {
            int j = i;
            if (i == 0)
            {
                while (i < points.Count && PixelEnviroment(points[i]))
                    i++;
                j = i;
                if (i >= points.Count)
                    break;
            }
            while (j < points.Count && PixelEnviroment(points[j]) == false)
                j++;
            i = j;
            if (i >= points.Count)
                break;
            while (i < points.Count && PixelEnviroment(points[i]))
                i++;
            if (i >= points.Count)
                break;
            count++;
        }
        
        return count;
    }

        private bool PixelEnviroment(Point p)
        {
            if (binarized == null) return false;
            
            if (binarized[p.X - 1, p.Y - 1]) return true;
            if (binarized[p.X - 1, p.Y]) return true;
            if (binarized[p.X - 1, p.Y + 1]) return true;
            if (binarized[p.X, p.Y - 1]) return true;
            if (binarized[p.X, p.Y]) return true;
            if (binarized[p.X, p.Y + 1]) return true;
            if (binarized[p.X + 1, p.Y - 1]) return true;
            if (binarized[p.X + 1, p.Y]) return true;
            if (binarized[p.X + 1, p.Y + 1]) return true;

            return false;
        }

        private List<Point> Bresenham(int x0, int y0, int x1, int y1)
        {
            List<Point> pixels = new List<Point>();
            int x, y, dx, dy, p, incE, incNE, stepx, stepy;
            dx = (x1 - x0);
            dy = (y1 - y0);
            if (dy < 0)
            {
                dy = -dy; stepy = -1;
            }
            else
                stepy = 1;
            if (dx < 0)
            {
                dx = -dx; stepx = -1;
            }
            else
                stepx = 1;
            x = x0;
            y = y0;
            pixels.Add(new Point(x, y));
            if (dx > dy)
            {
                p = 2 * dy - dx;
                incE = 2 * dy;
                incNE = 2 * (dy - dx);
                while (x != x1)
                {
                    x = x + stepx;
                    if (p < 0)
                    {
                        p = p + incE;
                    }
                    else
                    {
                        y = y + stepy;
                        p = p + incNE;
                    }
                    pixels.Add(new Point(x, y));
                }
            }
            else
            {
                p = 2 * dx - dy;
                incE = 2 * dx;
                incNE = 2 * (dx - dy);
                while (y != y1)
                {
                    y = y + stepy;
                    if (p < 0)
                    {
                        p = p + incE;
                    }
                    else
                    {
                        x = x + stepx;
                        p = p + incNE;
                    }
                    pixels.Add(new Point(x, y));
                }
            }
            return pixels;
        }
}