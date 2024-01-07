using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Graphics;
using MSDF_Font_Library.FontAtlas;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

namespace MSDF_Font_Library.Content
{
    [ContentProcessor(DisplayName = "Shader Font Processor")]
    internal class ShaderFontProcessor : ContentProcessor<ImportData, ShaderFont>
    {
        [DisplayName("Resolution")]
        [Description("Resolution of generated fonts")]
        [DefaultValue(128)]
        public virtual uint Resolution { get; set; } = 128;

        [DisplayName("Distance Range")]
        [Description("Distance field range, in pixels")]
        [DefaultValue(2)]
        public virtual uint DistanceRange { get; set; } = 2;

        private const string DEF_CHARSET = "\"ABCDEFGHIJKLMNOPQRSTUVWXYZÄÖÜabcdefghijklmnopqrstuvwxyzäöü1234567890/*-+,.!?ß´`'°^_:;²³{[]}§$%&()©€@=<>|#~ \\\"\\\\\"";
        private ImportData _ImportData;

        public override ShaderFont Process(ImportData input, ContentProcessorContext context)
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
            var result = JsonSerializer.Deserialize<JsonRoot>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            result.BitmapData = File.ReadAllBytes(_ImportData.Bitmap);
            return result;
        }

    }
}
