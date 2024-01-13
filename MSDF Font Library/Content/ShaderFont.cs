using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MSDF_Font_Library.FontAtlas;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MSDF_Font_Library.Content
{
    public class ShaderFont
    {
        [ContentSerializer] private readonly char _Fallback;
        [ContentSerializer] private readonly string _Name;
        [ContentSerializer] private readonly Dictionary<char, Glyph> _Glyphs;
        [ContentSerializer] private readonly byte[] _AtlasBitmap;
        [ContentSerializer] private readonly int _DistanceRange;
        [ContentSerializer] private readonly int _FontSize;
        [ContentSerializer] private readonly int _AtlasWidth;
        [ContentSerializer] private readonly int _AtlasHeight;
        [ContentSerializer] private readonly double _Height;
        [ContentSerializer] private readonly double _LineHeight;
        [ContentSerializer] private readonly double _Ascender;
        [ContentSerializer] private readonly double _Descender;
        [ContentSerializer] private readonly double _UnderlineY;
        [ContentSerializer] private readonly double _UnderlineThickness;
        [ContentSerializer] private readonly double _ActualHeight;
        [ContentSerializer] private readonly double _ActualBaseLine;
        [ContentSerializer] private readonly double _ActualLineHeight;

        [ContentSerializer] private Texture2D _AtlasTexture;

        public ShaderFont() { }

        public ShaderFont(string name, JsonRoot json, byte[] bitmap)
        {
            _Fallback = '';
            _Name = name;
            _AtlasBitmap = bitmap;
            _DistanceRange = json.Atlas.DistanceRange;
            _FontSize = json.Atlas.Size;
            _AtlasWidth = json.Atlas.Width;
            _AtlasHeight = json.Atlas.Height;
            _Height = Math.Abs(json.Metrics.Ascender) + Math.Abs(json.Metrics.Descender);
            _LineHeight = json.Metrics.LineHeight;
            _Ascender = json.Metrics.Ascender;
            _Descender = json.Metrics.Descender;
            _UnderlineY = json.Metrics.UnderlineY;
            _UnderlineThickness = json.Metrics.UnderlineThickness;
            
            _ActualHeight = json.Atlas.Size * (Math.Abs(json.Metrics.Ascender) + Math.Abs(json.Metrics.Descender));
            _ActualBaseLine = json.Atlas.Size * json.Metrics.Ascender;
            _ActualLineHeight = json.Atlas.Size * json.Metrics.LineHeight;
            
            if (json.IgnoreKerning)
                json.Kerning.Clear();

            _Glyphs = json.Glyphs
                .Select(x => new Glyph(this, x, json.Kerning))
                .OrderBy(x => x.Character)
                .ToDictionary(x => x.Character, x => x);
        }

        public string Name => _Name;
        public Texture2D AtlasTexture => _AtlasTexture;
        public int DistanceRange => _DistanceRange;
        public double FontSize => _FontSize;
        public int AtlasWidth => _AtlasWidth;
        public int AtlasHeight => _AtlasHeight;
        public double Height => _Height;
        public double LineHeight => _LineHeight;
        public double Ascender => _Ascender;
        public double Descender => _Descender;
        public double UnderlineY => _UnderlineY;
        public double UnderlineThickness => _UnderlineThickness;
        public double ActualHeight => _ActualHeight;
        public double ActualBaseLine => _ActualBaseLine;
        public double ActualLineHeight => _ActualLineHeight;

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            var stream = new MemoryStream(_AtlasBitmap);
            _AtlasTexture = Texture2D.FromStream(graphicsDevice, stream);
        }

        public bool CheckGlyphAvailability(char character)
        {
            return _Glyphs.TryGetValue(character, out Glyph glyph);
        }

        public Glyph GetGlyph(char character)
        {
            if (_Glyphs.TryGetValue(character, out Glyph glyph))
                return glyph;
            if (_Glyphs.TryGetValue(_Fallback, out Glyph glyphFallback))
                return glyphFallback;
            return new Glyph();
        }

    }
}
