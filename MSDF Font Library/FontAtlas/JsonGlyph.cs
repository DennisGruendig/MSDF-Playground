namespace MSDF_Font_Library.FontAtlas
{
    public class JsonGlyph
    {
        public int Unicode { get; set; }
        public double Advance { get; set; }
        public JsonBounds PlaneBounds { get; set; }
        public JsonBounds AtlasBounds { get; set; }
    }
}
