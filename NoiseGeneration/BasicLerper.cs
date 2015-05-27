using System;
namespace NoiseGeneration
{
	public class BasicLerper : ILerper
	{
		public float Lerp (float t, float a, float b)
		{
			if (t < 0f || t > 1f)
				throw new ArgumentException ("The argument 't' should be in the interval [0, 1]");
			return ((1 - t) * a) + (t * b);
		}
	}
}

