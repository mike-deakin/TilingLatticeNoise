using System;
using System.Collections.Generic;


namespace NoiseGeneration
{
	public class BlueScatterNoiseGenerator : INoiseGenerator //I might be breaking some contracts here.
															 //Might make intermediary iterfaces IScatterNoiseGenerator & ILatticeNoiseGenerator
	{
		INoiseMaker m_noiseMaker; //Delegate class to generate noise data.
		RandomHashMaker m_hashMaker; //Delegate class to populate the random hash seed and blend edges etc.
		Dictionary<int[], ScatterTile> m_tiles; //Is a multi-dim key needed (like with lattice noise)? There's not going to be tiling going on yet.
											   //Perhaps there will with a simplex implementation.
		ScatterTile curr_tile;
		
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

		public BlueScatterNoiseGenerator (IFader fader, IProximity proximity,
		                                  int dimension, int resolution, int hash_size, int min_octave, int num_octave, float persistence)
		{
			m_fader = fader;
			m_lerper = new BasicLerper ();
			m_prox = proximity;
			m_dim = dimension;
			m_resolution = resolution;
			m_hash_size = hash_size;
			m_oct_min = min_octave;
			m_oct_num = num_octave;
			m_persistence = persistence;

			m_tiles = new Dictionary<int[], ScatterTile> (new MapEqualityComparer ());
			m_noiseMaker = new BlueScatterNoiseMaker (m_fader, m_lerper, m_prox,
			                                     m_tiles, m_dim, m_resolution, m_hash_size,
			                                     m_oct_min, m_oct_num, m_persistence);
			m_hashMaker = new RandomHashMaker (m_tiles, m_dim, m_hash_size);
		}

		public void GenerateNoiseTile(params int[] coordinates)
		{
			curr_tile = new ScatterTile (m_dim, m_hash_size);
			m_tiles.Add (curr_tile);
			m_hashMaker.GenerateHash (coordinates);
			m_noiseMaker.GenerateNoise (coordinates);
		}
	}
}

