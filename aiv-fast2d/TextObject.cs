using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Collections.Generic;

namespace Aiv.Fast2D
{
	public class TextObject
	{
        public string FontFile { get; set; }

        public Vector2 Scale { get; set; }
        public Color Color { get; set; }
        public int Alpha { get; set; } = -1; // sets the whole text's alpha

        public Color FontBaseColor { get; set; } = Color.White;

        public string Text { get; set; } = "";

        private Sprite[] sprites;
        private Texture font;
        //char, from position, size
        public Dictionary<char, Tuple<Vector2, Vector2>> CharToSprite { get; set; }

        public Vector2 Position { get; set; }

        private Tuple<string, Vector2, Color> lastDraw;

        public float Padding { get; set; }
        // padding is ignored if paddingfunc != null
        public Func<float, float> PaddingFunc { get; set; }
        public float SpaceWidth { get; set; }
        public float SpaceHeight { get; set; }

        // 0: no animation
        // 1: padding animation
        //public int SpawnAnimation;
        //private float paddingAnimationStep = 0.9f;

        private void Init()
        {
            if (lastDraw == null)
                lastDraw = Tuple.Create("", Vector2.Zero, Color.White);
            if (font == null)
            {
                font = new Texture(FontFile);
                if (Color != FontBaseColor || Alpha >= 0 && Alpha < 255)
                { 
                    for (int y = 0; y < font.Height; y++)
                    {
                        for (int x = 0; x < font.Width; x++)
                        {
                            int position = (y * font.Width * 4) + (x * 4);
                            if (Color != FontBaseColor &&
                                font.Bitmap[position] == FontBaseColor.R && font.Bitmap[position + 1] == FontBaseColor.G &&
                                font.Bitmap[position + 2] == FontBaseColor.B && font.Bitmap[position + 3] == FontBaseColor.A)
                            {
                                font.Bitmap[position] = Color.R;
                                font.Bitmap[position + 1] = Color.G;
                                font.Bitmap[position + 2] = Color.B;
                                font.Bitmap[position + 3] = Color.A;
                            }
                            if (Alpha >= 0 && Alpha < 255 && font.Bitmap[position + 3] > 20)
                                font.Bitmap[position + 3] = (byte)Alpha;
                        }
                    }
                    font.Update();
                }
            }
            if (lastDraw.Item1 != Text || lastDraw.Item2 != Scale)
            {
                ScaledPadding = Padding*Scale.X;
                sprites = new Sprite[Text.Length];
                for (int i = 0; i < Text.Length; i++)
                {
                    var c = Text[i];
                    if (c == ' ')
                    {
                        sprites[i] = null;
                        continue;
                    }
                    sprites[i] = new Sprite(
                        (int)(CharToSprite[c].Item2.X * Scale.X),
                        (int)(CharToSprite[c].Item2.Y * Scale.Y)
                    );
                    sprites[i].scale = Scale;
                }
                lastDraw = Tuple.Create(Text, Scale, Color);
            }
        }

	    public float ScaledPadding { get; private set; }

	    public Vector2 Measure()
        {
            Init();
            float width = 0f;
            float height = 0f;
            for (int i = 0; i < Text.Length; i++)
            {
                var c = Text[i];
                float h;
                if (c == ' ')
                {
                    width += SpaceWidth * Scale.X + (PaddingFunc == null ? ScaledPadding : PaddingFunc(SpaceWidth * Scale.X));
                    h = SpaceHeight * Scale.Y;
                }
                else {
                    //width += CharToSprite[c].Item2.X + ((i + 1) < Text.Length && SpawnAnimation != 1 ? Padding : 1);
                    //h = CharToSprite[c].Item2.Y;
                    width += sprites[i].Width + (PaddingFunc == null ? ScaledPadding : PaddingFunc(SpaceWidth * Scale.X));
                        //((i + 1) < Text.Length && SpawnAnimation != 1 ? ScaledPadding : 1);
                    h = sprites[i].Height;
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
            for (int i = 0; i < Text.Length; i++)
            {
                var c = Text[i];
                if (c == ' ')
                {
                    position.X += SpaceWidth * Scale.X + (PaddingFunc == null ? ScaledPadding : PaddingFunc(SpaceWidth * Scale.X));
                    continue;
                }
                sprites[i].position = new Vector2(position.X, position.Y);
                sprites[i].DrawTexture(
                    font, (int)CharToSprite[c].Item1.X, (int)CharToSprite[c].Item1.Y,
                    (int)CharToSprite[c].Item2.X, (int)CharToSprite[c].Item2.Y);

                position.X += sprites[i].Width + (PaddingFunc == null ? ScaledPadding : PaddingFunc(sprites[i].Width));
            }
        }

        public TextObject Clone()
        {
            return (TextObject)MemberwiseClone();
        }
	}
}

