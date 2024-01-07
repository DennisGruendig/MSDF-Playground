using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSDF_Font_Library.Content
{
    public class ImportData
    {
        public string FontGenerator;
        public string AtlasGenerator;
        public string FontFile;
        public string TempFolder;

        public string Name => Path.GetFileNameWithoutExtension(FontFile);
        public string Charset => Path.Combine(TempFolder, "Charset.txt");
        public string Json => Path.Combine(TempFolder, "Output.json");
        public string Bitmap => Path.Combine(TempFolder, "Atlas.bmp");

    }
}
