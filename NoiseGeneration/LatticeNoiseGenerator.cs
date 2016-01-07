using System;
using System.Collections.Generic;
using NDimensionalArray;

namespace NoiseGeneration
{
	public class LatticeNoiseGenerator : INoiseGenerator
	{
		INoiseMaker m_noiseMaker; //Delegate class to generate noise data.
		RandomHashMaker m_hashMaker; //Delegate class to populate the random hash seed and blend edges etc.
		public Dictionary<int[], NoiseTile> m_tiles; //All the tiles generated so far.
		
		NoiseTile curr_tile; //Tile currently being worked on.

		IFader m_fader;
		ILerper m_lerper;
		IProximity m_prox;
		
		//Tile information
		int m_dim;
		int m_resolution;
		int m_hash_size;
		
		//vars for fractal noise
		int m_oct_min;
		int m_oct_num;
		float m_persistence;

		//Constructor without lerper arg. Uses basic lerper (because I cannot see any other lerper implementation being used.
//		public LatticeNoiseGenerator (IFader fader, IProximity proximity,
//		                              int dimension, int resolution, int hash_size, int min_octave, int num_octave, float persistence)
//		{
//			LatticeNoiseGenerator (fader, new BasicLerper (), proximity, dimension, resolution, hash_size, min_octave, num_octave, persistence);
//		}

		public LatticeNoiseGenerator (IFader fader, ILerper lerper, IProximity proximity,
		                              int dimension, int resolution, int hash_power, int min_octave, int num_octave, float persistence)
		{
			m_fader = fader;
			m_lerper = lerper;
			m_prox = proximity;
			m_dim = dimension;
			m_resolution = resolution;
			m_hash_size = (1 << hash_power) + 1; //TODO: I switched to inputting the power. This may need to be cleaned up throughout.
			m_oct_min = min_octave;
			m_oct_num = num_octave;
			m_persistence = persistence;

			m_tiles = new Dictionary<int[], NoiseTile> (new MapEqualityComparer());
			m_noiseMaker = new PerlinNoiseMaker (m_fader, m_lerper, m_prox,
			                                     m_tiles, m_dim, m_resolution, m_hash_size,
			                                     m_oct_min, m_oct_num, m_persistence);
			m_hashMaker = new RandomHashMaker (m_tiles, m_dim, hash_power);
		}

		public void GenerateNoiseTile(params int[] coordinates)
		{
			int[] coord = coordinates;
			curr_tile = new NoiseTile (m_dim, m_resolution, m_hash_size);
			m_tiles.Add (coord, curr_tile);
			m_hashMaker.GenerateHash (coord);
			m_noiseMaker.GenerateNoise (coord);
		}

		public NoiseTile GetTile(params int[] coordinates){
			NoiseTile result;
			if(m_tiles.TryGetValue(coordinates, out result))
				return result;
			else
				throw new Exception("No tile there?");
		}
	}
}

