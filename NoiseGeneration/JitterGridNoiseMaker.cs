using System;
using System.Collections.Generic;
namespace NoiseGeneration
{
	public class JitterGridNoiseMaker : INoiseMaker
	{
		Dictionary<int[], ScatterTile> m_tiles;
		int m_dim;
		int m_grid_width;
		
		Random m_rng;
		
		ScatterTile m_curr_tile;

		int m_points; //number of points = m_grid_width ^ m_dimensions

		public JitterGridNoiseMaker (Dictionary<int[], ScatterTile> tiles, int dimensions, int grid_width, int seed)
		{
			m_tiles = tiles;
			m_dim = dimensions;
			m_grid_width = grid_width;

			m_points = 1;
			for (int i = 0; i < m_dim; i ++) {
				m_points *= m_grid_width;
			}

			m_rng = new Random (seed);
		}

		public void GenerateNoise (params int[] tile_coord)
		{
			if (!m_tiles.TryGetValue (tile_coord, out m_curr_tile)) {
				//If there is no tile there, currently throw an exception. I want to create one instead though, but this is not simple.
				//TODO: Instead of throwing an exception, create the tile.
				throw new ArgumentNullException ("tile_coord",
				                                 "The given coordinate did not contain a ScatterTile object to write to." +
				                                 "Please create one here first.");
			} else {
				int[] indicies = new int[m_dim];
				double[] point = new double[m_dim];
				
				for (int m = 0; m < m_dim; m++) //First index = [0, 0, ...]
					indicies [m] = 0;
				
				for (int i = 0; i < m_points; i++) { 										//For each element in the array...
					for (int p = 0; p < m_dim; p++){										// ... add a point in the cell...
						point[p] = indicies[p] + m_rng.NextDouble();
					}

					for (int j = 0; j < m_dim; j++) { 										//... change the indicies array to reflect the next coordinate 
						if (++indicies [j] == m_grid_width)									//indicies[j] == m_res would mean we've gone past the last index in that direction
							indicies [j] = 0;
						else
							break;
					}
				}
			}
		}
	}
}

