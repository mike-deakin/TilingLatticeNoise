using System;
using System.Collections.Generic;

namespace NoiseGeneration
{
	public class RandomHashMaker
	{
		int m_temp_sleft;
		int m_temp_sright;
		int m_seed_pow; //TODO: Use this instead of m_seed_size as input.
		//m_seed_size = (2^m_seed_pow)+1
		int m_seed_size; //129 is a good number to use. End-user may want to change it (lower saves memory & time, higher allows high resolutions.

		//TODO: Decouple this with NoiseTile.
		Dictionary<int[], NoiseTile> m_tiles;
		Random m_rng; //C# Pseudo random number generator. May replace with the Unity prng for that release.
		int m_dim;
		
		public RandomHashMaker (Dictionary<int[], NoiseTile> tiles, int dimensions, int seed_pow)
		{
			m_tiles = tiles;
			m_dim = dimensions;
			m_seed_pow = seed_pow;
			m_seed_size = (1 << seed_pow) + 1; //(2^seed_pow) + 1
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

			m_temp_sleft = m_rng.Next (0, 512);
			m_temp_sright = m_rng.Next (0, 512);

			{ //initialise the seed array
				int i, j, k; //temp vars.
				for (i = 0; i < m_seed_size; i++) { //fill seed[] with numbers 0 to (m_seed_size - 1).
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

//			int result = seed[coord[len-1]];
//			for (int i = len - 2; i >=0; i--) {
//				result = seed[(coord[i] + result) % m_seed_size];
//			}
//
			int result = m_rng.Next (0, m_seed_size); //This appears to be fine. sooooo hashing is not needed?
			//My guess is that hashing was a space saving tool. Perlin's hash is clever but based on
			//a predetermined (hard-coded) seed. My solution naively bakes the hash results to a larger array.
			//The hash then is redundant if any suitably independent rng/hash can produce the baked values.
			//Then the question becomes can I save memory and keep determinism at tile borders?
			//Can I expand my rng somehow?
			//Currently, in effect, a random number is generated and then assigned to each pixel.
			//Is there an alternative, such that {seed, x, y, z, ...} -> [0, 255] and (seed, x + 1, y, z,...) cannot be "predicted"?
			//(To use the fuzzy crypto terminology)
			//If this can be done (and I suspect it can), the only difficulty left is to handle the changing seed at the border.

			//Another not on Perlin's implementation: Part of it's nature is that the seed contains each value once.
			//This might (?) reduce the probability of visual defects but reduces the number of unique tiles.
			//Not really a big deal imo, benefits outweigh this a lot, but it's worth noting since people are
			//implementing noise with longer periodicity.

			//A (dumb?) suggestion: seed rng with seed. cycle x times, then y more times, then z, etc.?
			//That's slow. At worst, n^imgSize!!!
			//A simple block cypher might do the trick. eg a Feistel or Lai-Massey. doesn't have to be crypto secure.
			//Have n rounds (where n is num of dimensions) with keys k1=x, k2=y etc.
			//The "message" can be a random seed.

			//TESTING Feistel generation. If this works, then don't need to bake.
			//int result = FeistelNHash(m_temp_sleft, m_temp_sright, coord)[0];

			return result;
		}

		private int[] FeistelNHash(int sleft, int sright, int[] coord){
			//Experiment:
			//Using a feistel network to hash the coordinates with a seed that is set with each tile.
			//Let seedL and seedR be  the "Plaintext" and the coordinates be the "round keys"

			int rounds = coord.Length;
			int L = sleft;
			int R = sright;

			for (int i = 0; i < rounds; i++) {
				int temp = R;
				R = L ^ FeistelRound (R, coord [i]);
				L = temp;
			}

			int[] result = new int[2];
			result[0] = L;
			result[1] = R;

			return result;
		}

		private int FeistelRound(int right, int k){
			//Round function to acompany the fiestel hash.
			int kexp = KeyExpand(k); //First, expand k to n+1 bits.

			return kexp ^ right; //Testing a very basic round method. Not expecting good results. Update: Did not get good results.
		}

		private int KeyExpand(int k){
			return (int)((((k * 25214903917) + 11) % (2 << 48)) >> 16) % (2 << (m_seed_pow + 1)); //Java linear congruential generator numbers then take mod 2^(m_seed_pow+1)
		}
	}
}

