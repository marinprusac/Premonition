using System;
using UnityEngine;

namespace Grid
{
    public readonly struct HexCoordinates : IEquatable<HexCoordinates>
    {
        public readonly int q, r, s, h;
        
        public HexCoordinates(int q, int r, int s, int h=0)
        {
            this.q = q;
            this.r = r;
            this.s = s;
            this.h = h;
        }
    
    
        public static HexCoordinates operator+(HexCoordinates first, HexCoordinates second)
        {
            return new HexCoordinates(first.q + second.q, first.r + second.r, first.s + second.s, first.h + second.h);
        }
    
        public static HexCoordinates operator-(HexCoordinates first, HexCoordinates second)
        {
            return new HexCoordinates(first.q - second.q, first.r - second.r, first.s - second.s, first.h - second.h);
        }
    
        public static HexCoordinates operator*(int scalar, HexCoordinates coords)
        {
            return new HexCoordinates(coords.q * scalar, coords.r * scalar, coords.s * scalar, coords.h * scalar);
        }
    
    
        public Vector3 ToPixelCoordinates(GridSettings settings)
        {
            var x = settings.tileRadius * (Mathf.Sqrt(3f) * q + Mathf.Sqrt(3f) / 2f * r);
            var z = settings.tileRadius * (3.0f / 2.0f * r);
            return new Vector3(x, h * settings.tileHeight, z);
        }
    
    
        public static HexCoordinates FromPixelCoordinates(Vector3 worldPos, GridSettings settings)
        {
            float x = worldPos.x;
            float y = worldPos.y;
            float z = worldPos.z;

            float q = (Mathf.Sqrt(3f) / 3f * x - 1f / 3f * z) / settings.tileRadius;
            float r = (2f / 3f * z) / settings.tileRadius;
            float s = -q - r;
            float h = y / settings.tileHeight;
        
            return SnapHexCoords(new Vector4(q, r, s, h));
        }
    
    
        private static HexCoordinates SnapHexCoords(Vector4 cube)
        {
            var qRounded = Mathf.RoundToInt(cube.x);
            var rRounded = Mathf.RoundToInt(cube.y);
            var sRounded = Mathf.RoundToInt(cube.z);
            var hRounded = Mathf.RoundToInt(cube.w);

            var qDiff = Mathf.Abs(qRounded - cube.x);
            var rDiff = Mathf.Abs(rRounded - cube.y);
            var sDiff = Mathf.Abs(sRounded - cube.z);

            if (qDiff > rDiff && qDiff > sDiff)
            {
                qRounded = -rRounded - sRounded;
            }
            else if (rDiff > sDiff)
            {
                rRounded = -qRounded - sRounded;
            }
            else
            {
                sRounded = -qRounded - rRounded;
            }

            return new HexCoordinates(qRounded, rRounded, sRounded, hRounded);
        }
    
    
        public static HexCoordinates Up => new HexCoordinates(0, 0, 0, 1);
        public static HexCoordinates Down => new HexCoordinates(0, 0, 0, -1);
    
        public static HexCoordinates Zero => new HexCoordinates(0, 0, 0);
    
        public static HexCoordinates Left => new HexCoordinates(-1, 0, 1);
        public static HexCoordinates Right => new HexCoordinates(1, 0, -1);
        public static HexCoordinates ForwardLeft => new HexCoordinates(0, -1, 1);
        public static HexCoordinates ForwardRight => new HexCoordinates(1, -1, 0);
        public static HexCoordinates BackLeft => new HexCoordinates(-1, 1, 0);
        public static HexCoordinates BackRight => new HexCoordinates(0, 1, -1);


        public static int Distance(HexCoordinates a, HexCoordinates b)
        {
            return (Mathf.Abs(a.q - b.q) + Mathf.Abs(a.r - b.r) + Mathf.Abs(a.s - b.s)) / 2;
        }
    
        public bool Equals(HexCoordinates other)
        {
            return q == other.q && r == other.r && s == other.s && h == other.h;
        }

        public override bool Equals(object obj)
        {
            return obj is HexCoordinates other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(q, r, s, h);
        }

        public override string ToString()
        {
            return $"HexCoordinates(q: {q}, r: {r}, s: {s}, h: {h})";
        }
    
        public bool Equals2D(HexCoordinates other)
        {
            return q == other.q && r == other.r && s == other.s;
        }
    }
}