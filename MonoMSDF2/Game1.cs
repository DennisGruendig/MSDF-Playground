using FontExtension;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoMSDF2.Text;
using System;

namespace MonoMSDF2
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private TextRenderer2 textRenderer;
        private TextRenderer2 textRenderer2;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1080,
                PreferredBackBufferHeight = 768,
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

            var effect = Content.Load<Effect>("FieldFontEffect");
            //var font = Content.Load<FieldFont>("arial");
            var font = Content.Load<FieldFont>("KiwiSoda");
            var font2 = Content.Load<FieldFont>("arial");

            textRenderer = new TextRenderer2(effect, font, GraphicsDevice);
            textRenderer2 = new TextRenderer2(effect, font2, GraphicsDevice);
        }

        KeyboardState PreviousKeyboardState;
        KeyboardState CurrentKeyboardState;
        bool mode;

        protected override void Update(GameTime gameTime)
        {
            PreviousKeyboardState = CurrentKeyboardState;
            CurrentKeyboardState = Keyboard.GetState();

            if (CurrentKeyboardState.IsKeyDown(Keys.Space) && !PreviousKeyboardState.IsKeyDown(Keys.Space))
            {
                mode = !mode;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            GraphicsDevice.DepthStencilState = DepthStencilState.None;
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            //var world = Matrix.CreateOrthographicOffCenter(0, _graphics.PreferredBackBufferWidth, 0, _graphics.PreferredBackBufferHeight, 0, 1000);
            var world = Matrix.CreateOrthographic(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight, 0, 1000);

            float sin = MathF.Sin(MathHelper.TwoPi * (float)(gameTime.TotalGameTime.TotalSeconds * .1 % 1));
            float cos = MathF.Cos(MathHelper.TwoPi * (float)(gameTime.TotalGameTime.TotalSeconds * .1 % 1));
            var pos = new Vector2(_graphics.PreferredBackBufferWidth * .5f + sin * 100f, _graphics.PreferredBackBufferHeight * .5f + cos * 100f);

            TextRenderer2 currentRenderer = mode ? textRenderer2 : textRenderer;

            currentRenderer.Render($"Sin: {sin.ToString("+0.00;-0.00")}", new Vector2(10, 10), new Vector2(1, 1), GraphicsDevice);
            currentRenderer.Render($"Cos: {cos.ToString("+0.00;-0.00")}", new Vector2(80, 10), new Vector2(1, 1), GraphicsDevice);
            currentRenderer.Render($"Pos X: {pos.X.ToString("0000.000")}", new Vector2(170, 10), new Vector2(1, 1), GraphicsDevice);
            currentRenderer.Render($"Pos Y: {pos.Y.ToString("0000.000")}", new Vector2(290, 10), new Vector2(1, 1), GraphicsDevice);
            currentRenderer.Render($"Pos: {pos.X.ToString("0000.000")} / {pos.Y.ToString("0000.000")}", pos, new Vector2(3, 3), GraphicsDevice);

            currentRenderer.Render($"Scaling Text: {sin.ToString("+0.00;-0.00")}", new Vector2(_graphics.PreferredBackBufferWidth * 0.5f, 10), new Vector2(sin + 1, sin + 1), GraphicsDevice);
        }
    }
}
