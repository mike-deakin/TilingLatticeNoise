//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
namespace NoiseGeneration
{
	public class SparseWhiteNoiseMaker : INoiseMaker
	{

		Dictionary<int[], ScatterTile> m_tiles;
		int m_dimensions;
		int m_points;

		int m_rng_max = 256;
		Random m_rng;

		ScatterTile m_curr_tile;

		public SparseWhiteNoiseMaker (Dictionary<int[], ScatterTile> tiles, int dimensions, int points, int seed, int rng_max = 256)
		{
			m_tiles = tiles;
			m_dimensions = dimensions;
			m_points = points;

			m_rng_max = rng_max;
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
				int[] point = new int[m_dimensions];
				List<int[]> tile_points = m_curr_tile.m_points;
				for (int i = 0; i < m_points; i++){
					for (int j = 0; j < m_dimensions; j++){
						point[j] = m_rng.Next(m_rng_max);
					}
					tile_points.Add(point); //Would it be quicker to save the points locally then copy with AddRange? Probably not a significant enough difference either way.
				}
			}
		}
	}
}

