using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MSDF_Font_Library.Content;
using MSDF_Font_Library.Datatypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDF_Font_Library.FontAtlas
{
    public class Glyph
    {
        [ContentSerializerIgnore] private readonly ShaderFont _Font;
        [ContentSerializer] private readonly char _Character;
        [ContentSerializer] private readonly float _Advance;
        [ContentSerializer] private readonly Rectangle _AtlasSource;
        [ContentSerializer] private readonly RectangleF _CursorBounds;

        public Glyph() { }

        public Glyph(ShaderFont font, JsonGlyph glyph)
        {
            _Font = font;
            _Character = (char)glyph.Unicode;
            _Advance = (float)(glyph.Advance * font.FontSize);

            if (!(glyph.Unicode == 32))
            {
                _AtlasSource = new Rectangle(
                    (int)(glyph.AtlasBounds.Left),
                    (int)(font.AtlasHeight - glyph.AtlasBounds.Bottom - (glyph.AtlasBounds.Top - glyph.AtlasBounds.Bottom)),
                    (int)(glyph.AtlasBounds.Right - glyph.AtlasBounds.Left),
                    (int)(glyph.AtlasBounds.Top - glyph.AtlasBounds.Bottom));

                _CursorBounds = new RectangleF(
                    (float)(glyph.PlaneBounds.Left * font.FontSize),
                    (float)(-glyph.PlaneBounds.Top * font.FontSize + font.Ascender * font.FontSize),
                    (float)(glyph.PlaneBounds.Right * font.FontSize),
                    (float)(glyph.PlaneBounds.Bottom * font.FontSize));
            }
        }

        public char Character { get => _Character; }
        public Rectangle AtlasSource { get => _AtlasSource; }
        public float Advance { get => _Advance; }
        public RectangleF CursorBounds { get => _CursorBounds; }
    }
}
