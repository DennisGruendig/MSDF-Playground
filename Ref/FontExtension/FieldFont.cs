using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Content;

namespace FontExtension
{
    public class FieldFont
    {
        [ContentSerializer] private readonly Dictionary<char, FieldGlyph> Glyphs;
        [ContentSerializer] private readonly string NameBackend;
        [ContentSerializer] private readonly float PxRangeBackend;
        [ContentSerializer] private readonly List<KerningPair> KerningPairsBackend;

        public FieldFont()
        {            
        }

        public FieldFont(string name, IReadOnlyCollection<FieldGlyph> glyphs, IReadOnlyCollection<KerningPair> kerningPairs, float pxRange)
        {
            NameBackend = name;
            PxRangeBackend = pxRange;
            KerningPairsBackend = kerningPairs.ToList();

            //Glyphs = new Dictionary<char, FieldGlyph>(glyphs.Count);
            //foreach (var glyph in glyphs)
            //{
            //    Glyphs.Add(glyph.Character, glyph);
            //}

            Glyphs = glyphs.ToDictionary(x => x.Character, x => x);
        }

        /// <summary>
        /// Name of the font
        /// </summary>
        public string Name => NameBackend;
        
        /// <summary>
        /// Distance field effect range in pixels
        /// </summary>
        public float PxRange => PxRangeBackend;

        /// <summary>
        /// Kerning pairs available in this font
        /// </summary>
        public IReadOnlyList<KerningPair> KerningPairs => KerningPairsBackend;

        /// <summary>
        /// Characters supported by this font
        /// </summary>
        [ContentSerializerIgnore]
        public IEnumerable<char> SupportedCharacters => Glyphs.Keys;
       
        /// <summary>
        /// Returns the glyph for the given character, or throws an exception when the glyph is not supported by this font
        /// </summary>        
        public FieldGlyph GetGlyph(char c)
        {
            if (Glyphs.TryGetValue(c, out FieldGlyph glyph))
            {
                return glyph;
            }
            else if (Glyphs.TryGetValue('*', out FieldGlyph glyph2))
            {
                return glyph2;
            }
            throw new InvalidOperationException($"Character '{c}' not found in font {Name}. Did you forget to include it in the character ranges?");
        }
    }
}
