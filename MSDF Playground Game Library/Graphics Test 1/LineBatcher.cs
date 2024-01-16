using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MSDF_Playground_Game_Library.Graphics_Test_1
{
    public class LineBatcher
    {
        public LineBatcher(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
            _RasterizerState = new RasterizerState();
            _RasterizerState.MultiSampleAntiAlias = false;
            _effect = new BasicEffect(graphicsDevice);

            _effect.TextureEnabled = false;
            _effect.FogEnabled = false;
            _effect.LightingEnabled = false;
            _effect.VertexColorEnabled = true;
            _effect.World = Matrix.Identity;
            _effect.View = Matrix.Identity;
            _effect.Projection = Matrix.Identity;

            //_vertices = new LineVertex[DEF_VERTICES];
            _vertices = new VertexPositionColor[DEF_VERTICES];
            _indices = new int[DEF_INDICES];

            for (int i = 0; i < _indices.Length; i += 1)
            {
                _indices[i] = i;
            }

            //_vertexBuffer = new DynamicVertexBuffer(_graphicsDevice, typeof(LineVertex), _vertices.Length, BufferUsage.WriteOnly);
            _vertexBuffer = new DynamicVertexBuffer(_graphicsDevice, typeof(VertexPositionColor), _vertices.Length, BufferUsage.WriteOnly);
            _indexBuffer = new IndexBuffer(_graphicsDevice, typeof(uint), _indices.Length, BufferUsage.WriteOnly);
            _indexBuffer.SetData(_indices);
        }

        ~LineBatcher()
        {
            _effect?.Dispose();
        }

        private const int DEF_VERTICES = 2048;
        private const int DEF_INDICES = 2048;

        private GraphicsDevice _graphicsDevice;
        private RasterizerState _RasterizerState;
        private RasterizerState _PreviousRasterizerState;
        private BasicEffect _effect;
        private Matrix _view;
        private Matrix _projection;
        private Viewport _viewport { get => _graphicsDevice.Viewport; }

        //private LineVertex[] _vertices;
        private VertexPositionColor[] _vertices;
        private int[] _indices;

        private int _lineCount = 0;
        private int _vertexCount = 0;
        private int _indexCount { get => _vertexCount * 2; }

        private DynamicVertexBuffer _vertexBuffer;
        private IndexBuffer _indexBuffer;

        private bool _beginCalled;

        public void Draw(Vector3 position1, Vector3 position2, Color? color = null, float thickness = 1f)
        {
            if (_vertices.Length < _vertexCount + 3) return;

            var actColor = color ?? Color.White;
            //_vertices[_vertexCount++] = new LineVertex(position1, thickness, actColor);
            //_vertices[_vertexCount++] = new LineVertex(position2, thickness, actColor);
            _vertices[_vertexCount++] = new VertexPositionColor(position1, actColor);
            _vertices[_vertexCount++] = new VertexPositionColor(position2, actColor);
            _lineCount += 1;
        }

        public void Begin(Matrix? view = null, Matrix? projection = null)
        {
            if (_beginCalled) return;
            _beginCalled = true;
            _view = view ?? Matrix.Identity;
            _projection = projection
                ?? Matrix.CreateOrthographicOffCenter(_viewport.X, _viewport.Width, _viewport.Height, _viewport.Y, 0, 1);
            _PreviousRasterizerState = _graphicsDevice.RasterizerState;
            _graphicsDevice.RasterizerState = _RasterizerState;
        }

        public void End()
        {
            if (!_beginCalled) return;
            _beginCalled = false;
            Flush();
            _graphicsDevice.RasterizerState = _PreviousRasterizerState;
        }

        private void Flush()
        {
            if (_lineCount <= 0) return;

            //_effect.Parameters["WorldViewProjection"].SetValue(_view * _projection);

            _vertexBuffer.SetData(_vertices);
            _graphicsDevice.SetVertexBuffer(_vertexBuffer);

            _graphicsDevice.Indices = _indexBuffer;

            //if (_graphicsDevice.DepthStencilState != DepthStencilState.Default)
            //    return;

            //if (_graphicsDevice.BlendState != BlendState.AlphaBlend)
            //    return;

            _effect.View = _view;
            _effect.Projection = _projection;

            EffectPassCollection passes = _effect.CurrentTechnique.Passes;
            for (int i = 0; i < passes.Count; i++)
            {
                passes[0].Apply();
                _graphicsDevice.DrawIndexedPrimitives(PrimitiveType.LineList, 0, 0, _lineCount);
            }

            _lineCount = 0;
            _vertexCount = 0;
        }

    }
}
