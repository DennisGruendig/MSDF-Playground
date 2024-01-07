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
        [ContentSerializer] private readonly float _LineHeight;
        [ContentSerializer] private readonly float _Ascender;
        [ContentSerializer] private readonly float _Descender;
        [ContentSerializer] private readonly float _UnderlineY;
        [ContentSerializer] private readonly float _UnderlineThickness;
        [ContentSerializer] private readonly float _LineOffset;

        [ContentSerializer] private Texture2D _AtlasTexture;

        public ShaderFont() { }

        public ShaderFont(string name, JsonRoot json, byte[] bitmap)
        {
            _Fallback = '?';
            _Name = name;
            _AtlasBitmap = bitmap;

            _DistanceRange = json.Atlas.DistanceRange;
            _FontSize = json.Atlas.Size;
            _AtlasWidth = json.Atlas.Width;
            _AtlasHeight = json.Atlas.Height;

            _LineHeight = (float)json.Metrics.LineHeight;
            _Ascender = (float)json.Metrics.Ascender;
            _Descender = (float)json.Metrics.Descender;
            _UnderlineY = (float)json.Metrics.UnderlineY;
            _UnderlineThickness = (float)json.Metrics.UnderlineThickness;
            _LineOffset = (float)(json.Atlas.Size * json.Metrics.LineHeight);

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
        public float LineHeight => _LineHeight;
        public float Ascender => _Ascender;
        public float Descender => _Descender;
        public float UnderlineY => _UnderlineY;
        public float UnderlineThickness => _UnderlineThickness;
        public float LineOffset => _LineOffset;

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            var stream = new MemoryStream(_AtlasBitmap);
            _AtlasTexture = Texture2D.FromStream(graphicsDevice, stream);
        }

        public Glyph GetGlyph(char character)
        {
            if (_Glyphs.TryGetValue(character, out Glyph glyph))
                return glyph;
            MissingCharacters.Add(_Name, character);
            if (_Glyphs.TryGetValue(_Fallback, out Glyph glyph2))
                return glyph2;
            MissingCharacters.Add(_Name, _Fallback);
            throw new InvalidOperationException($"Character '{character}' and fallback '{_Fallback}' both not found.");
        }

    }
}
