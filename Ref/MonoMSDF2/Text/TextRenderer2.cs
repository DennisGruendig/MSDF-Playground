using System.Collections.Generic;
using System.IO;
using System.Linq;
using FontExtension;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoMSDF2.Text
{
    public sealed class TextRenderer2
    {
        private const string LargeTextTechnique = "LargeText";
        private const string SmallTextTechnique = "SmallText";

        private readonly Effect Effect;
        private readonly FieldFont Font;
        private readonly GraphicsDevice Device;
        private readonly Quad Quad;       
        private readonly Dictionary<char, GlyphRenderInfo> Cache;        
      
        public TextRenderer2(Effect effect, FieldFont font, GraphicsDevice device)
        {
            Effect = effect;
            Font = font;
            Device = device;

            Quad = new Quad();
            Cache = new Dictionary<char, GlyphRenderInfo>();

            ForegroundColor = Color.White;
            EnableKerning = true;
            OptimizeForTinyText = false;
        }

        public Color ForegroundColor { get; set; }
        public bool EnableKerning { get; set; }

        /// <summary>
        /// Disables text anti-aliasing which might cause blurry text when the text is rendered tiny
        /// </summary>
        public bool OptimizeForTinyText { get; set; }

        public void Render(string text, Vector2 position, Vector2 scale)
        {
            if (string.IsNullOrEmpty(text))
                return;

            var sequence = text.Select(GetRenderInfo).ToArray();
            var textureWidth = sequence[0].Texture.Width;
            var textureHeight = sequence[0].Texture.Height;

            var fullWidth = sequence.Select(x => x.Texture.Width).Sum();

            var world = Matrix.CreateWorld(new Vector3(0, 0, 0), Vector3.Forward, Vector3.Up);
            var view = Matrix.CreateLookAt(Vector3.Backward, Vector3.Forward, Vector3.Up);
            var projection = Matrix.CreateOrthographicOffCenter(0, Device.Viewport.Width, 0, Device.Viewport.Height, 0, 1000);

            var translation = Matrix.CreateTranslation(2f / Device.Viewport.Width * position.X, 2f / Device.Viewport.Height * position.Y, 0);
            var scaling = Matrix.CreateScale(scale.X, scale.Y, 1f);

            Effect.Parameters["WorldViewProjection"].SetValue(scaling * world * view * projection * translation);
            Effect.Parameters["PxRange"].SetValue(Font.PxRange);
            Effect.Parameters["TextureSize"].SetValue(new Vector2(textureWidth, textureHeight));
            Effect.Parameters["ForegroundColor"].SetValue(Color.White.ToVector4());

            Effect.CurrentTechnique = OptimizeForTinyText ? Effect.Techniques[SmallTextTechnique] : Effect.Techniques[LargeTextTechnique];

            var pen = Vector2.Zero;
            for (var i = 0; i < sequence.Length; i++)
            {
                var current = sequence[i];

                Effect.Parameters["GlyphTexture"].SetValue(current.Texture);
                Effect.CurrentTechnique.Passes[0].Apply();

                var glyphHeight = textureHeight * (1.0f / current.Metrics.Scale);
                var glyphWidth = textureWidth * (1.0f / current.Metrics.Scale);

                var left = pen.X - current.Metrics.Translation.X;
                var bottom = pen.Y - current.Metrics.Translation.Y;

                var right = left + glyphWidth;
                var top = bottom + glyphHeight;

                if (!char.IsWhiteSpace(current.Character))
                    Quad.Render(Device, new Vector2(left, bottom), new Vector2(right, top));

                pen.X += current.Metrics.Advance;

                if (EnableKerning && i < sequence.Length - 1)
                {
                    var next = sequence[i + 1];

                    var pair = Font.KerningPairs.FirstOrDefault(
                        x => x.Left == current.Character && x.Right == next.Character);

                    if (pair != null)
                        pen.X += pair.Advance;
                }
            }
        }

        private GlyphRenderInfo GetRenderInfo(char c)
        {
            if(Cache.TryGetValue(c, out var value))
                return value;

            var unit = LoadRenderInfo(c);
            Cache.Add(c, unit);
            return unit;
        }

        private GlyphRenderInfo LoadRenderInfo(char c)
        {
            var glyph = Font.GetGlyph(c);
            using (var stream = new MemoryStream(glyph.Bitmap))
            {
                var texture = Texture2D.FromStream(Device, stream);
                var unit = new GlyphRenderInfo(c, texture, glyph.Metrics);
                           
                return unit;
            }
        }
    }
}
