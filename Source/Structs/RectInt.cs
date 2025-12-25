using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace BlazeEngine
{
    public struct RectInt : IFormattable, IEquatable<RectInt>
    {
        #region CenterDefinition
        // * Represents a center

        // Size == 4
        // # # # #
        // # # * #
        // # # # #
        // # # # #

        // Size == 5
        // # # # # #
        // # # # # #
        // # # * # #
        // # # # # #
        // # # # # #
        #endregion

        private int m_Width;
        private int m_Height;
        private int m_X;
        private int m_Y;

        public int X
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
        public int Y
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

        public int Width
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return m_Width;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                m_Width = Math.Abs(value);
            }
        }
        public int Height
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return m_Height;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                m_Height = Math.Abs(value);
            }
        }

        public Vec2Int Position
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new Vec2Int(X, Y);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }
        public Vec2Int Size
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new Vec2Int(Width, Height);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                Width = value.X;
                Height = value.Y;
            }
        }


        public int MinX
        {
            get
            {
                return GetMin(X, Width);
            }
            set
            {
                int minX = value;
                int maxX = MaxX;

                Width = Math.Abs(maxX - minX) + 1;
                X = GetCenter(minX, Width);
            }
        }
        public int MinY
        {
            get
            {
                return GetMin(Y, Height);
            }
            set
            {
                int minY = value;
                int maxY = MaxY;

                Height = Math.Abs(maxY - minY) + 1;
                Y = GetCenter(minY, Height);
            }
        }
        public int MaxX
        {
            get
            {
                return GetMax(X, Width);
            }
            set
            {
                int minX = MinX;
                int maxX = value;

                Width = Math.Abs(maxX - minX) + 1;
                X = GetCenter(minX, Width);
            }
        }
        public int MaxY
        {
            get
            {
                return GetMax(Y, Height);
            }
            set
            {
                int minY = MinY;
                int maxY = value;

                Height = Math.Abs(maxY - minY) + 1;
                Y = GetCenter(minY, Height);
            }
        }

        public Vec2Int Min
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new Vec2Int(MinX, MinY);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                MinX = value.X;
                MinY = value.Y;
            }
        }
        public Vec2Int Max
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new Vec2Int(MaxX, MaxY);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                MaxX = value.X;
                MaxY = value.Y;
            }
        }

        public Vec2 GetCenter()
        {
            return (Vec2)Min + ((Vec2)Size / 2f);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetMin(int pos, int size)
        {
            return pos - Mathf.FloorToInt(size / 2f);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetMax(int pos, int size)
        {
            pos += Mathf.FloorToInt(size / 2f);
            if (size % 2 == 0)
            {
                pos -= 1;
            }
            return pos;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetCenter(int min, int size)
        {
            return min + Mathf.FloorToInt(size / 2f);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RectInt(int x, int y, int width, int height)
        {
            m_X = x;
            m_Y = y;
            m_Width = width;
            m_Height = height;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RectInt(Vec2Int position, Vec2Int size)
        {
            m_X = position.X;
            m_Y = position.Y;
            m_Width = size.X;
            m_Height = size.Y;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetArea()
        {
            return Width * Height;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vec2Int[] GetAllPositionsWithin()
        {
            if (Width == 0 || Height == 0)
            {
                return Array.Empty<Vec2Int>();
            }

            Vec2Int[] positions = new Vec2Int[GetArea()];

            int i = 0;
            for (int x = MinX; x <= MaxX; x++)
            {
                for (int y = MinY; y <= MaxY; y++)
                {
                    positions[i++] = new Vec2Int(x, y);
                }
            }

            return positions;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Overlaps(RectInt rect)
        {
            return rect.MinX <= MaxX && rect.MaxX >= MinX && rect.MinY <= MaxY && rect.MaxY >= MinY;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(Vec2Int position)
        {
            return position.X >= MinX && position.X <= MaxX && position.Y >= MinY && position.Y <= MaxY;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(RectInt rect)
        {
            return rect.MinX >= MinX && rect.MaxX <= MaxX && rect.MinY >= MinY && rect.MaxY <= MaxY;
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
            if (formatProvider == null)
            {
                formatProvider = CultureInfo.InvariantCulture.NumberFormat;
            }

            return $"(X:{X.ToString(format, formatProvider)}, Y:{Y.ToString(format, formatProvider)}, Width:{Width.ToString(format, formatProvider)}, Height:{Height.ToString(format, formatProvider)}) \n MinX: {MinX.ToString(format, formatProvider)}, MinY: {MinY.ToString(format, formatProvider)}, MaxX: {MaxX.ToString(format, formatProvider)}, MaxY: {MaxY.ToString(format, formatProvider)}";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object other)
        {
            if (!(other is RectInt))
            {
                return false;
            }

            return Equals((RectInt)other);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(RectInt other)
        {
            return other.X == X && other.Y == Y && other.Width == Width && other.Height == Height;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            int hashCode = X.GetHashCode();
            int hashCode2 = Y.GetHashCode();
            int hashCode3 = Width.GetHashCode();
            int hashCode4 = Height.GetHashCode();
            return hashCode ^ (hashCode2 << 4) ^ (hashCode2 >> 28) ^ (hashCode3 >> 4) ^ (hashCode3 << 28) ^ (hashCode4 >> 4) ^ (hashCode4 << 28);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(RectInt lhs, RectInt rhs)
        {
            return !(lhs == rhs);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(RectInt lhs, RectInt rhs)
        {
            return lhs.X == rhs.X && lhs.Y == rhs.Y && lhs.Width == rhs.Width && lhs.Height == rhs.Height;
        }

        public static RectInt Zero
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new RectInt(0, 0, 0, 0);
            }
        }
        public static RectInt One
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new RectInt(0, 0, 1, 1);
            }
        }

        public static RectInt FromBounds(int minX, int minY, int maxX, int maxY)
        {
            return new RectInt
            {
                MinX = minX,
                MinY = minY,
                MaxX = maxX,
                MaxY = maxY
            };
        }
    }
}