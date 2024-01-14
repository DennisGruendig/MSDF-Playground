using Assimp;
using Assimp.Configs;
using Microsoft.Xna.Framework.Content.Pipeline;
using MSDF_Font_Library.FontAtlas;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;

namespace MSDF_Font_Library.Content
{
    [ContentProcessor(DisplayName = "Shader Font Processor - MSDF Font Library")]
    internal class ShaderFontProcessor : ContentProcessor<ImportData, ShaderFont>
    {
        [DisplayName("Resolution")]
        [Description("Resolution of generated fonts")]
        [DefaultValue(256)]
        public virtual uint Resolution { get; set; } = 256;

        [DisplayName("Distance Range")]
        [Description("Distance field range, in pixels")]
        [DefaultValue(2)]
        public virtual uint DistanceRange { get; set; } = 2;

        [DisplayName("Charset Definition (optional)")]
        [Description("String of all the chars, for which to create glyphs")]
        public virtual string CharsetDefinition { get; set; } = string.Empty;

        [DisplayName("Keep Temporary Data")]
        [Description("Prevents deletion of raw atlas data files")]
        [DefaultValue(false)]
        public virtual bool KeepTemp { get; set; } = false;

        [DisplayName("Ignore Kerning")]
        [Description("Ignores kerning data obtained from the font file")]
        [DefaultValue(false)]
        public virtual bool IgnoreKerning { get; set; } = false;

        private const string DEF_CHARSET = "ABCDEFGHIJKLMNOPQRSTUVWXYZÄÖÜabcdefghijklmnopqrstuvwxyzäöü1234567890/*-+,.!?ß´`'°^_:;²³{[]}§$%&()©€@=<>|#~ \\\"\\\\µ";
        private ImportData _ImportData;

        public override ShaderFont Process(ImportData input, ContentProcessorContext context)
        {
            _ImportData = input;

            if (string.IsNullOrEmpty(CharsetDefinition))
                CharsetDefinition = DEF_CHARSET;

            File.WriteAllText(_ImportData.Charset, $"\"{CharsetDefinition}\"");
            if (!File.Exists(_ImportData.Charset))
                throw new InvalidOperationException("Could not find Charset.txt");

            JsonRoot atlas = CreateFontAtlas();

            if (!File.Exists(_ImportData.AtlasImage))
                throw new InvalidOperationException("Could not find Atlas.png");

            byte[] bitmap = File.ReadAllBytes(_ImportData.AtlasImage);

            if (!KeepTemp)
                Directory.Delete(_ImportData.TempFolder, true);

            return new ShaderFont(_ImportData.Name, atlas, bitmap);
        }

        private JsonRoot CreateFontAtlas()
        {
            StringBuilder args = new StringBuilder();
            args.Append($"-font \"{_ImportData.FontFile}\" -scanline -type mtsdf ");
            args.Append($"-size {Resolution} -pxrange {DistanceRange} ");
            args.Append($"-imageout \"{_ImportData.AtlasImage}\" ");
            args.Append($"-json \"{_ImportData.Json}\" ");
            args.Append($"-charset \"{_ImportData.Charset}\" ");

            var startInfo = new ProcessStartInfo(_ImportData.AtlasGenerator)
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                Arguments = args.ToString()
            };

            var process = System.Diagnostics.Process.Start(startInfo);
            if (process is null)
                throw new InvalidOperationException("Could not start msdf-atlas-gen.exe");
            process.WaitForExit();

            if (!File.Exists(_ImportData.Json))
                throw new InvalidOperationException("Could not find Output.json");

            string jsonString = File.ReadAllText(_ImportData.Json);
            JsonRoot deserialized = JsonSerializer.Deserialize<JsonRoot>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            deserialized.IgnoreKerning = IgnoreKerning;

            return deserialized;
        }

    }
}
