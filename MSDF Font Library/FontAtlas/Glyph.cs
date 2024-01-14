using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using MSDF_Font_Library.Content;
using MSDF_Font_Library.Datatypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MSDF_Font_Library.FontAtlas
{
    public class Glyph
    {
        [ContentSerializer] private readonly char _Character;
        [ContentSerializer] private readonly float _Advance;
        [ContentSerializer] private readonly Dictionary<char, float> _KerningAdvances;
        [ContentSerializer] private readonly Rectangle _AtlasSource;
        [ContentSerializer] private readonly RectangleF _CursorBounds;

        public Glyph() { }

        public Glyph(ShaderFont font, JsonGlyph glyph, List<JsonKerning> kernings)
        {
            _Character = (char)glyph.Unicode;
            _Advance = (float)Math.Round(glyph.Advance, 7) * font.FontSize;

            _KerningAdvances = kernings
                .Where(x => x.Unicode1 == glyph.Unicode)
                .ToDictionary(x => (char)x.Unicode2, x => (float)Math.Round(x.Advance, 7) * font.FontSize);

            _AtlasSource = new Rectangle(
                (int)Math.Round(glyph.AtlasBounds.Left + 0.5, 1),
                (int)Math.Round(font.AtlasHeight - glyph.AtlasBounds.Bottom - (glyph.AtlasBounds.Top - glyph.AtlasBounds.Bottom) + 0.5, 1),
                (int)Math.Round(glyph.AtlasBounds.Right - glyph.AtlasBounds.Left - 1, 1),
                (int)Math.Round(glyph.AtlasBounds.Top - glyph.AtlasBounds.Bottom - 1, 1));

            _CursorBounds = new RectangleF(
                (float)Math.Round(glyph.PlaneBounds.Left, 7) * font.FontSize,
                font.Ascender * font.FontSize - (float)Math.Round(glyph.PlaneBounds.Top, 7) * font.FontSize,
                (float)Math.Round(glyph.PlaneBounds.Right, 7) * font.FontSize,
                (float)Math.Round(-glyph.PlaneBounds.Bottom, 7) * font.FontSize);
        }

        public char Character { get => _Character; }
        public Rectangle AtlasSource { get => _AtlasSource; }
        public RectangleF CursorBounds { get => _CursorBounds; }

        public float GetAdvance()
        {
            return _Advance;
        }
        public float GetAdvance(char? nextChar)
        {
            if (nextChar is null || _KerningAdvances is null)
                return _Advance;

            if (_KerningAdvances.TryGetValue(nextChar.Value, out float advance))
                return _Advance + advance;

            else
                return _Advance;
        }
    }
}
