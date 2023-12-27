using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using FontExtension;

namespace MSDF_Playground
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Texture2D[] MSDFTextures;
        private Effect MSDFshader;
        private SpriteFont DefaultFont;
        private SpriteFont HackFont;

        private Vector2 screenBorder;
        private Vector2 screenCenter;
        private Vector2 mousePosition;
        private Vector2 MSDFTextureSize;
        private int _currentTexture;
        private bool isPressed;
        private bool firstRender;

        int currentTexture
        {
            get { return _currentTexture; }
            set
            {
                _currentTexture = value;
                if (_currentTexture < 0) _currentTexture = MSDFTextures.Length - 1;
                if (_currentTexture > MSDFTextures.Length - 1) _currentTexture = 0;
                MSDFTextureSize = new Vector2(MSDFTextures[_currentTexture].Width, MSDFTextures[_currentTexture].Height);
            }
        }

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
        }

        protected override void Initialize()
        {
            firstRender = true;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            DefaultFont = Content.Load<SpriteFont>("DefaultFont");
            MSDFshader = Content.Load<Effect>("MSDFShader");

            // IMPORTANT : Your MSDF textures must have 'ColorKeyEnabled = false' in Content Pipeline Tool.
            MSDFTextures = new Texture2D[]
            {
                Content.Load<Texture2D>("GermanyChars/A"),
                Content.Load<Texture2D>("GermanyChars/B"),
                Content.Load<Texture2D>("Test"),
                Content.Load<Texture2D>("Germany")
            };

            var atlas = Content.Load<Texture2D>("Germany");

            List<Rectangle> bounds = new List<Rectangle>()
            {
                new Rectangle(838, 0, 145, 149),
                new Rectangle(838, 0, 145, 149),
                new Rectangle(838, 0, 145, 149),
                new Rectangle(838, 0, 145, 149),
                new Rectangle(838, 0, 145, 149),
                new Rectangle(838, 0, 145, 149)
            };

            List<Rectangle> cropping = new List<Rectangle>()
            {
                new Rectangle(0, 0, 0, 0),
                new Rectangle(0, 0, 0, 0),
                new Rectangle(0, 0, 0, 0),
                new Rectangle(0, 0, 0, 0),
                new Rectangle(0, 0, 0, 0),
                new Rectangle(0, 0, 0, 0)
            };

            List<char> chars = new List<char>()
            {
                'A',
                'B',
                'C',
                'D',
                'E',
                'F'
            };

            List<Vector3> kerning = new List<Vector3>()
            {
                new Vector3(0, 0, 0),
                new Vector3(0, 0, 0),
                new Vector3(0, 0, 0),
                new Vector3(0, 0, 0),
                new Vector3(0, 0, 0),
                new Vector3(0, 0, 0)
            };

            HackFont = new SpriteFont(Content.Load<Texture2D>("Germany"), bounds, cropping, chars, 5, 2, kerning, 'A');
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState kstate = Keyboard.GetState();
            MouseState mstate = Mouse.GetState();

            if (isPressed && kstate.GetPressedKeys().Length == 0)
                isPressed = false;

            if (!isPressed)
            {
                if (kstate.IsKeyDown(Keys.A))
                    currentTexture--;
                else if (kstate.IsKeyDown(Keys.D))
                    currentTexture++;

                if (kstate.GetPressedKeys().Length != 0)
                    isPressed = true;
            }

            mousePosition = mstate.Position.ToVector2();

            factor = 0.25f + MathF.Sin(MathF.PI * (float)(gameTime.TotalGameTime.TotalSeconds * 0.2f % 1));

            base.Update(gameTime);
        }

        float factor;

        protected override void Draw(GameTime gameTime)
        {
            if (_graphics.PreferredBackBufferWidth != Window.ClientBounds.Width
                || _graphics.PreferredBackBufferHeight != Window.ClientBounds.Height
                || firstRender)
            {
                _graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
                _graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
                screenBorder = new Vector2(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
                screenCenter = screenBorder * 0.5f;
                firstRender = false;
            }
            
            GraphicsDevice.Clear(Color.LightGray);

            _spriteBatch.Begin();

            //_spriteBatch.Draw(MSDFTextures[currentTexture], MSDFTextures[currentTexture].Bounds, Color.White);
            //_spriteBatch.DrawString(HackFont, "ABCDEFG", new Vector2(20, 20), Color.Black);
            //_spriteBatch.DrawString(HackFont, "A", new Vector2(20, 220), Color.Black);

            _spriteBatch.End();

            _spriteBatch.Begin(effect: MSDFshader);
            _spriteBatch.DrawString(HackFont, "ABCDEFG", new Vector2(20, 20), Color.Black);
            _spriteBatch.DrawString(HackFont, "A", new Vector2(20, 220), Color.Black);

            //Vector2 scale = (mousePosition - screenCenter) / screenBorder * 5;

            //MSDFshader.Parameters["pxRange"].SetValue(scale.Y);
            MSDFshader.Parameters["pxRange"].SetValue((int)screenBorder.Y * factor);
            MSDFshader.Parameters["textureSize"].SetValue(MSDFTextureSize); // MSDF shader needs texture size.

            // MSDF shader lerp with these colors.
            MSDFshader.Parameters["bgColor"].SetValue(new Vector4(0, 0, 0, 0));
            MSDFshader.Parameters["fgColor"].SetValue(new Vector4(0, 0.5f, 1, 1));

            //scale = MSDFTextureSize * scale.X;
            //_spriteBatch.Draw(MSDFTextures[currentTexture], new Rectangle((int)(screenCenter.X - scale.X / 2), (int)(screenCenter.Y - scale.Y / 2), (int)scale.X, (int)scale.Y), Color.White);

            Vector2 size = new Vector2(screenBorder.X * factor, screenBorder.Y * factor);
            Vector2 position = mousePosition - (size * 0.5f);

            _spriteBatch.Draw(MSDFTextures[currentTexture], new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y), Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}