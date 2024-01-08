using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MSDF_Font_Library.Content;
using MSDF_Font_Library.Datatypes;
using MSDF_Font_Library.FontAtlas;
using MSDF_Font_Library.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace MSDF_Playground
{
    public class Game1 : Game
    {
        private readonly List<MissingCharacterInfo> _MissingCharacters = MissingCharacters.CharacterList;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private ShaderFontBatch _ShaderFontBatch;
        private Effect MSDFshader;
        private ShaderFont Font;
        private bool firstRender = true;

        private KeyboardState kstate;
        private KeyboardState pkstate;
        private MouseState mstate;
        private MouseState pmstate;

        private TimeSpan next;
        private int fontIndex;

        private float Size = 0;
        private Vector2 Position = new Vector2(0, 0);
        private float scrollOffset = 0;
        private Glyph firstLetter;

        private const string DEF_CHARSET = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum. ";
        private int index;
        private char[] chars = new char[40];

        private DateTime start = DateTime.Now;
        private TimeSpan[] spanlist = new TimeSpan[500];
        private int spanindex = 0;
        private bool showSpan = false;

        private VerticalAlignment valign = VerticalAlignment.Middle;
        private HorizontalAlignment halign = HorizontalAlignment.Center;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1800,
                PreferredBackBufferHeight = 800,
                SynchronizeWithVerticalRetrace = false,
                GraphicsProfile = GraphicsProfile.HiDef
            };
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            MSDFshader = Content.Load<Effect>("MSDFShader");

            Font = Content.Load<ShaderFont>("TrueTypeFonts/BitPotionExt");
            Font.Initialize(GraphicsDevice);

            chars = (DEF_CHARSET + DEF_CHARSET).Substring(index, chars.Length).ToArray();
            firstLetter = Font.GetGlyph(chars[0]);

            _ShaderFontBatch = new ShaderFontBatch(GraphicsDevice, Font, MSDFshader);
        }

        protected override void Update(GameTime gameTime)
        {
            pkstate = kstate;
            kstate = Keyboard.GetState();
            pmstate = mstate;
            mstate = Mouse.GetState();

            if (gameTime.TotalGameTime >= next)
            {
                next = gameTime.TotalGameTime.Add(TimeSpan.FromMilliseconds(500));

            }

            Size +=  0.1f * (mstate.ScrollWheelValue - pmstate.ScrollWheelValue);
            
            if (mstate.LeftButton == ButtonState.Pressed)
            {
                Position += (mstate.Position - pmstate.Position).ToVector2();
            }

            if (kstate.IsKeyDown(Keys.Left)) halign = HorizontalAlignment.Right;
            else if (kstate.IsKeyDown(Keys.Right)) halign = HorizontalAlignment.Left;
            else halign = HorizontalAlignment.Center;

            if (kstate.IsKeyDown(Keys.Up)) valign = VerticalAlignment.Bottom;
            else if (kstate.IsKeyDown(Keys.Down)) valign = VerticalAlignment.Top;
            else valign = VerticalAlignment.Middle;
            
            if (kstate.IsKeyDown(Keys.Space) && !pkstate.IsKeyDown(Keys.Space))
            {
                switch (fontIndex)
                {
                    case 0:
                        Font = Content.Load<ShaderFont>("TrueTypeFonts/NDSBIOS");
                        fontIndex++; break;
                    case 1:
                        Font = Content.Load<ShaderFont>("TrueTypeFonts/KiwiSoda");
                        fontIndex++; break;
                    case 2:
                        Font = Content.Load<ShaderFont>("TrueTypeFonts/Connecticut");
                        fontIndex++; break;
                    case 3:
                        Font = Content.Load<ShaderFont>("TrueTypeFonts/Germany");
                        fontIndex++; break;
                    case 4:
                        Font = Content.Load<ShaderFont>("TrueTypeFonts/Bermuda");
                        fontIndex++; break;
                    case 5:
                        Font = Content.Load<ShaderFont>("TrueTypeFonts/Sylfaen");
                        fontIndex++; break;
                    case 6:
                        Font = Content.Load<ShaderFont>("TrueTypeFonts/SanDiego");
                        fontIndex++; break;
                    case 7:
                        Font = Content.Load<ShaderFont>("TrueTypeFonts/BitPotionExt");
                        fontIndex = 0; break;
                }
                Font.Initialize(GraphicsDevice);
                _ShaderFontBatch.Font = Font;
                firstLetter = Font.GetGlyph(chars[0]);
            }

            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime)
        {
            if (_graphics.PreferredBackBufferWidth != Window.ClientBounds.Width
                || _graphics.PreferredBackBufferHeight != Window.ClientBounds.Height
                || firstRender)
            {
                _graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
                _graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
                firstRender = false;
            }
            
            GraphicsDevice.Clear(Color.LightGray);

            scrollOffset -= (float)gameTime.ElapsedGameTime.TotalSeconds * 150;

            if (MathF.Abs(scrollOffset) > firstLetter.Advance)
            {
                scrollOffset += (float)firstLetter.Advance;
                index++;
                if (index >= DEF_CHARSET.Length)
                    index = 0;

                chars = (DEF_CHARSET + DEF_CHARSET).Substring(index, chars.Length).ToArray();
                firstLetter = Font.GetGlyph(chars[0]);
            }

            string frameMessage = string.Empty;
            if (showSpan)
                frameMessage = $"µs: {(spanlist.Select(x => x.TotalMicroseconds).Sum() / spanlist.Length).ToString("000.0")}";
            else
                frameMessage = $"µs: {spanindex.ToString("000")} / {spanlist.Length.ToString("000")}";

            float lineOffset = Font.LineOffset;

            _spriteBatch.Begin();

            var vp = GraphicsDevice.Viewport;
            var mid = new Vector2(vp.Width * 0.5f, vp.Height * 0.5f);
            var pixelbl = new Texture2D(GraphicsDevice, 1, 1);
            pixelbl.SetData(new Color[] { Color.Black });
            Color lineIntensity = new Color(1, 1, 1, 0.3f);

            // Horizontal
            _spriteBatch.Draw(pixelbl, new Rectangle(0, (int)mid.Y - 1, vp.Width, 2), lineIntensity);

            // Vertical
            _spriteBatch.Draw(pixelbl, new Rectangle((int)mid.X - 1, 0, 2, vp.Height), lineIntensity);
            
            _spriteBatch.End();

            start = DateTime.Now;

            _ShaderFontBatch.Begin();
            _ShaderFontBatch.DrawString(frameMessage, new Vector2(_graphics.PreferredBackBufferWidth * 0.5f, _graphics.PreferredBackBufferHeight * 0.5f), vAlign: valign, hAlign: halign);
            _ShaderFontBatch.DrawString(new string(chars), new Vector2(0 + scrollOffset, _graphics.PreferredBackBufferHeight - lineOffset));
            _ShaderFontBatch.End();

            spanlist[spanindex++] = DateTime.Now - start;
            if (spanindex >= spanlist.Length)
            {
                showSpan = true;
                spanindex = 0;
            }

            base.Draw(gameTime);
        }

    }
}