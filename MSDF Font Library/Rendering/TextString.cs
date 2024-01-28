using Microsoft.Xna.Framework;
using MSDF_Font_Library.Content;
using MSDF_Font_Library.Datatypes;
using MSDF_Font_Library.FontAtlas;
using System;
using System.Linq;

namespace MSDF_Font_Library.Rendering
{
    public class TextString
    {
        private ShaderFont _font;
        private string _text;
        private Vector2 _position;
        private Vector2 _scale;
        private Vector2 _size;
        private HorizontalAlignment _horiAlignment;
        private VerticalAlignment _vertAlignment;
        private Glyph[] _glyphs;
        private GlyphDrawCall[] _drawCallBuffer;

        public TextString(ShaderFont font, string text = null, Vector2? position = null, Vector2? scale = null, HorizontalAlignment horiAlignment = HorizontalAlignment.Left, VerticalAlignment vertAlignment = VerticalAlignment.Base)
        {
            _font = font;
            _text = text ?? string.Empty;
            _position = position ?? Vector2.Zero;
            _scale = scale ?? Vector2.One;
            _horiAlignment = horiAlignment;
            _vertAlignment = vertAlignment;
            InitializeArrays();
        }

        private void InitializeArrays()
        {
            _glyphs = _text
                .Select(x => _font.GetGlyph(x))
                .ToArray();

            _drawCallBuffer = _glyphs
                .Select(x => new GlyphDrawCall(x.AtlasSource))
                .ToArray();

            UpdateDrawCallBuffer();
        }

        private void UpdateDrawCallBuffer()
        {
            double cursorTravel = 0;
            bool isLast;
            char? nextChar;
            double advance;
            Vector2 offset = Vector2.Zero;

            for (int i = 0; i < _glyphs.Length; i++)

            {
                isLast = i == _glyphs.Length - 1;
                nextChar = isLast ? null : _glyphs[i + 1].Character;
                advance = (isLast ? _glyphs[i].CursorBounds.Width : _glyphs[i].GetAdvance(nextChar)) * _scale.X;

                _drawCallBuffer[i].Scale = _scale;
                _drawCallBuffer[i].Position = new Vector2(
                    (float)(_position.X + cursorTravel + _glyphs[i].CursorBounds.X),
                    (float)(_position.Y + _glyphs[i].CursorBounds.Y * _scale.Y));

                cursorTravel += advance;
            }

            _size = new Vector2((float)cursorTravel, _font.ActualHeight * _scale.Y);

            switch (_horiAlignment)
            {
                case HorizontalAlignment.Left: break;
                case HorizontalAlignment.Center: offset.X -= _size.X * 0.5f; break;
                case HorizontalAlignment.Right: offset.X -= _size.X; break;
            }
            switch (_vertAlignment)
            {
                case VerticalAlignment.Top: break;
                case VerticalAlignment.Middle: offset.Y -= _size.Y * 0.5f; break;
                case VerticalAlignment.Base: offset.Y -= _scale.Y * _font.ActualBaseLine; break;
                case VerticalAlignment.Bottom: offset.Y -= _size.Y; break;
            }

            for (int i = 0; i < _drawCallBuffer.Length; i++)
            {
                _drawCallBuffer[i].Position = new Vector2(
                    _drawCallBuffer[i].Position.X + offset.X,
                    _drawCallBuffer[i].Position.Y + offset.Y);
            }
        }

        public void Update(string text = null, Vector2? position = null, Vector2? scale = null, HorizontalAlignment? horiAlignment = null, VerticalAlignment? vertAlignment = null)
        {
            Vector2? clampedScale = scale is null ? null : Vector2.Clamp(scale.Value, Vector2.Zero, new Vector2(float.MaxValue));

            bool bigChange = text is not null && _text != text;
            if (bigChange)
            {
                _text = text ?? _text;
                _position = position ?? _position;
                _scale = clampedScale ?? _scale;
                _horiAlignment = horiAlignment ?? _horiAlignment;
                _vertAlignment = vertAlignment ?? _vertAlignment;
                InitializeArrays();
                return;
            }
            bool smallChange =
                (position is not null && _position != position) ||
                (clampedScale is not null && _scale != clampedScale) ||
                (horiAlignment is not null && _horiAlignment != horiAlignment) ||
                (vertAlignment is not null && _vertAlignment != vertAlignment);
            if (smallChange)
            {
                _position = position ?? _position;
                _scale = clampedScale ?? _scale;
                _horiAlignment = horiAlignment ?? _horiAlignment;
                _vertAlignment = vertAlignment ?? _vertAlignment;
                UpdateDrawCallBuffer();
            }
        }

        public string Text
        {
            get { return _text; }
            set
            {
                if (_text == value) return;
                _text = value;
                InitializeArrays();
            }
        }

        public ShaderFont Font 
        {
            get { return _font; }
            set
            {
                if (_font == value) return;
                _font = value;
                InitializeArrays();
            }
        }

        public Vector2 Position
        {
            get { return _position; }
            set
            {
                if (_position == value) return;
                _position = value;
                UpdateDrawCallBuffer();
            }
        }

        public Vector2 Scale
        {
            get { return _scale; }
            set
            {
                if (value.X < 0) value.X = 0;
                if (value.Y < 0) value.Y = 0;
                if (_scale == value) return;
                _scale = value;
                UpdateDrawCallBuffer();
            }
        }

        public HorizontalAlignment HoriAlignment
        {
            get { return _horiAlignment; }
            set
            {
                if (_horiAlignment == value) return;
                _horiAlignment = value;
                UpdateDrawCallBuffer();
            }
        }

        public VerticalAlignment VertAlignment
        {
            get { return _vertAlignment; }
            set
            {
                if (_vertAlignment == value) return;
                _vertAlignment = value;
                UpdateDrawCallBuffer();
            }
        }

        public Vector2 Size => _size;
        public GlyphDrawCall[] DrawCallBuffer => _drawCallBuffer;
    }
}
