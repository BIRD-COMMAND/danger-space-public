using Freya;
using System;
using UnityEngine;

namespace Extensions
{
	public static class General
	{

		// Used in LookAt methods
		private static Vector2 lookDirection; private static float lookRotation;
		/// <summary>
		/// Points the Rigidbody2D's Up vector at the target. If a lerpFactor is supplied it will lerp the body's rotation by that amount toward the target.
		/// </summary>
		public static void LookAt(this Rigidbody2D body, Rigidbody2D target, float lerpFactor = 1.0f) { 
			lookDirection = (target.position - body.position).normalized;
			lookRotation = (Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg) - 90f;
			body.rotation = Mathf.LerpAngle(body.rotation, lookRotation, lerpFactor);
		}
		/// <summary>
		/// Points the Rigidbody2D's Up vector at the target. If a lerpFactor is supplied it will lerp the body's rotation by that amount toward the target.
		/// </summary>
		public static void LookAt(this Rigidbody2D body, Transform target, float lerpFactor = 1.0f) {
			lookDirection = ((Vector2)target.position - body.position).normalized;
			lookRotation = (Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg) - 90f;
			body.rotation = Mathf.LerpAngle(body.rotation, lookRotation, lerpFactor);
		}
		/// <summary>
		/// Points the Rigidbody2D's Up vector at the target. If a lerpFactor is supplied it will lerp the body's rotation by that amount toward the target.
		/// </summary>
		public static void LookAt(this Rigidbody2D body, Vector3 target, float lerpFactor = 1.0f) {
			lookDirection = ((Vector2)target - body.position).normalized;
			lookRotation = (Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg) - 90f;
			body.rotation = Mathf.LerpAngle(body.rotation, lookRotation, lerpFactor);
		}
		/// <summary>
		/// Points the Rigidbody2D's Up vector at the target. If a lerpFactor is supplied it will lerp the body's rotation by that amount toward the target.
		/// </summary>
		public static void LookAt(this Rigidbody2D body, Vector2 target, float lerpFactor = 1.0f) {
			lookDirection = (target - body.position).normalized;
			lookRotation = (Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg) - 90f;
			body.rotation = Mathf.LerpAngle(body.rotation, lookRotation, lerpFactor);
		}

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