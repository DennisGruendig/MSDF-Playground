using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MSDF_Font_Library;
using MSDF_Font_Library.FontAtlas;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MSDF_Playground
{
    public class Game1 : Game
    {
        private readonly List<MissingCharacterInfo> _MissingCharacters = MissingCharacters.CharacterList;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Texture2D[] MSDFTextures;
        private Effect MSDFshader;
        private SpriteFont DefaultFont;

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
            firstRender = true;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            MSDFshader = Content.Load<Effect>("MSDFShader");

            Font = Content.Load<ShaderFont>("TrueTypeFonts/Germany");
            Font.Initialize(GraphicsDevice);

            // IMPORTANT : Your MSDF textures must have 'ColorKeyEnabled = false' in Content Pipeline Tool.
            MSDFTextures =
            [
                Font.AtlasTexture
            ];
            currentTexture = 0;
            index = DEF_CHARSET.Length;



        }

        Effect DistanceFieldFontShader;
        ShaderFont Font;
        ShaderFont Font2;

        KeyboardState kstate;
        KeyboardState pkstate;
        MouseState mstate;
        MouseState pmstate;

        TimeSpan next;

        protected override void Update(GameTime gameTime)
        {
            pkstate = kstate;
            kstate = Keyboard.GetState();
            pmstate = mstate;
            mstate = Mouse.GetState();

            if (gameTime.TotalGameTime >= next)
            {
                next = gameTime.TotalGameTime.Add(TimeSpan.FromMilliseconds(100));

                bool charFound = false;
                while (!charFound)
                {
                    index++;
                    if (index >= DEF_CHARSET.Length)
                        index = 0;
                        
                    charFound = Font.SupportedCharacters.Contains(DEF_CHARSET[index]);
                }

                for (int i = 0; i < chars.Length - 1; i++)
                {
                    chars[i] = chars[i + 1];
                }
                chars[chars.Length - 1] = DEF_CHARSET[index];
            }

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

            Size +=  0.1f * (mstate.ScrollWheelValue - pmstate.ScrollWheelValue);
            
            if (mstate.LeftButton == ButtonState.Pressed)
            {
                Position += (mstate.Position - pmstate.Position).ToVector2();
            }

            if (kstate.IsKeyDown(Keys.Space) && !pkstate.IsKeyDown(Keys.Space))
            {
                switch (fontIndex)
                {
                    case 0:
                        Font = Content.Load<ShaderFont>("TrueTypeFonts/NDSBIOS");
                        fontIndex++;
                        break;
                    case 1:
                        Font = Content.Load<ShaderFont>("TrueTypeFonts/KiwiSoda");
                        fontIndex++;
                        break;
                    case 2:
                        Font = Content.Load<ShaderFont>("TrueTypeFonts/Connecticut");
                        fontIndex++;
                        break;
                    case 3:
                        Font = Content.Load<ShaderFont>("TrueTypeFonts/Suissnord");
                        fontIndex++;
                        break;
                    case 4:
                        Font = Content.Load<ShaderFont>("TrueTypeFonts/Bermuda");
                        fontIndex++;
                        break;
                    case 5:
                        Font = Content.Load<ShaderFont>("TrueTypeFonts/Sylfaen");
                        fontIndex++;
                        break;
                    case 6:
                        Font = Content.Load<ShaderFont>("TrueTypeFonts/SanDiego");
                        fontIndex++;
                        break;
                    case 7:
                        Font = Content.Load<ShaderFont>("TrueTypeFonts/Germany");
                        fontIndex = 0;
                        break;
                }
                Font.Initialize(GraphicsDevice);
            }

            if (kstate.IsKeyDown(Keys.NumPad0)) bgColor = .0f;
            if (kstate.IsKeyDown(Keys.NumPad1)) bgColor = .1f;
            if (kstate.IsKeyDown(Keys.NumPad2)) bgColor = .2f;
            if (kstate.IsKeyDown(Keys.NumPad3)) bgColor = .3f;
            if (kstate.IsKeyDown(Keys.NumPad4)) bgColor = .4f;
            if (kstate.IsKeyDown(Keys.NumPad5)) bgColor = .5f;
            if (kstate.IsKeyDown(Keys.NumPad6)) bgColor = .6f;
            if (kstate.IsKeyDown(Keys.NumPad7)) bgColor = .7f;
            if (kstate.IsKeyDown(Keys.NumPad8)) bgColor = .8f;
            if (kstate.IsKeyDown(Keys.NumPad9)) bgColor = .9f;

            base.Update(gameTime);
        }
        int fontIndex;

        float Size = 0;
        Vector2 Position = new Vector2(0, 0);
        float bgColor = .2f;

        //private const string DEF_CHARSET = "ABCDEFGHIJKLMNOPQRSTUVWXYZÄÖÜabcdefghijklmnopqrstuvwxyzäöü1234567890/*-+,.!?ß´`'°^_:;²³{[]}§$%&()©€@=<>|#~ \\\"\\\\";
        private const string DEF_CHARSET = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum. ";
        private int index;
        private char[] chars = new char[20];

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

            Vector2 size = MSDFTextureSize + new Vector2(Size);
            Vector2 position = Position + screenCenter - (size * 0.5f);

            _spriteBatch.Begin(effect: MSDFshader);
            //_spriteBatch.Draw(MSDFTextures[currentTexture], MSDFTextures[currentTexture].Bounds, Color.White);

            MSDFshader.Parameters["pxRange"].SetValue(size.Y);
            MSDFshader.Parameters["textureSize"].SetValue(MSDFTextureSize); // MSDF shader needs texture size.

            // MSDF shader lerp with these colors.
            MSDFshader.Parameters["bgColor"].SetValue(new Vector4(0, 0, 0, bgColor));
            MSDFshader.Parameters["fgColor"].SetValue(new Vector4(0, 0.5f, 1, 1));

            float distance = 20;

            for (int i = 0; i < chars.Length; i++)
            {
                var t1 = new Glyph(Font.AtlasRoot, Font.GetGlyph(chars[i]));
                _spriteBatch.Draw(
                Font.AtlasTexture,
                new Vector2(distance, 150 - t1.AtlasSize.Y) + t1.PlaneOrigin,
                t1.AtlasRectangle,
                Color.White);
                distance += t1.Advance;
            }

            float distance2 = 20;
            string zwei = "Pos: 12.500";
            for (int i = 0; i < zwei.Length; i++)
            {
                var t1 = new Glyph(Font.AtlasRoot, Font.GetGlyph(zwei[i]));
                _spriteBatch.Draw(
                Font.AtlasTexture,
                new Vector2(distance2, 400 - t1.AtlasSize.Y) + t1.PlaneOrigin,
                t1.AtlasRectangle,
                Color.White);
                distance2 += t1.Advance;
            }

            float distance3 = 20;
            for (int i = 0; i < DEF_CHARSET.Length && distance3 < _graphics.PreferredBackBufferWidth; i++)
            {
                var t1 = new Glyph(Font.AtlasRoot, Font.GetGlyph(DEF_CHARSET[i]));
                _spriteBatch.Draw(
                Font.AtlasTexture,
                new Vector2(distance3, 650 - t1.AtlasSize.Y) + t1.PlaneOrigin,
                t1.AtlasRectangle,
                Color.White);
                distance3 += t1.Advance;
            }

            _spriteBatch.End();

            _spriteBatch.Begin(effect: MSDFshader);
            
            MSDFshader.Parameters["pxRange"].SetValue(size.Y);
            MSDFshader.Parameters["textureSize"].SetValue(MSDFTextureSize); // MSDF shader needs texture size.

            // MSDF shader lerp with these colors.
            MSDFshader.Parameters["bgColor"].SetValue(new Vector4(0, 0, 0, bgColor));
            MSDFshader.Parameters["fgColor"].SetValue(new Vector4(0, 0.5f, 1, 1));

            //_spriteBatch.Draw(MSDFTextures[currentTexture], new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y), Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

    }
}