using System;

namespace GameCore {
	public static class MathUtils {
        public static double DegToRad(double deg) => deg * Math.PI / 180.0;
        public static double RadToDeg(double rad) => rad * 180.0 / Math.PI;

        public static float   Lerp(float a, float b, float x)       => a + (b - a) * x;
        public static double  Lerp(double a, double b, double x)    => a + (b - a) * x;
        public static decimal Lerp(decimal a, decimal b, decimal x) => a + (b - a) * x;

        public static byte    Clamp(byte value, byte min, byte max)          => Math.Max(min, Math.Min(value, max));
        public static short   Clamp(short value, short min, short max)       => Math.Max(min, Math.Min(value, max));
        public static int     Clamp(int value, int min, int max)             => Math.Max(min, Math.Min(value, max));
        public static long    Clamp(long value, long min, long max)          => Math.Max(min, Math.Min(value, max));
        public static decimal Clamp(decimal value, decimal min, decimal max) => Math.Max(min, Math.Min(value, max));
        public static float   Clamp(float value, float min, float max)       => Math.Max(min, Math.Min(value, max));
        public static double  Clamp(double value, double min, double max)    => Math.Max(min, Math.Min(value, max));
    }
}
