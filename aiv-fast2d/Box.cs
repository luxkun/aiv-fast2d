using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace Aiv.Fast2D
{
	public class Box : Sprite
	{
        public Color Color { get; set; }

        public bool Fill { get; set; }

        private Tuple<bool, int, int, Color> lastRedrawInfo;
        private Texture cachedTexture;

        public Box(int width, int height) : base(width, height)
        {
            cachedTexture = new Texture(width, height);
            lastRedrawInfo = Tuple.Create(false, 0, 0, Color.White);
        }

        public void Draw()
        {
            if (Width <= 0 || Height <= 0)
                return;
            if (lastRedrawInfo.Item1 != Fill || lastRedrawInfo.Item2 != Width ||
                lastRedrawInfo.Item3 != Height || lastRedrawInfo.Item4 != Color) // redraw
            {
                if (Fill)
                {
                    Utils.FillRectangle(cachedTexture, Vector2.Zero, new Vector2(cachedTexture.Width, cachedTexture.Height), Color);
                }
                else
                {
                    // top
                    Utils.DrawLinearLine(cachedTexture, Vector2.Zero, Color, Width, false);
                    // bottom
                    Utils.DrawLinearLine(cachedTexture, new Vector2(0, cachedTexture.Height - 1), Color, Width, false);
                    // left
                    Utils.DrawLinearLine(cachedTexture, Vector2.Zero, Color, Height, true);
                    // right
                    Utils.DrawLinearLine(cachedTexture, new Vector2(cachedTexture.Width - 1, 0), Color, Height, true);
                }
                lastRedrawInfo = Tuple.Create(Fill, Width, Height, Color);
                cachedTexture.Update();
            }
            DrawTexture(cachedTexture);
        }

        public Box Clone()
        {
            return (Box)MemberwiseClone();
        }
	}
}

