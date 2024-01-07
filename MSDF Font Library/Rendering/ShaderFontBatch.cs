using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using MSDF_Font_Library.FontAtlas;
using MSDF_Font_Library.Content;
using MSDF_Font_Library.Datatypes;

namespace MSDF_Font_Library.Rendering
{
    public class ShaderFontBatch
    {
        private GraphicsDevice _GraphicsDevice;
        private SpriteBatch _SpriteBatch;
        private Effect _Shader;
        private bool _BeginCalled;

        public Color ForegroundColor { get; set; }
        public Color BackgroundColor { get; set; }
        public ShaderFont Font { get; set; }

        public ShaderFontBatch(GraphicsDevice graphicsDevice, ShaderFont font, Effect shader, Color? foregroundColor = null, Color? backgroundColor = null)
        {
            _GraphicsDevice = graphicsDevice;
            _SpriteBatch = new SpriteBatch(graphicsDevice);
            _Shader = shader;
            _BeginCalled = false;
            Font = font;
            ForegroundColor = foregroundColor ?? Color.Black;
            BackgroundColor = backgroundColor ?? Color.Transparent;
        }

        public void Begin()
        {
            if (_BeginCalled)
                throw new Exception("Begin cannot be called again until End has been successfully called.");
            _BeginCalled = true;

            _SpriteBatch.Begin(effect: _Shader);
            _Shader.Parameters["pxRange"].SetValue(Font.DistanceRange);
            _Shader.Parameters["textureSize"].SetValue(new Vector2(Font.AtlasWidth, Font.AtlasWidth));
            _Shader.Parameters["fgColor"].SetValue(ForegroundColor.ToVector4());
            _Shader.Parameters["bgColor"].SetValue(BackgroundColor.ToVector4());
        }

        public void End()
        {
            if (!_BeginCalled)
                throw new InvalidOperationException("Begin must be called before calling End.");
            _BeginCalled = false;
            _SpriteBatch.End();
        }

        public void DrawString(string text, Vector2 position, Vector2? scale = null, HorizontalAlignment hAlign = HorizontalAlignment.Left, VerticalAlignment vAlign = VerticalAlignment.Top)
        {
            if (!_BeginCalled)
                throw new InvalidOperationException("Begin must be called before calling DrawString.");

            Vector2 size = MeasureString(text);
            float advance = 0;

            switch (hAlign)
            {
                case HorizontalAlignment.Left: break;
                case HorizontalAlignment.Center: position.X -= size.X * 0.5f; break;
                case HorizontalAlignment.Right: position.X -= size.X; break;
            }
            switch (vAlign)
            {
                case VerticalAlignment.Top: break;
                case VerticalAlignment.Middle: position.Y -= Font.LineOffset * 0.5f; break;
                case VerticalAlignment.Bottom: position.Y -= Font.LineOffset; break;
            }

            for (int i = 0; i < text.Length; i++)
            {
                var glyph = Font.GetGlyph(text[i]);
                _SpriteBatch.Draw(
                    Font.AtlasTexture,
                    new Vector2(
                        (int)(position.X + glyph.CursorBounds.X + advance),
                        (int)(position.Y + glyph.CursorBounds.Y)),
                    glyph.AtlasSource,
                    Color.White);
                advance += glyph.Advance;
            }
        }

        public Vector2 MeasureString(string text)
        {
            Vector2 size = new Vector2(0, Font.LineOffset);
            float advance = 0;

            for (int i = 0; i < text.Length; i++)
            {
                var glyph = Font.GetGlyph(text[i]);
                if (i == text.Length - 1)
                    advance += glyph.CursorBounds.Width;
                else
                    advance += (float)glyph.Advance;

                size.X = (float)advance;
            }
            return size;
        }

    }
}
