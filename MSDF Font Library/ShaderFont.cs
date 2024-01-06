using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MSDF_Font_Library.FontAtlas;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace MSDF_Font_Library
{
    public class ShaderFont
    {
        [ContentSerializer] private readonly char _Fallback;
        [ContentSerializer] private readonly string _Name;
        [ContentSerializer] private readonly JsonRoot _AtlasRoot;
        [ContentSerializer] private readonly Dictionary<char, FontAtlas.JsonGlyph> _Glyphs;

        public ShaderFont() { }

        public ShaderFont(string name, JsonRoot atlasRoot)
        {
            _Fallback = '#';
            _Name = name;
            _AtlasRoot = atlasRoot;
            _Glyphs = atlasRoot.Glyphs.ToDictionary(x => (char)x.Unicode, x => x);
        }

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            var stream = new MemoryStream(_AtlasRoot.BitmapData);
            AtlasTexture = Texture2D.FromStream(graphicsDevice, stream);
        }

        public Texture2D AtlasTexture { get; private set; }

        public string Name => _Name;
        public JsonRoot AtlasRoot => _AtlasRoot;

        [ContentSerializerIgnore]
        public IEnumerable<char> SupportedCharacters => _Glyphs.Keys;

        public FontAtlas.JsonGlyph GetGlyph(char c)
        {
            if (_Glyphs.TryGetValue(c, out FontAtlas.JsonGlyph glyph))
                return glyph;
            if (_Glyphs.TryGetValue(_Fallback, out FontAtlas.JsonGlyph fallback))
            {
                MissingCharacters.Add(_Name, c);
                return fallback;
            }
            MissingCharacters.Add(_Name, _Fallback);
            throw new InvalidOperationException($"Fallback character '{_Fallback}' not found in font {_Name}.");
        }

    }
}
