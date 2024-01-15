using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MSDF_Playground_Game_Library.Graphics_Test_1
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [DataContract]
    public struct LineVertex : IVertexType
    {
        public LineVertex(Vector3 position, float thickness = 1f, Color? color = null)
        {
            color = color ?? Color.White;

            Position = position;
            Thickness = thickness;
            Color = color ?? Color.White;
        }

        public Vector3 Position;
        public float Thickness;
        public Color Color;

        public static readonly VertexDeclaration VertexDeclaration;
        VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;
        
        static LineVertex()
        {
            var elements = new VertexElement[] {
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(sizeof(float) * 4, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 0),
                new VertexElement(sizeof(float) * 5, VertexElementFormat.Color, VertexElementUsage.Color, 0),
                //new VertexElement(sizeof(float) * 3 + sizeof(int), VertexElementFormat.Color, VertexElementUsage.Color, 1),
            };
            VertexDeclaration = new VertexDeclaration(elements);
        }
    }
}
