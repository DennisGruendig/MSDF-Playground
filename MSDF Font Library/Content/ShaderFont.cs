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
        [ContentSerializer] private readonly float _Height;
        [ContentSerializer] private readonly float _LineHeight;
        [ContentSerializer] private readonly float _Ascender;
        [ContentSerializer] private readonly float _Descender;
        [ContentSerializer] private readonly float _UnderlineY;
        [ContentSerializer] private readonly float _UnderlineThickness;
        [ContentSerializer] private readonly float _ActualHeight;
        [ContentSerializer] private readonly float _ActualBaseLine;
        [ContentSerializer] private readonly float _ActualLineHeight;

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
            _Height = MathF.Abs((float)json.Metrics.Ascender) + MathF.Abs((float)json.Metrics.Descender);
            _LineHeight = (float)json.Metrics.LineHeight;
            _Ascender = (float)json.Metrics.Ascender;
            _Descender = (float)json.Metrics.Descender;
            _UnderlineY = (float)json.Metrics.UnderlineY;
            _UnderlineThickness = (float)json.Metrics.UnderlineThickness;
            _ActualHeight = json.Atlas.Size * (MathF.Abs((float)json.Metrics.Ascender) + MathF.Abs((float)json.Metrics.Descender));
            _ActualBaseLine = (float)(json.Atlas.Size * json.Metrics.Ascender);
            _ActualLineHeight = (float)(json.Atlas.Size * json.Metrics.LineHeight);
            _Glyphs = json.Glyphs
                .Select(x => new Glyph(this, x))
                .OrderBy(x => x.Character)
                .ToDictionary(x => x.Character, x => x);
        }

        public string Name => _Name;
        public Texture2D AtlasTexture => _AtlasTexture;
        public int DistanceRange => _DistanceRange;
        public float FontSize => _FontSize;
        public int AtlasWidth => _AtlasWidth;
        public int AtlasHeight => _AtlasHeight;
        public float Height => _Height;
        public float LineHeight => _LineHeight;
        public float Ascender => _Ascender;
        public float Descender => _Descender;
        public float UnderlineY => _UnderlineY;
        public float UnderlineThickness => _UnderlineThickness;
        public float ActualHeight => _ActualHeight;
        public float ActualBaseLine => _ActualBaseLine;
        public float ActualLineHeight => _ActualLineHeight;

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
            throw new InvalidOperationException($"Character '{character}' and fallback '{_Fallback}' both not found.");
        }

    }
}
