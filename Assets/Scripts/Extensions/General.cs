using Freya;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Extensions
{
	public static class General
	{

		/// <summary>
		/// Returns the rotation around the Z axis
		/// </summary>
		public static float Angle2D(this Quaternion q) { return q.eulerAngles.z; }

		// Used in LookAt methods
		private static Vector2 lookDirection; private static float lookRotation;
		/// <summary>
		/// Points the Rigidbody2D's Up vector at the target. If a lerpFactor is supplied it will lerp the body's rotation by that amount toward the target.
		/// </summary>
		public static void LookAt(this Rigidbody2D body, Rigidbody2D target, float lerpFactor = 1.0f) { 
			lookDirection = (target.position - body.position).normalized;
			lookRotation = (Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg) - 90f;
			body.MoveRotation(Mathf.LerpAngle(body.rotation, lookRotation, lerpFactor));
		}
		/// <summary>
		/// Points the Rigidbody2D's Up vector at the target. If a lerpFactor is supplied it will lerp the body's rotation by that amount toward the target.
		/// </summary>
		public static void LookAt(this Rigidbody2D body, Transform target, float lerpFactor = 1.0f) {
			lookDirection = ((Vector2)target.position - body.position).normalized;
			lookRotation = (Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg) - 90f;
			body.MoveRotation(Mathf.LerpAngle(body.rotation, lookRotation, lerpFactor));
		}
		/// <summary>
		/// Points the Rigidbody2D's Up vector at the target. If a lerpFactor is supplied it will lerp the body's rotation by that amount toward the target.
		/// </summary>
		public static void LookAt(this Rigidbody2D body, Vector3 target, float lerpFactor = 1.0f) {
			lookDirection = ((Vector2)target - body.position).normalized;
			lookRotation = (Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg) - 90f;
			body.MoveRotation(Mathf.LerpAngle(body.rotation, lookRotation, lerpFactor));
		}
		/// <summary>
		/// Points the Rigidbody2D's Up vector at the target. If a lerpFactor is supplied it will lerp the body's rotation by that amount toward the target.
		/// </summary>
		public static void LookAt(this Rigidbody2D body, Vector2 target, float lerpFactor = 1.0f) {
			lookDirection = (target - body.position).normalized;
			lookRotation = (Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg) - 90f;
			body.MoveRotation(Mathf.LerpAngle(body.rotation, lookRotation, lerpFactor));
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
		/// Returns a position that is <paramref name="units"/> forward from this transform position.
		/// </summary>
		public static Vector2 UnitsForward(this Transform t, float units)	{ return t.position + (t.up * units);	  }
		/// <summary>
		/// Returns a position that is <paramref name="units"> backward from this transform position.
		/// </summary>
		public static Vector2 UnitsBackward(this Transform t, float units)	{ return t.position + (-t.up * units);	  }
		/// <summary>
		/// Returns a position that is <paramref name="units"> left from this transform position.
		/// </summary>
		public static Vector2 UnitsLeft(this Transform t, float units)		{ return t.position + (-t.right * units); }
		/// <summary>
		/// Returns a position that is <paramref name="units"> right from this transform position.
		/// </summary>
		public static Vector2 UnitsRight(this Transform t, float units)		{ return t.position + (t.right * units);  }

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

		/// <summary>
		/// Equivalent to TryGetComponent, but checks the parent GameObjects as well.
		/// </summary>
		public static bool TryGetComponentInParent<T>(this Component c, out T component) where T : Component {
			component = c.GetComponentInParent<T>(); if (component) { return true; } else { return false; }
		}

		/// <summary>
		/// Finds the first component of type <typeparamref name="T"/> on this GameObject, any parent, or any child, in that order. Returns null if no component is found.
		/// </summary>
		public static T FindComponent<T>(this Component c) where T : Component {
			Component component = c.GetComponentInParent<T>(); if (component) { return (T)component; }
			component = c.GetComponentInChildren<T>(); if (component) { return (T)component; }
			return null;
		}
		/// <summary>
		/// Finds the first component of type <typeparamref name="T"/> on this GameObject, any parent, or any child, in that order. Returns null if no component is found.
		/// </summary>
		public static T FindComponent<T>(this GameObject c) where T : Component
		{
			Component component = c.GetComponentInParent<T>(); if (component) { return (T)component; }
			component = c.GetComponentInChildren<T>(); if (component) { return (T)component; }
			return null;
		}
		/// <summary>
		/// Finds all components of type <typeparamref name="T"/> on this GameObject, any parent, or any child, in that order. Returns null if no component is found.
		/// </summary>
		public static List<T> FindComponents<T>(this Component c) where T : Component
		{
			List<T> components = new List<T>();
			components.AddRange(c.GetComponentsInParent<T>());
			components.AddRange(c.GetComponentsInChildren<T>());
			if (components.Count < 2) { return components; }
			else { return components.Distinct().ToList(); }
		}
		/// <summary>
		/// Finds all components of type <typeparamref name="T"/> on this GameObject, any parent, or any child, in that order. Returns null if no component is found.
		/// </summary>
		public static List<T> FindComponents<T>(this GameObject c) where T : Component
		{
			List<T> components = new List<T>();
			components.AddRange(c.GetComponentsInParent<T>());
			components.AddRange(c.GetComponentsInChildren<T>());
			if (components.Count < 2) { return components; }
			else { return components.Distinct().ToList(); }
		}
		/// <summary>
		/// Finds the first component of type <typeparamref name="T"/> on this GameObject, any parent, or any child, in that order. Returns null if no component is found.
		/// </summary>
		public static bool FindComponent<T>(this Component c, out T component) where T : Component
		{
			component = c.GetComponentInParent<T>(); if (component) { return true; }
			component = c.GetComponentInChildren<T>(); if (component) { return true; }
			component = null; return false;
		}
		/// <summary>
		/// Finds the first component of type <typeparamref name="T"/> on this GameObject, any parent, or any child, in that order. Returns null if no component is found.
		/// </summary>
		public static bool FindComponent<T>(this GameObject c, out T component) where T : Component
		{
			component = c.GetComponentInParent<T>(); if (component) { return true; }
			component = c.GetComponentInChildren<T>(); if (component) { return true; }
			component = null; return false;
		}

		/// <summary>
		/// Approximates the total collision force between both bodies in a Collision2D
		/// </summary>
		public static Vector2 ApproximateForce(this Collision2D collision)
		{
			// Calculate the average mass of the two colliding bodies
			// float averageMass = (collision.rigidbody.mass + collision.otherRigidbody.mass) / 2.0f;

			// Calculate the relative velocity of the collision
			// Vector2 relativeVelocity = collision.relativeVelocity;

			// Calculate the approximated force using the average mass and relative velocity
			// Vector2 approximatedForce = relativeVelocity * averageMass;
			// return approximatedForce;

			return collision.relativeVelocity * ((collision.rigidbody.mass + collision.otherRigidbody.mass) / 2.0f);
		}

	}
}