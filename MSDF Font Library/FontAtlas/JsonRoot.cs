using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MSDF_Font_Library.FontAtlas
{
    public class JsonRoot
    {
        public JsonAtlas Atlas { get; set; }
        public JsonMetrics Metrics { get; set; }
        public List<JsonGlyph> Glyphs { get; set; }
        //public List<object> Kerning { get; set; }

        // ToDo Löschen
        public byte[] BitmapData { get; set; }
    }
}
