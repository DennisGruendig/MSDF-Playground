using FontExtension;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoMSDF2.Text;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MonoMSDF2
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
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

        protected override void LoadContent()
        {
            var effect = Content.Load<Effect>("FieldFontEffect");
            var font = Content.Load<FieldFont>("KiwiSoda");
            var font2 = Content.Load<FieldFont>("Germany");

            textRenderer = new TextRenderer2(effect, font, GraphicsDevice);
            textRenderer.OptimizeForTinyText = true;

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

        DateTime start;
        DateTime end;
        TimeSpan[] rendertimes = new TimeSpan[600];
        int index;
        int micro;

        protected override void Draw(GameTime gameTime)
        {
            micro = rendertimes.Sum(x => x.Microseconds) / rendertimes.Length;
            start = DateTime.Now;
            
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

            currentRenderer.Render($"Frametime: {micro.ToString("0000.0000")} Possible Framerate: {(micro > 0 ? 1000000 / micro : 0).ToString("000000.00")}", new Vector2(10, _graphics.PreferredBackBufferHeight - 40), new Vector2(1.5f));
            currentRenderer.Render($"Sin: {sin.ToString("+0.00;-0.00")}", new Vector2(10, 10), new Vector2(1, 1));
            currentRenderer.Render($"Cos: {cos.ToString("+0.00;-0.00")}", new Vector2(80, 10), new Vector2(1, 1));
            currentRenderer.Render($"Pos X: {pos.X.ToString("0000.000")}", new Vector2(170, 10), new Vector2(1, 1));
            currentRenderer.Render($"Pos Y: {pos.Y.ToString("0000.000")}", new Vector2(290, 10), new Vector2(1, 1));
            currentRenderer.Render($"Pos: {pos.X.ToString("0000.000")} / {pos.Y.ToString("0000.000")}", pos, new Vector2(3, 3));

            float scale = (sin + 1f) * 3;
            currentRenderer.Render($"Scaling Text: {(scale * 100f).ToString("0.0")}%", new Vector2(_graphics.PreferredBackBufferWidth * 0.5f, 10), new Vector2(scale, scale));

            end = DateTime.Now;

            rendertimes[index++] = end - start;
            if (index >= rendertimes.Length) index = 0;
        }
    }
}
