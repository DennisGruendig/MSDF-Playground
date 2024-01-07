using Microsoft.Xna.Framework.Content.Pipeline;
using System;
using System.IO;
using System.Reflection;

namespace MSDF_Font_Library.Content
{
    [ContentImporter(".ttf", DisplayName = "Shader Font Importer", DefaultProcessor = "ShaderFontProcessor")]
    public class ShaderFontImporter : ContentImporter<ImportData>
    {
        public override ImportData Import(string filename, ContentImporterContext context)
        {
            ImportData data = new ImportData()
            {
                FontFile = Path.GetFullPath(filename),
                FontGenerator = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "msdfgen.exe"),
                AtlasGenerator = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "msdf-atlas-gen.exe"),
                TempFolder = Path.Combine(Directory.GetCurrentDirectory(), "Temp Content", "Shader Fonts", Path.GetFileNameWithoutExtension(filename))
            };

            if (!File.Exists(data.FontGenerator))
                throw new FileNotFoundException($"Font Generator File: {data.FontGenerator}");

            if (!File.Exists(data.AtlasGenerator))
                throw new FileNotFoundException($"Atlas Font Generator File: {data.AtlasGenerator}");

            if (!File.Exists(data.FontFile))
                throw new FileNotFoundException($"Font File: {data.FontFile}");

            if (!Directory.Exists(data.TempFolder))
                Directory.CreateDirectory(data.TempFolder);

            return data;
        }
    }
}
