using SixLabors.ImageSharp;

namespace MagicMedia
{
    public static class RationalExtensions
    {
        public static double GetValue(this Rational rational)
        {
            return rational.Numerator / rational.Denominator;
        }
    }
}
