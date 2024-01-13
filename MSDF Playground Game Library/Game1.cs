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
using System.Reflection;
using System.Runtime.InteropServices;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MSDF_Playground_Game_Library
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private ShaderFontBatch _ShaderFontBatch;
        private Effect MSDFshader;
        private ShaderFont Font;
        private string FontName = "BitPotionExt";
        private bool firstRender = true;

        private KeyboardState kstate;
        private KeyboardState pkstate;
        private MouseState mstate;
        private MouseState pmstate;

        private TimeSpan next;
        private int fontIndex = -1;

        private float Size = 1;
        private Vector2 Position = new Vector2(0, 0);
        private Vector2 uiScale = new Vector2(.5f, .5f);

        private DateTime start = DateTime.Now;
        private TimeSpan[] spanlist = new TimeSpan[500];
        private int spanindex = 0;
        private bool showSpan = false;

        private VerticalAlignment valign = VerticalAlignment.Base;
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

            Font = Content.Load<ShaderFont>($"TrueTypeFonts/{FontName}");
            Font.Initialize(GraphicsDevice);

            _ShaderFontBatch = new ShaderFontBatch(GraphicsDevice, Font, MSDFshader);
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

            if ((kstate.IsKeyDown(Keys.Space) && !pkstate.IsKeyDown(Keys.Space)))// || gameTime.TotalGameTime >= next)
            {
                fontIndex = fontIndex < 10 ? fontIndex + 1 : 0;
                switch (fontIndex)
                {
                    case 0: FontName = "Arial"; break;
                    case 1: FontName = "Paperkind"; break;
                    case 2: FontName = "KiwiSoda"; break;
                    case 3: FontName = "Connecticut"; break;
                    case 4: FontName = "Germany"; break;
                    case 5: FontName = "Bermuda"; break;
                    case 6: FontName = "Sylfaen"; break;
                    case 7: FontName = "SanDiego"; break;
                    case 8: FontName = "Inkfree"; break;
                    case 9: FontName = "NDSBIOS"; break;
                    case 10: FontName = "BitPotionExt"; break;
                }

                Font = Content.Load<ShaderFont>($"TrueTypeFonts/{FontName}");
                Font.Initialize(GraphicsDevice);
                _ShaderFontBatch.Font = Font;
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

            float lineOffset = Font.ActualLineHeight;

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
            _spriteBatch.Draw(pixelbu, new Rectangle(0, (int)((4 * uiScale.Y) + (Font.Ascender * Font.FontSize + MathF.Abs(Font.Descender) * Font.FontSize) * uiScale.Y), vp.Width, 2), lineIntensity2);
            _spriteBatch.Draw(pixelye, new Rectangle(0, (int)((4 * uiScale.Y) + (Font.LineHeight * Font.FontSize) * uiScale.Y), vp.Width, 2), lineIntensity2);
            _spriteBatch.Draw(pixelma, new Rectangle(0, (int)((4 * uiScale.Y) + (Font.Height * Font.FontSize * 0.5f) * uiScale.Y), vp.Width, 2), lineIntensity2);
            _spriteBatch.Draw(pixelcy, new Rectangle(0, (int)((4 * uiScale.Y) + (Font.Height * Font.FontSize) * uiScale.Y), vp.Width, 2), lineIntensity2);

            _spriteBatch.End();

            start = DateTime.Now;

            _ShaderFontBatch.Begin(Size);
            _ShaderFontBatch.DrawString($"Font Name: {FontName} - {frameMessage}", new Vector2(5, (int)(5 * uiScale.Y)), uiScale, HorizontalAlignment.Left, VerticalAlignment.Top);
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