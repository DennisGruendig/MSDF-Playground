using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace FontExtension
{
    public class Metrics
    {
        [ContentSerializer] private readonly float AdvanceBackend;
        [ContentSerializer] private readonly float ScaleBackend;
        [ContentSerializer] private readonly Vector2 TranslationBackend;

        public Metrics()
        {
            
        }

        public Metrics(float advance, float scale, Vector2 translation)
        {
            AdvanceBackend = advance;
            ScaleBackend = scale;
            TranslationBackend = translation;
        }

        public float Advance => AdvanceBackend;
        public float Scale => ScaleBackend;
        public Vector2 Translation => TranslationBackend;
    }
}
