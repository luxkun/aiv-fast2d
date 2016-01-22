using System;
using System.Collections.Generic;
using System.Drawing;
using OpenTK;

namespace Aiv.Fast2D
{
    public class TextObject
    {
        private bool disposed;
        public Texture FontTexture { get; set; }
        private bool initialized = false;

        private Tuple<string, Vector2, Color> lastDraw;

        private Sprite[] sprites;

        public Vector2 Scale { get; set; }
        public Color Color { get; set; }
        public int Alpha { get; set; } = -1; // sets the whole text's alpha

        public Color FontBaseColor { get; set; } = Color.White;

        public string Text { get; set; } = "";
        //char, from position, size
        public Dictionary<char, Tuple<Vector2, Vector2>> CharToSprite { get; set; }

        public Vector2 Position { get; set; }

        public bool StaticColor { get; set; }

        public float Padding { get; set; }
        // padding is ignored if paddingfunc != null
        public Func<float, float> PaddingFunc { get; set; }
        public float SpaceWidth { get; set; }
        public float SpaceHeight { get; set; }

        public float ScaledPadding { get; private set; }

        // 0: no animation
        // 1: padding animation
        //public int SpawnAnimation;
        //private float paddingAnimationStep = 0.9f;

        private void Init()
        {
            if (lastDraw == null)
                lastDraw = Tuple.Create("", Vector2.Zero, Color.White);
            if (!initialized)
            {
                initialized = true;
                if ((Color != FontBaseColor && !StaticColor) || (Alpha >= 0 && Alpha < 255))
                {
                    for (var y = 0; y < FontTexture.Height; y++)
                    {
                        for (var x = 0; x < FontTexture.Width; x++)
                        {
                            var position = y* FontTexture.Width*4 + x*4;
                            if (!StaticColor && Color != FontBaseColor &&
                                FontTexture.Bitmap[position] == FontBaseColor.R && FontTexture.Bitmap[position + 1] == FontBaseColor.G &&
                                FontTexture.Bitmap[position + 2] == FontBaseColor.B &&
                                FontTexture.Bitmap[position + 3] == FontBaseColor.A)
                            {
                                FontTexture.Bitmap[position] = Color.R;
                                FontTexture.Bitmap[position + 1] = Color.G;
                                FontTexture.Bitmap[position + 2] = Color.B;
                                FontTexture.Bitmap[position + 3] = Color.A;
                            }
                            if (Alpha >= 0 && Alpha < 255 && FontTexture.Bitmap[position + 3] > 20)
                                FontTexture.Bitmap[position + 3] = (byte) Alpha;
                        }
                    }
                    FontTexture.Update();
                }
            }
            ScaledPadding = Padding*Scale.X;

            if (lastDraw.Item1 != Text)
            {
                if (sprites != null)
                    Dispose();
                disposed = false;
                sprites = new Sprite[Text.Length];
                for (var i = 0; i < Text.Length; i++)
                {
                    var c = Text[i];
                    if (c == ' ')
                    {
                        sprites[i] = null;
                        continue;
                    }
                    sprites[i] = new Sprite(
                        (int) CharToSprite[c].Item2.X,
                        (int) CharToSprite[c].Item2.Y
                        );
                }
            }
            lastDraw = Tuple.Create(Text, Scale, Color);
        }

        public Vector2 Measure()
        {
            Init();
            var width = 0f;
            var height = 0f;
            for (var i = 0; i < Text.Length; i++)
            {
                var c = Text[i];
                float h;
                if (c == ' ')
                {
                    width += SpaceWidth*Scale.X +
                             (PaddingFunc == null ? ScaledPadding : PaddingFunc(SpaceWidth*Scale.X));
                    h = SpaceHeight*Scale.Y;
                }
                else
                {
                    //width += CharToSprite[c].Item2.X + ((i + 1) < Text.Length && SpawnAnimation != 1 ? Padding : 1);
                    //h = CharToSprite[c].Item2.Y;
                    width += sprites[i].Width*Scale.X +
                             (PaddingFunc == null ? ScaledPadding : PaddingFunc(sprites[i].Width*Scale.X));
                    //((i + 1) < Text.Length && SpawnAnimation != 1 ? ScaledPadding : 1);
                    h = sprites[i].Height*Scale.Y;
                }
                if (h > height)
                    height = h;
            }
            //return Vector2.Multiply(new Vector2(width, height), Scale);
            return new Vector2(width, height);
        }

        public void Draw()
        {
            Init();

            var position = new Vector2(Position.X, Position.Y);
            for (var i = 0; i < Text.Length; i++)
            {
                var c = Text[i];
                if (c == ' ')
                {
                    position.X += SpaceWidth*Scale.X +
                                  (PaddingFunc == null ? ScaledPadding : PaddingFunc(SpaceWidth*Scale.X));
                    continue;
                }
                sprites[i].scale = Scale;
                sprites[i].position = new Vector2(position.X, position.Y);
                sprites[i].DrawTexture(
                    FontTexture, (int) CharToSprite[c].Item1.X, (int) CharToSprite[c].Item1.Y,
                    (int) CharToSprite[c].Item2.X, (int) CharToSprite[c].Item2.Y);

                position.X += sprites[i].Width*Scale.X +
                              (PaddingFunc == null ? ScaledPadding : PaddingFunc(sprites[i].Width*Scale.X));
            }
        }

        public void Dispose()
        {
            if (disposed)
                return;
            foreach (var sprite in sprites)
                sprite?.Dispose();
        }

        public TextObject Clone()
        {
            return (TextObject) MemberwiseClone();
        }
    }
}