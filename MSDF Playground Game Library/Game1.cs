using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MSDF_Font_Library.Content;
using MSDF_Font_Library.Datatypes;
using MSDF_Font_Library.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MSDF_Playground_Game_Library
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private ShaderFontBatch _ShaderFontBatch;
        private Effect MSDFshader;
        private bool firstRender = true;

        private KeyboardState kstate;
        private KeyboardState pkstate;
        private MouseState mstate;
        private MouseState pmstate;
        private TimeSpan next;

        private float Size = 1;
        private Vector2 Position = new Vector2(0, 0);
        private Vector2 uiScale = new Vector2(.5f, .5f);

        private DateTime start = DateTime.Now;
        private TimeSpan[] spanlist = new TimeSpan[500];
        private int spanindex = 0;
        private bool showSpan = false;

        private VerticalAlignment valign = VerticalAlignment.Base;
        private HorizontalAlignment halign = HorizontalAlignment.Center;

        private ShaderFont Font => _ShaderFontBatch.Font; 
        private Dictionary<string, ShaderFont> _fonts = new();
        private int _fontIndex = 0;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1600,
                PreferredBackBufferHeight = 900,
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

            foreach (string path in Directory.GetFiles(@"Content\Fonts\Detailed"))
            {
                string filename = Path.GetFileNameWithoutExtension(path);
                ShaderFont font = Content.Load<ShaderFont>($"Fonts/Detailed/{filename}");
                _fonts.Add(filename, font);
            }

            foreach (string path in Directory.GetFiles(@"Content\Fonts\Pixelated"))
            {
                string filename = Path.GetFileNameWithoutExtension(path);
                ShaderFont font = Content.Load<ShaderFont>($"Fonts/Pixelated/{filename}");
                _fonts.Add(filename, font);
            }

            foreach (var font in _fonts)
            {
                font.Value.Initialize(GraphicsDevice);
            }

            _ShaderFontBatch = new ShaderFontBatch(GraphicsDevice, _fonts.First().Value, MSDFshader);
        }

        protected override void Update(GameTime gameTime)
        {
            pkstate = kstate;
            kstate = Keyboard.GetState();
            pmstate = mstate;
            mstate = Mouse.GetState();

            if (mstate.RightButton == ButtonState.Pressed)
            {
                Size = 1f;
                Position = Vector2.Zero;
            }
            else
            {
                if (mstate.LeftButton == ButtonState.Pressed)
                    Position += (mstate.Position - pmstate.Position).ToVector2();

                Size += 0.01f / 120f * (mstate.ScrollWheelValue - pmstate.ScrollWheelValue);
                if (Size < 0) Size = 0;
            }

            if (kstate.IsKeyDown(Keys.Left) && !pkstate.IsKeyDown(Keys.Left))
                if (halign == HorizontalAlignment.Left)
                    halign = HorizontalAlignment.Center;
                else if (halign == HorizontalAlignment.Center)
                    halign = HorizontalAlignment.Right;

            if (kstate.IsKeyDown(Keys.Right) && !pkstate.IsKeyDown(Keys.Right))
                if (halign == HorizontalAlignment.Right)
                    halign = HorizontalAlignment.Center;
                else if (halign == HorizontalAlignment.Center)
                    halign = HorizontalAlignment.Left;

            if (kstate.IsKeyDown(Keys.Up) && !pkstate.IsKeyDown(Keys.Up))
                if (valign == VerticalAlignment.Top)
                    valign = VerticalAlignment.Middle;
                else if (valign == VerticalAlignment.Middle)
                    valign = VerticalAlignment.Base;
                else if (valign == VerticalAlignment.Base)
                    valign = VerticalAlignment.Bottom;

            if (kstate.IsKeyDown(Keys.Down) && !pkstate.IsKeyDown(Keys.Down))
                if (valign == VerticalAlignment.Bottom)
                    valign = VerticalAlignment.Base;
                else if (valign == VerticalAlignment.Base)
                    valign = VerticalAlignment.Middle;
                else if (valign == VerticalAlignment.Middle)
                    valign = VerticalAlignment.Top;

            if ((kstate.IsKeyDown(Keys.Space) && !pkstate.IsKeyDown(Keys.Space)) || gameTime.TotalGameTime >= next)
            //if ((kstate.IsKeyDown(Keys.Space) && !pkstate.IsKeyDown(Keys.Space)))
            {
                _fontIndex = _fontIndex < _fonts.Count - 1 ? _fontIndex + 1 : 0;
                _ShaderFontBatch.Font = _fonts.ElementAt(_fontIndex).Value;
            }

            if (gameTime.TotalGameTime >= next)
            {
                next = gameTime.TotalGameTime.Add(TimeSpan.FromMilliseconds(5000));
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

            string frameMessage = string.Empty;
            if (showSpan)
                frameMessage = $"µs: {(spanlist.Select(x => x.TotalMicroseconds).Sum() / spanlist.Length).ToString("000.0")}";
            else
                frameMessage = $"µs: {spanindex.ToString("000")} / {spanlist.Length.ToString("000")}";

            double lineOffset = Font.ActualLineHeight;

            _spriteBatch.Begin();

            var vp = GraphicsDevice.Viewport;
            var mid = new Vector2(vp.Width * 0.5f, vp.Height * 0.5f);
            var pixelbl = new Texture2D(GraphicsDevice, 1, 1);
            var pixelrd = new Texture2D(GraphicsDevice, 1, 1);
            var pixelgr = new Texture2D(GraphicsDevice, 1, 1);
            var pixelbu = new Texture2D(GraphicsDevice, 1, 1);
            var pixelye = new Texture2D(GraphicsDevice, 1, 1);
            var pixelma = new Texture2D(GraphicsDevice, 1, 1);
            var pixelcy = new Texture2D(GraphicsDevice, 1, 1);
            pixelbl.SetData(new Color[] { Color.Black });
            pixelrd.SetData(new Color[] { Color.Red });
            pixelgr.SetData(new Color[] { Color.Lime });
            pixelbu.SetData(new Color[] { Color.Blue });
            pixelye.SetData(new Color[] { Color.Yellow });
            pixelma.SetData(new Color[] { Color.Magenta });
            pixelcy.SetData(new Color[] { Color.Cyan });
            Color lineIntensity = new Color(1, 1, 1, 0.3f);
            Color lineIntensity2 = new Color(1, 1, 1, 0.2f);

            // Position indicator
            _spriteBatch.Draw(pixelbl, new Rectangle(0, (int)mid.Y - 1 + (int)Position.Y, vp.Width, 2), lineIntensity);
            _spriteBatch.Draw(pixelbl, new Rectangle((int)mid.X - 1 + (int)Position.X, 0, 2, vp.Height), lineIntensity);

            // Font positions
            _spriteBatch.Draw(pixelrd, new Rectangle(0, (int)(4 * uiScale.Y), vp.Width, 2), lineIntensity2);
            _spriteBatch.Draw(pixelgr, new Rectangle(0, (int)((4 * uiScale.Y) + (Font.Ascender * Font.FontSize) * uiScale.Y), vp.Width, 2), lineIntensity2);
            _spriteBatch.Draw(pixelbu, new Rectangle(0, (int)((4 * uiScale.Y) + (Font.Ascender * Font.FontSize + Math.Abs(Font.Descender) * Font.FontSize) * uiScale.Y), vp.Width, 2), lineIntensity2);
            _spriteBatch.Draw(pixelye, new Rectangle(0, (int)((4 * uiScale.Y) + (Font.LineHeight * Font.FontSize) * uiScale.Y), vp.Width, 2), lineIntensity2);
            _spriteBatch.Draw(pixelma, new Rectangle(0, (int)((4 * uiScale.Y) + (Font.Height * Font.FontSize * 0.5f) * uiScale.Y), vp.Width, 2), lineIntensity2);
            _spriteBatch.Draw(pixelcy, new Rectangle(0, (int)((4 * uiScale.Y) + (Font.Height * Font.FontSize) * uiScale.Y), vp.Width, 2), lineIntensity2);

            _spriteBatch.End();

            start = DateTime.Now;

            _ShaderFontBatch.Begin(Size);
            _ShaderFontBatch.DrawString($"Font {_fontIndex + 1} Name: {Font.Name} - {frameMessage}", new Vector2(5, (int)(5 * uiScale.Y)), uiScale, HorizontalAlignment.Left, VerticalAlignment.Top);
            _ShaderFontBatch.DrawString($"Scale: {Size.ToString("0.000")}", new Vector2(5, (int)(5 + Font.ActualLineHeight * uiScale.Y)), uiScale, HorizontalAlignment.Left, VerticalAlignment.Top);
            _ShaderFontBatch.DrawString($"{halign} / {valign}", new Vector2(_graphics.PreferredBackBufferWidth * 0.5f + (int)Position.X, _graphics.PreferredBackBufferHeight * 0.5f + (int)Position.Y), new Vector2(Size), halign, valign);
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