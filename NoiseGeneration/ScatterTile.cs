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
using NDimensionalArray;
using System.Collections.Generic;

namespace NoiseGeneration
{
	public struct ScatterTile
	{
		public List<int[]> points; //List of points populated by noise algorithm.
		public NDArrayRegular<int> hash;  //Hash values to seed noise data. Should be of side length 2^n + 1.

		public ScatterTile(int dim, int hash_resolution){
			points = new List<int[]>();
			hash = new NDArrayRegular<int> (dim, hash_resolution);
		}
	}
}

