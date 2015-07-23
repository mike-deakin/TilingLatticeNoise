using System;
using NDimensionalArray;
using System.Collections.Generic;

namespace NoiseGeneration
{
	public struct ScatterTile
	{
		public List<double[]> m_points; //List of points populated by noise algorithm.
		public int m_seed;
		public int m_dims;

		public ScatterTile(int seed, int dims){
			m_dims = dims;
			m_seed = seed;
			m_points = new List<double[]> ();
		}
	}
}

