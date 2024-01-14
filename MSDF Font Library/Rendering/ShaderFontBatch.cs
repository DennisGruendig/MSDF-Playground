using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using MSDF_Font_Library.Content;
using MSDF_Font_Library.Datatypes;

namespace MSDF_Font_Library.Rendering
{
    public class ShaderFontBatch
    {
        private SpriteBatch _spriteBatch;
        private Effect _shader;
        private bool _beginCalled;
        private TextString _textString;

        public Color ForegroundColor { get; set; }
        public ShaderFont Font { get; set; }
        public bool PixelPosition;

        public ShaderFontBatch(GraphicsDevice graphicsDevice, ShaderFont font, Effect shader, Color? foregroundColor = null, Color? backgroundColor = null)
        {
            _spriteBatch = new SpriteBatch(graphicsDevice);
            _shader = shader;
            _beginCalled = false;
            _textString = new TextString(font);
            Font = font;
            ForegroundColor = foregroundColor ?? Color.Black;
            PixelPosition = true;
        }

        public void Begin(float scale = 1)
        {
            if (_beginCalled)
                throw new Exception("Begin cannot be called again until End has been successfully called.");
            _beginCalled = true;

            _spriteBatch.Begin(effect: _shader);
            //_shader.Parameters["pxOffset"].SetValue(scale);
            _shader.Parameters["pxRange"].SetValue(Font.DistanceRange);
            //_shader.Parameters["pxRange"].SetValue(scale);
            _shader.Parameters["textureSize"].SetValue(Font.AtlasSize);
            _shader.Parameters["fgColor"].SetValue(ForegroundColor.ToVector4());
        }

        public void End()
        {
            if (!_beginCalled)
                throw new InvalidOperationException("Begin must be called before calling End.");
            _beginCalled = false;
            _spriteBatch.End();
        }

        public void DrawString(string text, Vector2 position, Vector2? scale = null, HorizontalAlignment hAlign = HorizontalAlignment.Left, VerticalAlignment vAlign = VerticalAlignment.Top)
        {
            if (!_beginCalled)
                throw new InvalidOperationException("Begin must be called before calling DrawString.");

            _textString.Font = Font;
            _textString.Update(text, position, scale, hAlign, vAlign);

            for (int i = 0; i < _textString.DrawCallBuffer.Length; i++)
            {
                if (_textString.Text[i] != 32)
                    _spriteBatch.Draw(
                        Font.AtlasTexture,
                        _textString.DrawCallBuffer[i].Position,
                        _textString.DrawCallBuffer[i].SourceRectangle,
                        Color.White, 0f, Vector2.Zero,
                        _textString.DrawCallBuffer[i].Scale,
                        SpriteEffects.None, 0f);
            }
        }

    }
}
