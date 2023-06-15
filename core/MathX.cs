using System;

namespace GameCore {
	public static class MathX {
		/// <summary>
		/// Generador de números aleatorios
		/// </summary>
		private static readonly Random rng = new Random();

		public static double DegRatio = 180 / Math.PI;

		public static double DegToRad(double deg) => deg / DegRatio;
		public static double RadToDeg(double rad) => rad * DegRatio;

		public static int Round(float value)   => (int)Math.Round(value);
		public static int Round(double value)  => (int)Math.Round(value);
		public static int Round(decimal value) => (int)Math.Round(value);

		public static float   Lerp(float a, float b, float x)       => a + (b - a) * x;
		public static double  Lerp(double a, double b, double x)    => a + (b - a) * x;
		public static decimal Lerp(decimal a, decimal b, decimal x) => a + (b - a) * x;

		public static byte    Clamp(byte value, byte min, byte max)	         => Math.Max(min, Math.Min(value, max));
		public static short   Clamp(short value, short min, short max)	     => Math.Max(min, Math.Min(value, max));
		public static int     Clamp(int value, int min, int max)             => Math.Max(min, Math.Min(value, max));
		public static long    Clamp(long value, long min, long max)          => Math.Max(min, Math.Min(value, max));
		public static float   Clamp(float value, float min, float max)       => Math.Max(min, Math.Min(value, max));
		public static double  Clamp(double value, double min, double max)    => Math.Max(min, Math.Min(value, max));
		public static decimal Clamp(decimal value, decimal min, decimal max) => Math.Max(min, Math.Min(value, max));

		public static int RandInt() {
			return rng.Next();
		}
		public static int RandInt(int maxValue) {
			return rng.Next(maxValue);
		}
		public static int RandInt(int minValue, int maxValue) {
			return rng.Next(minValue, maxValue);
		}
		public static double Rand() {
			return rng.NextDouble();
		}
		public static double Rand(double maxValue) {
			return rng.NextDouble() * maxValue;
		}
		public static double Rand(double minValue, double maxValue) {
			return minValue + rng.NextDouble() * (maxValue - minValue);
		}
		
		public static T Choose<T>(params T[] e) {
			return e[rng.Next(0, e.Length)];
		}
	}
}
