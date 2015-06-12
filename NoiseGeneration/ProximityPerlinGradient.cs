using System;
namespace NoiseGeneration
{
	public class ProximityPerlinGradient : IProximity
	{
		public float Proximity(int hash, params float[] coordinates){
			//My own simple, n-dimensional version.
			//Easily extends to n dimensions, but not the same as Perlin's alg.
			//Uses different unit vectors, so may produce defects?

			//Uses the values of the n least significant bits to add or subtract the coordinate val.
			//Equiv to the dot prod of inner loc vector and a selected vector form n-cube centre to a vertex.
			//This is where it differs, as Perlin uses vectors to the edges.
			float result = 0f;
			int len = coordinates.Length;

			//Trying this out. Get a bit loc to ommit. This shifts vectors from pointing to verticies to edges.
			int blocker = (len != 1) ? (hash >> len) % len : 1 ; //Only ommit a bit if we are working in 2 or more dimensions.

			for (int i = 0; i < len; i++) {
				if (i != blocker){ //If this bit isn't to be ommited...
					if ((hash & 1) == 0) //Check the least sig bit and add or sub based on value.
						result += coordinates[i];
					else
						result -= coordinates[i];
				}
				hash >>= 1; //Right shift to get the next bit.
			}
			return (result);

			//After some pen-and-paper analysis, this gives more even results than Perlin's implementation!
			//Equal probability of dotting with each vector.
			//While this is nicer, it may not result in any improvement in visual quality.
			//Hand verified for 2 and 3 dimensions only. (2/(dim*4) chance for each vector. i.e. 2/8 for 2d, 2/12 for 3d)
		}
	}
}

