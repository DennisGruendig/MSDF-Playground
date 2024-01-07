using MSDF_Font_Library;
using MSDF_Font_Library.FontAtlas;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using System.Text;
using MSDF_Font_Library.Content;

namespace Tester
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();



            ShaderFont imported = Process(Import());




        }

        string filename = @"C:\Users\Dennis\source\repos\MSDF Playground\MSDF Playground\Content\TrueTypeFonts\Germany.ttf";
        string library = @"C:\Users\Dennis\source\repos\MSDF Playground\MSDF Font Library\bin\Debug\net6.0";

        private ImportData Import() {
            ImportData data = new ImportData()
            {
                FontFile = Path.GetFullPath(filename),
                FontGenerator = Path.Combine(library, "msdfgen.exe"),
                AtlasGenerator = Path.Combine(library, "msdf-atlas-gen.exe"),
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

        private const uint Resolution = 128;
        private const uint DistanceRange = 2;

        private const string DEF_CHARSET = "\"ABCDEFGHIJKLMNOPQRSTUVWXYZÄÖÜabcdefghijklmnopqrstuvwxyzäöü/*-+,.!?ß´`'°^_:;²³{[]}§$%&()©€@=<>|#~ \\\"\\\\\"";
        private ImportData _ImportData;

        public ShaderFont Process(ImportData input)
        {
            _ImportData = input;

            if (!File.Exists(_ImportData.Charset))
                File.WriteAllText(_ImportData.Charset, DEF_CHARSET);

            var atlas = CreateFontAtlas();
            return new ShaderFont(_ImportData.Name, atlas);
        }

        private JsonRoot CreateFontAtlas()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"-font \"{_ImportData.FontFile}\" ");
            sb.Append($"-imageout \"{_ImportData.Bitmap}\" ");
            sb.Append($"-size {Resolution} -pxrange {DistanceRange} ");
            sb.Append($"-json \"{_ImportData.Json}\" ");
            sb.Append($"-charset \"{_ImportData.Charset}\" ");

            var startInfo = new ProcessStartInfo(_ImportData.AtlasGenerator)
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                Arguments = sb.ToString()
            };

            var process = System.Diagnostics.Process.Start(startInfo);
            if (process is null)
                throw new InvalidOperationException("Could not start msdf-atlas-gen.exe");

            process.WaitForExit();
            return ParseJson();
        }

        private JsonRoot ParseJson()
        {
            if (!File.Exists(_ImportData.Json))
                throw new InvalidOperationException("Could not load Output.json");

            string jsonString = File.ReadAllText(_ImportData.Json);

            var result = JsonSerializer.Deserialize<JsonRoot>(jsonString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (result == null)
                throw new NullReferenceException("Error deserializing json string");

            result.BitmapData = File.ReadAllBytes(_ImportData.Bitmap);
            return result;
        }
    }
}
