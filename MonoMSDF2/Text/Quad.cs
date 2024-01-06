using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoMSDF2.Text
{
    public sealed class Quad
    {
        private readonly VertexPositionTexture[] Vertices;
        private readonly short[] Indices;

        public Quad()
        {
            Vertices = new[]
            {
                new VertexPositionTexture(
                    new Vector3(0, 0, 0),
                    new Vector2(1, 1)),
                new VertexPositionTexture(
                    new Vector3(0, 0, 0),
                    new Vector2(0, 1)),
                new VertexPositionTexture(
                    new Vector3(0, 0, 0),
                    new Vector2(0, 0)),
                new VertexPositionTexture(
                    new Vector3(0, 0, 0),
                    new Vector2(1, 0))
            };

            Indices = new short[] { 0, 1, 2, 2, 3, 0 };
        }

        /// <summary>
        /// Renders a fullscreen quad (when using device coordinates)
        /// </summary>        
        public void Render(GraphicsDevice device)
            => Render(device, Vector2.One * -1, Vector2.One);

        public void Render(GraphicsDevice device, Vector2 v1, Vector2 v2)
        {
            Vertices[0].Position.X = v2.X;
            Vertices[0].Position.Y = v1.Y;

            Vertices[1].Position.X = v1.X;
            Vertices[1].Position.Y = v1.Y;

            Vertices[2].Position.X = v1.X;
            Vertices[2].Position.Y = v2.Y;

            Vertices[3].Position.X = v2.X;
            Vertices[3].Position.Y = v2.Y;

            device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, Vertices, 0, 4, Indices, 0, 2);
        }
    }
}
