using Microsoft.Xna.Framework.Content;

namespace FontExtension
{    
    public class FieldGlyph
    {        
        [ContentSerializer] private readonly char CharacterBackend;
        [ContentSerializer] private readonly byte[] BitmapBackend;
        [ContentSerializer] private readonly Metrics MetricsBackend;

        public FieldGlyph()
        {
           
        }

        public FieldGlyph(char character, byte[] bitmap, Metrics metrics)
        {
            CharacterBackend = character;
            BitmapBackend = bitmap;
            MetricsBackend = metrics;
        }
        
        /// <summary>
        /// The character this glyph represents
        /// </summary>
        public char Character => CharacterBackend;
        /// <summary>
        /// Distance field for this character
        /// </summary>
        public byte[] Bitmap => BitmapBackend;                
        /// <summary>
        /// Metrics for this character
        /// </summary>
        public Metrics Metrics => MetricsBackend;
    }
}
