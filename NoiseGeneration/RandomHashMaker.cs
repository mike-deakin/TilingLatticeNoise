using System;
using System.Collections.Generic;

namespace NoiseGeneration
{
	public class RandomHashMaker
	{
		int m_seed_size; //129 is a good number to use. End-user may want to change it (lower saves memory & time, higher allows high resolutions.

		//TODO: Decouple this with NoiseTile.
		Dictionary<int[], NoiseTile> m_tiles;
		Random m_rng; //C# Pseudo random number generator. May replace with the Unity prng for that release.
		int m_dim;
		
		public RandomHashMaker (Dictionary<int[], NoiseTile> tiles, int dimensions, int seed_size)
		{
			m_tiles = tiles;
			m_dim = dimensions;
			m_seed_size = seed_size;
		}

		public void GenerateHash(params int[] tile_coord){
			NoiseTile tile;
			int[] coord = tile_coord;
			if(!m_tiles.TryGetValue(coord, out tile)){
				//If there is no tile there, currently throw an exception. I want to create one instead though, but this is not simple.
				//TODO: Instead throwing an exception, create the tile instead.
				throw new ArgumentNullException("tile_coord",
				                                "The given coordinate did not contain a NoiseTile object to write to." +
				                                "Please create one here first.");
			}

			m_rng = new Random (System.DateTime.Now.Millisecond); //Currently setting a seed by time to give random looking results.
																  //End-user may want to set a seed by some other means for reapeatable results.
			int[] seed = new int[m_seed_size];

			{ //initialise the seed array
				int i, j, k; //temp vars.
				for (i = 0; i < m_seed_size; i++) { //fill first half of seed[] with numbers 0 to (m_seed_size - 1).
					seed [i] = i;
				}

				while (--i > 0){ //scramble the values (these becomes the seed values)
					j = seed[i];
					k = m_rng.Next(0, m_seed_size);
					seed[i] = seed[k];
					seed[k] = j;
				}
			}

			{ //"bake" the hash values for each coordinate
				//This acts as a for loop over all coordinates of the hash array. This only works with NDArrayRegular at the moment.
				//Not sure how to change so works with NDArrayIrregular as well.
				//May just write methods in NDArray or something.
				int[] indicies = new int[m_dim];
				int width = tile.hash.SideLength;
				int len = tile.hash.Size;
				int val = 0;
				
				for (int m = 0; m < m_dim; m++)
					indicies [m] = 0;

				for (int i = 0; i < len; i++) { //For each element in the array...
					val = ReentrantHash (indicies, seed);
					tile.hash.SetAt (val, indicies);
					for (int j = 0; j < m_dim; j++) { //... change the indicies array to reflect the next coordinate 
						if (++indicies [j] == width) //indicies[j] == width would mean we've gone past the last index in that direction
							indicies [j] = 0;
						else
							break;
					}
				}
			}

			{ //Alter the hash by matching the baked values at the boundaries with neighboring tile.
				int num_neighbors = 2 * m_dim; //There are 2 neighboring tiles (potentially) in each direction.
				int[] neighbor_coord;
				NoiseTile neighbor;

				for (int i = 0; i < m_dim; i++){ //In every dimension...
					neighbor_coord = coord;
					neighbor_coord[i]++; //'next' tile in current direction

					if (m_tiles.TryGetValue(neighbor_coord, out neighbor)){
						int[] neighbor_hash = neighbor.hash.Flat;
						int[] this_hash = tile.hash.Flat;
						int hash_width = tile.hash.SideLength;
						int hash_size = this_hash.Length;
						int hash_pow = (int)Math.Pow(hash_width, i);

						for (int j = 0; j < hash_size; j++){
							//Can I write comments in LaTeX?
							//If this index is of a point at the boundary, copy the baked hash value accross.
							//at x_n = 0, (j + side_length^n) % side_length^(n+1) < side_length^n
							if((j + hash_pow) % (hash_pow * hash_width) < hash_pow){
								this_hash[j] = neighbor_hash[j - ((hash_width - 1) * hash_pow)];
							}
						}
					}

					neighbor_coord[i] -= 2; //'previous' tile in current direction.

					if (m_tiles.TryGetValue(neighbor_coord, out neighbor)){
						int[] neighbor_hash = neighbor.hash.Flat;
						int[] this_hash = tile.hash.Flat;
						int hash_width = tile.hash.SideLength;
						int hash_size = this_hash.Length;
						int hash_pow = (int)Math.Pow(hash_width, i);
						
						for (int j = 0; j < (hash_size - hash_pow); j++){
							//Can I write comments in LaTeX?
							//If this index is of a point at the boundary, copy the baked hash value accross.
							//at x_n = 0, j % side_length^(n+1) < side_length^n
							if(j % (hash_width * hash_pow) < hash_pow){
								this_hash[j] = neighbor_hash[j + ((hash_width - 1) * hash_pow)];
							}
						}
					}
					//Reset the coordinate
					neighbor_coord[i]++;
				}
			}/**/
		}

		//Perlin's Reentrant hash.
		private int ReentrantHash(int[] coord, int[] seed){
			int len = coord.Length;

			int result = seed[coord[len-1]];
			for (int i = len - 2; i >=0; i--) {
				result = seed[(coord[i] + result) % m_seed_size];
			}

//			int result = 0;
//			for (int i = 0; i < len; i++) {
//				result = seed[(coord[i] + result) % m_seed_size];
//			}

			return result;
		}
	}
}

