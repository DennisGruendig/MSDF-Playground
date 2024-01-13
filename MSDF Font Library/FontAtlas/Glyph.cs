using CppNet;
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
        [ContentSerializer] private readonly char _Character;
        [ContentSerializer] private readonly float _Advance;
        [ContentSerializer] private readonly Dictionary<char, float> _KerningAdvances;
        [ContentSerializer] private readonly Rectangle _AtlasSource;
        [ContentSerializer] private readonly RectangleF _CursorBounds;

        public Glyph() { }

        public Glyph(ShaderFont font, JsonGlyph glyph, List<JsonKerning> kernings)
        {
            _Character = (char)glyph.Unicode;
            _Advance = (float)(glyph.Advance * font.FontSize);

            _KerningAdvances = kernings
                .Where(x => x.Unicode1 == glyph.Unicode)
                .ToDictionary(x => (char)x.Unicode2, x => (float)(x.Advance * font.FontSize));

            _AtlasSource = new Rectangle(
                (int)(glyph.AtlasBounds.Left + 0.5),
                (int)(font.AtlasHeight - glyph.AtlasBounds.Bottom - (glyph.AtlasBounds.Top - glyph.AtlasBounds.Bottom) + 0.5),
                (int)(glyph.AtlasBounds.Right - glyph.AtlasBounds.Left - 0.5),
                (int)(glyph.AtlasBounds.Top - glyph.AtlasBounds.Bottom - 0.5));

            _CursorBounds = new RectangleF(
                (float)(glyph.PlaneBounds.Left * font.FontSize),
                (float)(font.Ascender * font.FontSize - glyph.PlaneBounds.Top * font.FontSize),
                (float)(glyph.PlaneBounds.Right * font.FontSize),
                (float)(-glyph.PlaneBounds.Bottom * font.FontSize));
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
