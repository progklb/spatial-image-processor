using UnityEngine;
using System.Collections;

/// <summary>
/// Provides useful math utility functions.
//// </summary>
public static class MathUtils 
{
	/// <summary>
	/// Vector-specific functions.
	/// </summary>
	public static class VectorUtils
	{
		/// <summary>
		/// Returns true if the two vectors are within absolute difference of one another. The max difference is indicated by the tolerance parameter.
		/// </summary>
		/// <param name="tolerance">The max allowable difference between the two Vectors</param>
		public static bool Approx(Vector3 a, Vector3 b, float tolerance = 0.1f)
		{
			return Mathf.Abs(a.x - b.x) < tolerance && Mathf.Abs(a.y - b.y) < tolerance && Mathf.Abs(a.z - b.z) < tolerance;
		}

		/// <summary>
		/// Returns true if the first parameter is smaller than or equal to the second parameter.
		/// </summary>
		public static bool SmallThanEqualTo(Vector3 a, Vector3 b)
		{
			return a.x <= b.x && a.y <= b.y && a.z <= b.z;
		}

		/// <summary>
		/// Returns true if the first parameter is greater than or equal to the second parameter.
		/// </summary>
		public static bool GreatThanEqualTo(Vector3 a, Vector3 b)
		{
			return a.x >= b.x && a.y >= b.y && a.z >= b.z;
		}
	}
}
