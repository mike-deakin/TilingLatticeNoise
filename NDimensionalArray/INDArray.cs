using System;
using IEnumerable = System.Collections.IEnumerable;

namespace NDimensionalArray
{
	//<summary>Generalised N-Dimensional array.</summary>
	//
	public interface INDArray<T> : IEnumerable, IEquatable<INDArray<T>>
	{
		//<summary>Get method. Returns value in array at index equivilent to the input coordinate.</summary>
		//<param name="coords">N coordinates to the target value in array.</param>
		T GetAt(params int[] coords);

		//<summary>Set method. Sets value in array at index equivilent to the input coordinate.</summary>
		//<param name="value">Value to enter into array.</param>
		//<param name="coords">N coordinates to the target value in array.</param>
		void SetAt(T value, params int[] coords);

		int Size {
			get;
		}
		
		dynamic SideLength {
			get;
		}
		
		int Dimensions {
			get;
		}
		
		 T[] Flat {
			get;
		}
		
		T this [params int[] coord] {
			get;
			set;
		}
	}
}

