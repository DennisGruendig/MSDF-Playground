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
        private char _Character;
        private double _Advance;
        private Vector2 _AtlasSize;
        private Vector2 _AtlasOrigin;
        private Vector2 _PlaneOrigin;
        private Vector2 _PlaneOffset;
        private Rectangle _AtlasRectangle;

        public Glyph(JsonRoot jsonRoot, JsonGlyph jsonGlyph)
        {
            _Character = (char)jsonGlyph.Unicode;
            _Advance = jsonGlyph.Advance * jsonRoot.Atlas.Size;

            if (!(jsonGlyph.Unicode == 32))
            {
                _AtlasSize = new Vector2(
                    (float)(jsonGlyph.AtlasBounds.Right - jsonGlyph.AtlasBounds.Left),
                    (float)(jsonGlyph.AtlasBounds.Top - jsonGlyph.AtlasBounds.Bottom));

                _AtlasOrigin = new Vector2(
                    (float)jsonGlyph.AtlasBounds.Left,
                    (float)(jsonRoot.Atlas.Height - jsonGlyph.AtlasBounds.Bottom - AtlasSize.Y));

                _AtlasRectangle = new Rectangle(
                    (int)AtlasOrigin.X,
                    (int)AtlasOrigin.Y,
                    (int)AtlasSize.X,
                    (int)AtlasSize.Y);

                _PlaneOrigin = new Vector2(
                    (float)(jsonGlyph.PlaneBounds.Left * jsonRoot.Atlas.Size),
                    (float)(-jsonGlyph.PlaneBounds.Top * jsonRoot.Atlas.Size));

                _PlaneOffset = new Vector2(
                    (float)(jsonGlyph.PlaneBounds.Right * jsonRoot.Atlas.Size),
                    (float)(-jsonGlyph.PlaneBounds.Bottom * jsonRoot.Atlas.Size));
            }
        }

        public char Character { get => _Character; }
        public double Advance { get => _Advance; }
        public Vector2 AtlasSize { get => _AtlasSize; }
        public Vector2 AtlasOrigin { get => _AtlasOrigin; }
        public Vector2 PlaneOrigin { get => _PlaneOrigin; }
        public Vector2 PlaneOffset { get => _PlaneOffset; }
        public Rectangle AtlasRectangle { get => _AtlasRectangle; }
    }
}
