using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MSDF_Font_Library.Rendering
{
    public struct GlyphDrawCall
    {
        public GlyphDrawCall(Rectangle sourceRectangle)
        {
            SourceRectangle = sourceRectangle;
            Position = Vector2.Zero;
            Scale = Vector2.One;
        }
        public GlyphDrawCall(Rectangle sourceRectangle, Vector2 position, Vector2? scale = null)
        {
            SourceRectangle = sourceRectangle;
            Position = position;
            Scale = scale ?? Vector2.One;
        }

        public readonly Rectangle SourceRectangle { get; }
        public Vector2 Position { get; set; }
        public Vector2 Scale { get; set; }

    }
}
