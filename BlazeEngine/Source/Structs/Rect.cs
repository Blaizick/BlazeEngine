
using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace BlazeEngine;

    [System.Serializable]
    public struct Rect : IFormattable, IEquatable<Rect>
    {
        private float m_X;
        private float m_Y;
        private float m_Width;
        private float m_Height;


        public float X
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return m_X;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                m_X = value;
            }
        }
        public float Y
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return m_Y;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                m_Y = value;
            }
        }
        public float Width
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return m_Width;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                m_Width = value;
            }
        }
        public float Height
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return m_Height;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                m_Height = value;
            }
        }

        public Vec2 Position
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new Vec2(X, Y);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }
        public Vec2 Size
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new Vec2(Width, Height);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                Width = value.X;
                Height = value.Y;
            }
        }


        public float MinX
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return X - (Width / 2f);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                float min = value;
                float max = MaxX;
                Width = max - min;
                X = min + (Width / 2f);
            }
        }
        public float MaxX
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return X + (Width / 2f);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                float min = MinX;
                float max = value;
                Width = max - min;
                X = min + (Width / 2f);
            }
        }
        public float MinY
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return Y - (Height / 2f);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                float min = value;
                float max = MaxY;
                Height = max - min;
                Y = min + (Height / 2f);
            }
        }
        public float MaxY
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return Y + (Height / 2f);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                float min = MinY;
                float max = value;
                Height = max - min;
                Y = min + (Height / 2f);
            }
        }

        public Vec2 Min
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new Vec2(MinX, MinY);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                MinX = value.X;
                MinY = value.Y;
            }
        }
        public Vec2 Max
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new Vec2(MaxX, MaxY);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                MaxX = value.X;
                MaxY = value.Y;
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Rect(float x, float y, float width, float height)
        {
            m_X = x;
            m_Y = y;
            m_Width = width;
            m_Height = height;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Rect(Vec2 position, Vec2 size) : this(position.X, position.Y, size.X, size.Y) { }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Rect(Rect source)
        {
            m_X = source.X;
            m_Y = source.Y;
            m_Width = source.Width;
            m_Height = source.Height;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set(Rect source)
        {
            m_X = source.X;
            m_Y = source.Y;
            m_Width = source.Width;
            m_Height = source.Height;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(Vec2 point)
        {
            return point.X >= MinX && point.X < MaxX && point.Y >= MinY && point.Y < MaxY;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Overlaps(Rect other)
        {
            return other.MaxX > MinX && other.MinX < MaxX && other.MaxY > MinY && other.MinY < MaxY;
        }

        public Vec2 ClampPosition(Vec2 position)
        {
            position.X = Mathf.Clamp(position.X, MinX, MaxX);
            position.Y = Mathf.Clamp(position.Y, MinY, MaxY);
            return position;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Rect lhs, Rect rhs)
        {
            return !(lhs == rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Rect lhs, Rect rhs)
        {
            return lhs.X == rhs.X && lhs.Y == rhs.Y && lhs.Width == rhs.Width && lhs.Height == rhs.Height;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ (Width.GetHashCode() << 2) ^ (Y.GetHashCode() >> 2) ^ (Height.GetHashCode() >> 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Rect other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y) && Width.Equals(other.Width) && Height.Equals(other.Height);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString()
        {
            return ToString(null, null);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ToString(string format)
        {
            return ToString(format, null);
        }
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format))
            {
                format = "F2";
            }

            if (formatProvider == null)
            {
                formatProvider = CultureInfo.InvariantCulture.NumberFormat;
            }

            return $"(X: {X.ToString(format, formatProvider)}, Y: {Y.ToString(format, formatProvider)}, Width: {Width.ToString(format, formatProvider)}, Height: {Height.ToString(format, formatProvider)}\nMinX: {MinX.ToString(format, formatProvider)}, MinY: {MinY.ToString(format, formatProvider)}, MaxX: {MaxX.ToString(format, formatProvider)}, MaxY: {MaxY.ToString(format, formatProvider)})";
        }


        public static Rect Zero
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new Rect(0, 0, 0, 0);
            }
        }

        public static Rect FromBounds(float minX, float minY, float maxX, float maxY)
        {
            return new Rect
            {
                MinX = minX,
                MinY = minY,
                MaxX = maxX,
                MaxY = maxY,
            };
        }
    }
