using System;
using NDimensionalArray;
using System.Collections.Generic;

namespace NoiseGeneration
{
	public class PerlinNoiseMaker : INoiseMaker
	{
		
		//Delegate classes. (I don't want to confuse matters with C# delegates.)
		IFader m_fader;
		ILerper m_lerper;
		IProximity m_prox;

		//Tile information
		Dictionary<int[], NoiseTile> m_tiles;
		int m_dim;
		int m_res;
		int m_seed_size;

		//vars for fractal noise
		int m_oct_min; //Octave to start at.
		int m_oct_num; //Total octaves to compute over.
		float m_persistence;

		int m_perms;

		NoiseTile m_curr_tile;
		
		public PerlinNoiseMaker (IFader fader, ILerper lerper, IProximity prox,
		                         Dictionary<int[], NoiseTile> tiles, int dimension, int resolution, int seed_size,
		                         int min_octave, int num_octave, float persistence)
		{
			m_fader = fader;
			m_lerper = lerper;
			m_prox = prox;

			m_tiles = tiles;
			m_dim = dimension;
			m_res = resolution;
			m_seed_size = seed_size;

			m_oct_min = min_octave;
			m_oct_num = num_octave;
			m_persistence = persistence;

			//m_perms = 2^m_dim
			m_perms = 2;
			for (int a = 1; a < m_dim; a++) {
				m_perms *= 2;
			}
		}

		public void GenerateNoise (params int[] tile_coord){
			int[] coord = tile_coord;
			if (!m_tiles.TryGetValue (coord, out m_curr_tile)) {
				//If there is no tile there, currently throw an exception. I want to create one instead though, but this is not simple.
				//TODO: Instead of throwing an exception, create the tile.
				throw new ArgumentNullException ("tile_coord",
				                                "The given coordinate did not contain a NoiseTile object to write to." +
												"Please create one here first.");
			} else {
				int[] indicies = new int[m_dim];
				int len = m_curr_tile.data.Size;
				
				for (int m = 0; m < m_dim; m++) //First index = [0, 0, ...]
					indicies [m] = 0;
				
				for (int i = 0; i < len; i++) { 											//For each element in the array...
					m_curr_tile.data.SetAt((int)(FractalNoise(indicies)*255), indicies); 	// ... calculate noise...
					for (int j = 0; j < m_dim; j++) { 										//... change the indicies array to reflect the next coordinate 
						if (++indicies [j] == m_res) 										//indicies[j] == m_res would mean we've gone past the last index in that direction
							indicies [j] = 0;
						else
							break;
					}
				}
			}
		}

		private float Noise(int scale, params int[] coordinate)
		{
			int[] ints_lo = new int[m_dim]; //integer parts
			int[] ints_hi = new int[m_dim]; //(hash grid coordinates)
			float[] flts_lo = new float[m_dim]; //fractional parts
			float[] flts_hi = new float[m_dim]; //(inner, local coordinates)
			float[] fades = new float[m_dim]; //fractional parts with fade func applied.

			//scaling vars
			int imgMod = (m_res / scale);
			int hashMod = ((m_seed_size - 1) / scale);

			//populate the cell data
			for (int x = 0; x < m_dim; x++) {
				int tmp = (coordinate[x] / imgMod); //integer div does floor naturally

				flts_lo[x] = ((float)coordinate[x] / imgMod) - tmp;
				ints_lo[x] = (tmp * hashMod);
				flts_hi[x] = flts_lo[x] - 1f;
				ints_hi[x] = (ints_lo[x] + hashMod);

				fades[x] = m_fader.Fade(flts_lo[x]);
			}

			//Now to calculate proximity values and lerp recursively...
			float[] proximities = new float[m_perms];

			int[] curr_ints = new int[m_dim];
			float[] curr_flts = new float[m_dim];

			for (int j = 0; j < m_perms; j++) {
				//This method neat, but probably slower and more memory intensive than need be by a large margin.
				//Selects every permutation of picking from _low or _high in every dimension (2^dim of them)
				//eg in 3d, selects indicies from(where 0 indicates _low, 1 indicates _high):
				//	{0,0,0}
				//	{0,0,1}
				//	{0,1,0}
				//	{0,1,1}
				//	{1,0,0}
				//	{1,0,1}
				//  {1,1,0}
				//	{1,1,1}
				// This is nice because it is scalable to any dimension, but it is slow.
//				curr_ints = SelectByPermIndicies<int>(j, ints_lo, ints_hi);
//				curr_flts = SelectByPermIndicies<float>(j, flts_lo, flts_hi);

				int temp = j;

				for (int m = 0; m < m_dim; m++){
					if ((temp & 1) == 0){
						curr_ints[m] = ints_lo[m];
						curr_flts[m] = flts_lo[m];
					} else {
						curr_ints[m] = ints_hi[m];
						curr_flts[m] = flts_hi[m];
					}
					temp >>= 1;
				}
				
				proximities[j] = m_prox.Proximity(m_curr_tile.hash.GetAt(curr_ints), curr_flts);
			}

			return RecursiveLerp (proximities, fades);
		}
		
		private float FractalNoise(params int[] coordinate)
		{
			float sum = 0f;
			int freq = m_oct_min;
			float amplitude = 1;
			float totalAmp = 0;

			for (int i = 0; i < m_oct_num; i++) {
				sum += Noise(freq, coordinate) * amplitude;
				totalAmp += amplitude;
				amplitude *= m_persistence;
				freq *= 2;
			}

			return ((sum / totalAmp) + 1) / 2;
		}

		//Fairly simple implementation.
		//The binary form of t is already the t-th permutation of 0/1 indicies!
		//Thus, these permutations will also (naturally) be in lexicographical order!
		//If there's a 1 in the i-th bit of t, then select from arr_b
		private T[] SelectByPermIndicies<T>(int t, T[] arr_a, T[] arr_b)
		{
			//TODO: throw/catch some exceptions here? |arr_a| == |arr_b|
			int temp = t;
			int size = arr_a.Length;
			T[] result = new T[size];

			for (int i = 0; i < size; i++) {
				result[i] = ((temp & 1) != 0) ? arr_b[i] : arr_a[i];
				temp >>= 1;
			}
			return result;
		}

		//Lerp between the calculated proximities.
		private float RecursiveLerp(float[] prox, float[] fades)
		{
			{
				int pow2checker = prox.Length;
				while (pow2checker > 1) {
					if (pow2checker % 2 != 0)
						throw new ArgumentException ("Proximities must be a power of 2 in length!");
					pow2checker >>= 1;
				}
			}

			float[] temp;
			int iter = 0;
			while (prox.Length > 1) {
				temp = new float[prox.Length/2];
				for(int i = 0; i < temp.Length; i++){
					temp[i] = m_lerper.Lerp(fades[iter], 
					                        prox[2*i], 
					                        prox[(2*i)+1]);
				}
				iter++;
				prox = temp;
			}
			return prox [0];
		}
	}
}

