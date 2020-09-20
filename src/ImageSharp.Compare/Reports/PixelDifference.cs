using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Numerics;

namespace UkooLabs.ImageSharp.Compare.Reports
{
    public readonly struct PixelDifference
    {
        public PixelDifference(Point position, Rgba64 expected, Rgba64 actual)
        {
            Position = position;
            RedDifference = actual.R - expected.R;
            GreenDifference = actual.G - expected.G;
            BlueDifference = actual.B - expected.B;
            AlphaDifference = actual.A - expected.A;
            var actualLengthSquared = actual.ToVector4().LengthSquared();
            var expectedLengthSquared = expected.ToVector4().LengthSquared();
            Delta = actualLengthSquared < expectedLengthSquared ? 0.0f : 1.0f;
        }

        public Point Position { get; }

        public int RedDifference { get; }

        public int GreenDifference { get; }

        public int BlueDifference { get; }

        public int AlphaDifference { get; }

        public float Delta { get; }

        public override string ToString() => $"[Δ({RedDifference},{GreenDifference},{BlueDifference},{AlphaDifference}) @ ({Position.X},{Position.Y})]";
    }
}
