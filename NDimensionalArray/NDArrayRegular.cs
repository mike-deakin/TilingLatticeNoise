using System;
using IEnumerator = System.Collections.IEnumerator;
using IEnumerable = System.Collections.IEnumerable;

namespace NDimensionalArray
{
	//<summary>Regular N-Dimensional array. The length of each side is equal.
	//Geometrically, this is shaped like a hypercube/n-cube.</summary>
	public class NDArrayRegular<T> : INDArray<T>
	{
		private T[] m_array = null;
		private int m_size = -1;
		private int m_side_length = -1;
		private int m_dimension = -1;

		//<summary> Equilateral array constructor. Same size in all (orthogonal) directions. </summary>
		//<param name="dim">Number of dimensions of array. Must be greater than 0.<param>
		//<param name="side_length">Size of array in each dimension. Must be greater than 0.<param>
		public NDArrayRegular (int dim, int side_length)
		{
			m_dimension = dim;
			m_side_length = side_length;
			m_size = (int)Math.Pow (side_length, dim);
			m_array = new T[m_size];
		}

		public int Size {
			get { return m_size; }
		}

		public dynamic SideLength {
			get { return m_side_length; }
		}

		public int Dimensions {
			get { return m_dimension; }
		}

		public T[] Flat {
			get { return m_array; }
		}

		public T this [params int[] coord] {
			get { return GetAt (coord); }
			set { SetAt (value, coord); }
		}

		//Return array value at selected coordinate. Zero indexed.
		public T GetAt(params int[] coordinates)
		{
			if (m_dimension != coordinates.Length)
				throw new ArgumentException ("Must pass as many coordinates as there are dimensions of the array.\n" +
				                             "Number of coordinates was not the same as number of array dimensions.");
			foreach (int x in coordinates)
				if (x > m_side_length)
					throw new IndexOutOfRangeException ();

			int linear_loc = coordinates[0];
			int dist = m_side_length;
			for (int i = 1; i < m_dimension; i++) {
				linear_loc += (coordinates[i]) * dist;
				dist *= m_side_length;
			}

			return m_array[linear_loc];
		}

		//Set array value at selected coordinate. Zero indexed.
		public void SetAt(T value, params int[] coordinates)
		{
			if (m_dimension != coordinates.Length)
				throw new ArgumentException ("Must pass as many coordinates as there are dimensions of the array.\n" +
				                             "Number of coordinates was not the same as number of array dimensions.");
			foreach (int x in coordinates)
				if (x > m_side_length)
					throw new IndexOutOfRangeException ();

			int linear_loc = coordinates[0];
			int dist = m_side_length;
			for (int i = 1; i < coordinates.Length; i++) {
				linear_loc += (coordinates[i]) * dist;
				dist *= m_side_length;
			}

			m_array [linear_loc] = value;
		}

		//Idea to extract a coordinate hypersurface but I think it is slow as hell.
//		public NDArrayRegular<T> GetHypersurface(int coordinate, int value)
//		{
//			//Create a coordinate hypersurface by fixing the coordinate at value and varying all other coordinates.
//			NDArrayRegular<T> result = new NDArrayRegular<T> (m_dimension - 1, m_side_length);
//
//			int n = m_dimension;
//			int[] indicies = new int[n];
//			int width = m_side_length;
//			int len = m_size;
//			
//			for (int i = 0; i < len; i++) { //For each element in the array...
//				for (int j = 0; j < n; j++) { //... change the indicies array to reflect the next coordinate
//					if (j == coordinate)
//						indicies[j] = value;
//					else if (++indicies [j] == width) //indicies[j] == width would mean we've gone past the last index in that direction
//						indicies [j] = 0;
//					else
//						break;
//				}
//				//get and set at that loc.
//			}
//
//			return result;
//		}

		public bool Equals (INDArray<T> other)
		{
			if (other == null)
				return false;
			if (ReferenceEquals (this, other))
				return true;
			if (other.GetType () != typeof(NDArrayRegular<T>))
				return false;
			NDArrayRegular<T> up_cast = (NDArrayRegular<T>)other;
			return m_array == up_cast.m_array && m_size == up_cast.m_size && m_side_length == up_cast.m_side_length && m_dimension == up_cast.m_dimension;
		}

		public override bool Equals (Object obj)
		{
			if (obj == null)
				return false;
			if (ReferenceEquals (this, obj))
				return true;
			if (obj.GetType () != typeof(NDArrayRegular<T>))
				return false;
			NDArrayRegular<T> other = (NDArrayRegular<T>)obj;
			return m_array == other.m_array && m_size == other.m_size && m_side_length == other.m_side_length && m_dimension == other.m_dimension;
		}
		
		public override int GetHashCode ()
		{
			unchecked {
				return (m_array != null ? m_array.GetHashCode () : 0) ^ m_size.GetHashCode () ^ m_side_length.GetHashCode () ^ m_dimension.GetHashCode ();
			}
		}

		public override string ToString ()
		{
			return string.Format ("[NDArray: m_array={0}]", m_array);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return (IEnumerator)GetEnumerator ();
		}

		public NDArrayEnumerator<T> GetEnumerator()
		{
			return new NDArrayEnumerator<T>(m_array);
		}
	}
}

