using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDF_Font_Library.FontAtlas
{
    public class Glyph
    {
        public Glyph(JsonRoot jsonRoot, JsonGlyph jsonGlyph)
        {
            _Character = (char)jsonGlyph.Unicode;
            _Advance = jsonGlyph.Advance * jsonRoot.Atlas.Size;

            if (jsonGlyph.Unicode == 32)
            {
                _AtlasSize = new Vector2(0, 0);
                _AtlasOrigin = Vector2.Zero;
                _AtlasRectangle = new Rectangle(0, 0, 0, 0);
            }
            else
            {
                _AtlasSize = new Vector2(
                    jsonGlyph.AtlasBounds.Right - jsonGlyph.AtlasBounds.Left,
                    jsonGlyph.AtlasBounds.Top - jsonGlyph.AtlasBounds.Bottom);

                _AtlasOrigin = new Vector2(
                    jsonGlyph.AtlasBounds.Left,
                    jsonRoot.Atlas.Height - jsonGlyph.AtlasBounds.Bottom - AtlasSize.Y);

                _AtlasRectangle = new Rectangle(
                    (int)AtlasOrigin.X,
                    (int)AtlasOrigin.Y,
                    (int)AtlasSize.X,
                    (int)AtlasSize.Y);

                _PlaneOrigin = new Vector2(
                    jsonGlyph.PlaneBounds.Left * jsonRoot.Atlas.Size,
                    - jsonGlyph.PlaneBounds.Bottom * jsonRoot.Atlas.Size);
            }
        }

        private char _Character;
        private float _Advance;
        private Vector2 _AtlasSize;
        private Vector2 _AtlasOrigin;
        private Vector2 _PlaneOrigin;
        private Rectangle _AtlasRectangle;

        public char Character { get => _Character; }
        public float Advance { get => _Advance * Scaling.X; }
        public Vector2 AtlasSize { get => _AtlasSize; }
        public Vector2 AtlasOrigin { get => _AtlasOrigin; }
        public Vector2 PlaneOrigin { get => _PlaneOrigin * Scaling; }
        public Rectangle AtlasRectangle { get => _AtlasRectangle; }
        public Vector2 Scaling { get; set; } = new Vector2(1f);
    }
}
