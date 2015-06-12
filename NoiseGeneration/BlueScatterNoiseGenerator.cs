using System;
using System.Collections.Generic;


namespace NoiseGeneration
{
	public class BlueScatterNoiseGenerator : INoiseGenerator //I might be breaking some contracts here.
															 //Might make intermediary iterfaces IScatterNoiseGenerator & ILatticeNoiseGenerator
	{
		Dictionary<int, ScatterTile> mTiles; //Is a multi-dim key needed (like with lattice noise)? There's not going to be tiling going on yet.
											 //Perhaps there will with a simplex implementation.

		public BlueScatterNoiseGenerator ()
		{
		}

		public void GenerateNoiseTile(params int[] coordinates)
		{

		}
	}
}

