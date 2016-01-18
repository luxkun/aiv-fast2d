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

        private Sprite[] Sprites;
        private Texture Font;
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
            if (Font == null)
            {
                Font = new Texture(FontFile);
                if (Alpha >= 0 && Alpha < 255)
                { //Color != FontBaseColor || 
                    for (int y = 0; y < Font.Height; y++)
                    {
                        for (int x = 0; x < Font.Width; x++)
                        {
                            int position = (y * Font.Width * 4) + (x * 4);
                            //if (Color != FontBaseColor && 
                            //    Font.Bitmap[position] == FontBaseColor.R && Font.Bitmap[position + 1] == FontBaseColor.G && 
                            //    Font.Bitmap[position + 2] == FontBaseColor.B && Font.Bitmap[position + 3] == FontBaseColor.A)
                            //{
                            //    Font.Bitmap[position] = Color.R;
                            //    Font.Bitmap[position + 1] = Color.G;
                            //    Font.Bitmap[position + 2] = Color.B;
                            //    Font.Bitmap[position + 3] = Color.A;
                            //}
                            if (Alpha >= 0 && Alpha < 255 && Font.Bitmap[position + 3] > 20)
                                Font.Bitmap[position + 3] = (byte)Alpha;
                        }
                    }
                    Font.Update();
                }
            }
            if (lastDraw.Item1 != Text || lastDraw.Item2 != Scale)
            {
                ScaledPadding = Padding*Scale.X;
                Sprites = new Sprite[Text.Length];
                for (int i = 0; i < Text.Length; i++)
                {
                    var c = Text[i];
                    if (c == ' ')
                    {
                        Sprites[i] = null;
                        continue;
                    }
                    Sprites[i] = new Sprite(
                        (int)(CharToSprite[c].Item2.X * Scale.X),
                        (int)(CharToSprite[c].Item2.Y * Scale.Y)
                    );
                    Sprites[i].scale = Scale;
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
                    width += Sprites[i].Width + (PaddingFunc == null ? ScaledPadding : PaddingFunc(SpaceWidth * Scale.X));
                        //((i + 1) < Text.Length && SpawnAnimation != 1 ? ScaledPadding : 1);
                    h = Sprites[i].Height;
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
                Sprites[i].position = new Vector2(position.X, position.Y);
                Sprites[i].DrawTexture(
                    Font, (int)CharToSprite[c].Item1.X, (int)CharToSprite[c].Item1.Y,
                    (int)CharToSprite[c].Item2.X, (int)CharToSprite[c].Item2.Y);

                position.X += Sprites[i].Width + (PaddingFunc == null ? ScaledPadding : PaddingFunc(Sprites[i].Width));
                ;
            }
        }

        public TextObject Clone()
        {
            return (TextObject)MemberwiseClone();
        }
	}
}

