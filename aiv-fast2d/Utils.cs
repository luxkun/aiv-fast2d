using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aiv.Fast2D
{
    public static class Utils
    {
        public static void DrawLinearLine(Texture texture, Vector2 from, Color color, int length, bool vertical)
        {
            for (int p = 0; p < length; p++)
            {
                int x, y;
                if (vertical)
                {
                    x = (int)from.X;
                    y = p;
                } else
                {
                    x = p;
                    y = (int)from.Y;
                }
                int position = (y * texture.Width * 4) + (x * 4);
                texture.Bitmap[position] = color.R;
                texture.Bitmap[position + 1] = color.G;
                texture.Bitmap[position + 2] = color.B;
                texture.Bitmap[position + 3] = color.A;
            }
        }

        internal static void DrawCircle(Texture texture, float radius, Color color, bool fill, int segments = 20, float step = 0.1f)
        {
            for (float r = step; r < radius; r += step)
                for (int i = 0; i < segments; i++)
                {
                    double theta = (2.0 * Math.PI * i) / segments;
                    int x = (int)(r * Math.Cos(theta));
                    int y = (int)(r * Math.Sin(theta));

                    int position = (y * texture.Width * 4) + (x * 4);
                    texture.Bitmap[position] = color.R;
                    texture.Bitmap[position + 1] = color.G;
                    texture.Bitmap[position + 2] = color.B;
                    texture.Bitmap[position + 3] = color.A;
                }
        }

        public static void FillRectangle(Texture texture, Vector2 from, Vector2 to, Color color, bool alphaRest = true)
        {
            if (to.X < 0)
                to.X = 0;
            if (to.Y < 0)
                to.Y = 0;
            if (from.X < 0)
                from.X = 0;
            if (from.Y < 0)
                from.Y = 0;
            for (int y = (int)from.Y; y < (int)to.Y; y++)
            {
                for (int x = (int)from.X; x < (int)to.X; x++)
                {
                    int position = (y * texture.Width * 4) + (x * 4);
                    texture.Bitmap[position] = color.R;
                    texture.Bitmap[position + 1] = color.G;
                    texture.Bitmap[position + 2] = color.B;
                    texture.Bitmap[position + 3] = color.A;
                }
            }
            if (alphaRest)
            {
                // for now supports only X filling
                for (int y = (int)from.Y; y < texture.Height; y++)
                {
                    for (int x = (int)to.X; x < texture.Width; x++)
                    {
                        int position = (y * texture.Width * 4) + (x * 4);
                        texture.Bitmap[position + 3] = 0;
                    }
                }
            }
        }
    }
}
