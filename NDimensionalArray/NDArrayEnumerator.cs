using System;
using IEnumerator = System.Collections.IEnumerator;
using IEnumerable = System.Collections.IEnumerable;

namespace NDimensionalArray
{
	public class NDArrayEnumerator<T> : IEnumerator
	{
		public T[] m_array;
		int position = -1;
		
		public NDArrayEnumerator(T[] array)
		{
			m_array = array;
		}
		
		public bool MoveNext()
		{
			position++;
			return (position < m_array.Length);
		}
		
		public void Reset()
		{
			position = -1;
		}
		
		object IEnumerator.Current {
			get { return Current; }
		}
		
		public T Current {
			get {
				try {
					return m_array [position];
				} catch (IndexOutOfRangeException) {
					throw new InvalidOperationException ();
				}
			}
		}
	}
}

