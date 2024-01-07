using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using MSDF_Font_Library.FontAtlas;

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

            // DebuggerLines();

            _SpriteBatch.Begin(effect: _Shader);
            _Shader.Parameters["pxRange"].SetValue(Font.AtlasRoot.Atlas.DistanceRange);
            _Shader.Parameters["textureSize"].SetValue(new Vector2(Font.AtlasTexture.Width, Font.AtlasTexture.Width));
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

        public void DrawString(string text, Vector2 position, Vector2? scale = null)
        {
            if (!_BeginCalled)
                throw new InvalidOperationException("Begin must be called before calling DrawString.");

            for (int i = 0; i < text.Length; i++)
            {
                var glyph = new Glyph(Font.AtlasRoot, Font.GetGlyph(text[i]));

                _SpriteBatch.Draw(
                    Font.AtlasTexture,
                    new Vector2((int)(position + glyph.PlaneOrigin).X, (int)(position + glyph.PlaneOrigin).Y),
                    glyph.AtlasRectangle,
                    Color.White);

                position.X += (float)glyph.Advance;
            }
        }

        public Vector2 MeasureString(string text)
        {
            Vector2 tempPos = new Vector2(0, (float)(Font.AtlasRoot.Atlas.Size * Font.AtlasRoot.Metrics.LineHeight));
            bool firstChar = true;
            for (int i = 0; i < text.Length; i++)
            {
                var glyph = new Glyph(Font.AtlasRoot, Font.GetGlyph(text[i]));
                if (firstChar)
                {
                    firstChar = false;
                    //tempPos.X -= glyph.PlaneOrigin.X;
                }
                if (i == text.Length - 1 && glyph.PlaneOffset.X > 0)
                    tempPos.X += glyph.PlaneOffset.X;
                else
                    tempPos.X += (float)glyph.Advance;
            }
            return tempPos;
        }

        private void DebuggerLines(string text, Vector2 position)
        {
            // Debug coordinate lines
            _SpriteBatch.Begin();
            var vp = _GraphicsDevice.Viewport;
            var mid = new Vector2(vp.Width * 0.5f, vp.Height * 0.5f);
            var pixelbl = new Texture2D(_GraphicsDevice, 1, 1);
            var pixelr = new Texture2D(_GraphicsDevice, 1, 1);
            var pixelg = new Texture2D(_GraphicsDevice, 1, 1);
            var pixelb = new Texture2D(_GraphicsDevice, 1, 1);
            pixelbl.SetData(new Color[] { Color.Black });
            pixelr.SetData(new Color[] { Color.Red });
            pixelg.SetData(new Color[] { Color.Lime });
            pixelb.SetData(new Color[] { Color.Blue });
            Color lineIntensity = new Color(1, 1, 1, 0.3f);
            Color rectIntensity = new Color(1, 1, 1, 0.0001f);

            // String Measurement
            var size = MeasureString(text);
            var ascender = Font.AtlasRoot.Atlas.Size * Font.AtlasRoot.Metrics.Ascender;
            //var descender = Font.AtlasRoot.Atlas.Size * Font.AtlasRoot.Metrics.Descender;
            //var height = Font.AtlasRoot.Atlas.Size * Font.AtlasRoot.Metrics.LineHeight;
            //_SpriteBatch.Draw(pixelg, new Rectangle((int)(lastpos.X -1), (int)(vp.Height * 0.5f - ascender), (int)size.X, (int)size.Y), rectIntensity);

            // Horizontal lines
            _SpriteBatch.Draw(pixelbl, new Rectangle(0, (int)mid.Y - 1, vp.Width, 2), lineIntensity);

            _SpriteBatch.Draw(pixelr, new Rectangle(0, (int)(vp.Height * 0.5f - ascender - 1), vp.Width, 2), lineIntensity);
            _SpriteBatch.Draw(pixelr, new Rectangle(0, (int)(vp.Height * 0.5f - ascender - 1 + size.Y), vp.Width, 2), lineIntensity);

            // Vertical Lines
            _SpriteBatch.Draw(pixelbl, new Rectangle((int)mid.X - 1, 0, 2, vp.Height), lineIntensity);
            _SpriteBatch.Draw(pixelbl, new Rectangle(19, 0, 2, vp.Height), lineIntensity);
            _SpriteBatch.Draw(pixelbl, new Rectangle(vp.Width - 21, 0, 2, vp.Height), lineIntensity);

            _SpriteBatch.Draw(pixelr, new Rectangle((int)(position.X - 1), 0, 2, vp.Height), lineIntensity);
            _SpriteBatch.Draw(pixelr, new Rectangle((int)(position.X - 1) + (int)size.X, 0, 2, vp.Height), lineIntensity);

            _SpriteBatch.End();
        }

    }
}
