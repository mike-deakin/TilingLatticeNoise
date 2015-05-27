using System;
using IEnumerator = System.Collections.IEnumerator;
using IEnumerable = System.Collections.IEnumerable;

namespace NDimensionalArray
{
	public class NDArrayIrregular<T> : INDArray<T>
	{
		private T[] m_array = null;
		private int m_size = -1;
		private int[] m_side_lengths = null;
		private int m_dimension = -1;

		public NDArrayIrregular (int dim, params int[] side_lengths)
		{
			if (dim != side_lengths.Length)
				throw new ArgumentException ("Must pass as many side lengths as there are to be dimensions of the array.\n" +
												"Size of side_lengths was not the same as the value of dim.");
			m_dimension = dim;
			m_side_lengths = side_lengths;
			m_size = 1;
			foreach (int l in side_lengths)
				m_size *= l;
			m_array = new T[m_size];
		}

		public int Size {
			get { return m_size; }
		}
		
		public dynamic SideLength {
			get { return m_side_lengths; }
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
		
		public T GetAt(params int[] coordinates)
		{
			if (m_dimension != coordinates.Length)
				throw new ArgumentException ("Must pass as many coordinates as there are dimensions of the array.\n" +
				                             "Size of coordinates was not the same as number of array dimensions.");

			int linear_loc = coordinates[0];

			//This loop looks like it finishes too soon, but it doesn't. Remember zero indexing!
			for (int i = 1; i < m_dimension; i++) {
				linear_loc += coordinates[i] * m_side_lengths[i - 1];
			}
			
			return m_array[linear_loc];
		}

		public void SetAt(T value, params int[] coordinates)
		{
			if (m_dimension != coordinates.Length)
				throw new ArgumentException ("Must pass as many coordinates as there are dimensions of the array.\n" +
				                             "Size of coordinates was not the same as number of array dimensions.");

			int linear_loc = coordinates[0];

			//This loop looks like it finishes too soon, but it doesn't. Remember zero indexing!
			for (int i = 1; i < coordinates.Length; i++) {
				linear_loc += coordinates[i] * m_side_lengths[i - 1];
			}
			
			m_array [linear_loc] = value;
		}
		
		public bool Equals (INDArray<T> other)
		{
			if (other == null)
				return false;
			if (ReferenceEquals (this, other))
				return true;
			if (other.GetType () != typeof(NDArrayIrregular<T>))
				return false;
			NDArrayIrregular<T> up_cast = (NDArrayIrregular<T>)other;
			return m_array == up_cast.m_array && m_size == up_cast.m_size && m_side_lengths == up_cast.m_side_lengths && m_dimension == up_cast.m_dimension;
		}
		
		public override bool Equals (Object obj)
		{
			if (obj == null)
				return false;
			if (ReferenceEquals (this, obj))
				return true;
			if (obj.GetType () != typeof(NDArrayIrregular<T>))
				return false;
			NDArrayIrregular<T> other = (NDArrayIrregular<T>)obj;
			return m_array == other.m_array && m_size == other.m_size && m_side_lengths == other.m_side_lengths && m_dimension == other.m_dimension;
		}
		
		public override int GetHashCode ()
		{
			unchecked {
				return (m_array != null ? m_array.GetHashCode () : 0) ^ m_size.GetHashCode () ^ m_side_lengths.GetHashCode () ^ m_dimension.GetHashCode ();
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

