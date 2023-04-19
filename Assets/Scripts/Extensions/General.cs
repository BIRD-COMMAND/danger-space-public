using Freya;
using System;
using UnityEngine;

namespace Extensions
{
	public static class General
	{

		/// <summary>
		/// Equivalent to Transform.LookAt, but points the transform's Up vector at the target
		/// </summary>
		public static void LookUp(this Transform t, Transform target)
		{
			t.rotation = Quaternion.FromToRotation(t.up, target.position - t.position) * t.rotation;
		}
		/// <summary>
		/// Equivalent to Transform.LookAt, but points the transform's Up vector at the target
		/// </summary>
		public static void LookUp(this Transform t, Vector3 target) {
			t.rotation = Quaternion.FromToRotation(t.up, target - t.position) * t.rotation;
		}

		/// <summary>
		/// Draws the curve in the scene view
		/// </summary>
		/// <param name="curve"></param>
		public static void Draw(this IParamCurve<Vector2> curve) {
			for (int i = 1; i < 100; i++) {
				Debug.DrawLine(curve.Eval((i - 1) / 99f), curve.Eval(i / 99f), Color.HSVToRGB((i / 99f), 1f, 1f));
			}
		}

		/// <summary>
		/// Finds the closest t value on this curve to the given point using a binary search. Returns a float value between 0f and 1f, inclusive.
		/// </summary>
		public static float ClosestT(this Freya.IParamCurve<Vector2> curve, Vector2 point, float margin = 0.0001f)
		{
			float lowerBound = 0f, upperBound = 1f, midPoint, slope;
			while (upperBound - lowerBound > margin) {
				midPoint = (upperBound + lowerBound) / 2f;
				slope = Vector2.Distance(point, curve.Eval(midPoint + margin)) - Vector2.Distance(point, curve.Eval(midPoint - margin));
				// The distance is decreasing
				if (slope < 0) { lowerBound = midPoint; }
				// The distance is increasing
				else if (slope > 0) { upperBound = midPoint; }
				// The slope is equal to zero, which means we've found the lowest value within the given margin
				else { break; }
			}
			return (upperBound + lowerBound) / 2f;
		}

	}
}