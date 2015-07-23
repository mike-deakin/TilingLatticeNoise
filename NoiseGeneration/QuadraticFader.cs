using System;
namespace NoiseGeneration
{
	public class QuadraticFader : IFader
	{
		//Quadratic fader simply returns t^2. Shows grid lines in result.
		public float Fade (float t)
		{
			if (t < 0f || t > 1f)
				throw new ArgumentException ("The argument 't' should be in the interval [0, 1]");
			return t * t;
		}
	}
}

