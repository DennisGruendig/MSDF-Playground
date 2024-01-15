using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MSDF_Font_Library.Content;
using MSDF_Font_Library.Datatypes;
using MSDF_Font_Library.Rendering;
using MSDF_Playground_Game_Library.Graphics_Test_1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MSDF_Playground_Game_Library
{
    public class Game2 : Game
    {
        private GraphicsDeviceManager _graphics;
        private LineBatcher _LineBatcher;
        private bool firstRender = true;

        private KeyboardState kstate;
        private KeyboardState pkstate;
        private MouseState mstate;
        private MouseState pmstate;
        private TimeSpan next;

        private float Size = 1;

        private DateTime start = DateTime.Now;
        private TimeSpan[] spanlist = new TimeSpan[500];
        private int spanindex = 0;
        private bool showSpan = false;

        public Game2()
        {
            _graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 800,
                PreferredBackBufferHeight = 450,
                SynchronizeWithVerticalRetrace = false,
                GraphicsProfile = GraphicsProfile.HiDef,
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
            _LineBatcher = new LineBatcher(GraphicsDevice);
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
            }
            else
            {
                Size += 0.001f / 120f * (mstate.ScrollWheelValue - pmstate.ScrollWheelValue);
                if (Size < 0) Size = 0;
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

            GraphicsDevice.Clear(Color.Black);
            var vp = GraphicsDevice.Viewport;

            string frameMessage = string.Empty;
            if (showSpan)
                frameMessage = $"µs: {(spanlist.Select(x => x.TotalMicroseconds).Sum() / spanlist.Length).ToString("000.0")}";
            else
                frameMessage = $"µs: {spanindex.ToString("000")} / {spanlist.Length.ToString("000")}";

            _LineBatcher.Begin();
            _LineBatcher.Draw(new Vector3(10, 10, 0), new Vector3(vp.Width - 10, 10, 0), new Vector3(vp.Width - 10, vp.Height - 10, 0));
            _LineBatcher.Draw(new Vector3(10, 100, 0), new Vector3(50, 120, 0), new Vector3(30, 140, 0));
            _LineBatcher.End();

            base.Draw(gameTime);
        }

    }
}