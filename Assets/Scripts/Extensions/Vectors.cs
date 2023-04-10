using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Extensions
{

	/// <summary>
	/// Extension methods for Vector2 and Vector3
	/// </summary>
	public static class Vectors
	{

		#region Conversions and Threshold Validations

		/// <summary>
		/// Convenience method for casting from Vector3 to Vector2
		/// </summary>
		public static Vector2 ToV2(this Vector3 vec) { return vec; }
		/// <summary>
		/// Convenience method for casting from Vector2 to Vector3
		/// </summary>
		public static Vector3 ToV3(this Vector2 vec) { return vec; }

		/// <summary>
		/// Returns a new VectorInt made by using Mathf.RoundToInt() on each component
		/// </summary>
		public static Vector2Int ToVector2Int(this Vector2 vec)
		{
			return new Vector2Int(Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.y));
		}
		/// <summary>
		/// Returns a new VectorInt made by using Mathf.RoundToInt() on each component
		/// </summary>
		public static Vector3Int ToInt(this Vector3 vec)
		{
			return new Vector3Int(Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.y), Mathf.RoundToInt(vec.z));
		}

		/// <summary>
		/// Sets any NaN component values to 0f.
		/// </summary>
		public static Vector2 Validate(this Vector2 vec) {
			if (float.IsNaN(vec.x) || float.IsInfinity(vec.y)) { Debug.Log("Validate found NaN"); }
			return new Vector2(float.IsNaN(vec.x) ? 0f : vec.x, float.IsNaN(vec.y) ? 0f : vec.y);
		}
		/// <summary> 
		/// Sets any NaN component values to 0f.
		/// </summary>
		public static Vector3 Validate(this Vector3 vec) {
			return new Vector3(float.IsNaN(vec.x) ? 0f : vec.x, float.IsNaN(vec.y) ? 0f : vec.y, float.IsNaN(vec.z) ? 0f : vec.z);
		}

		/// <summary>
		/// If vec.x, vec.y, or vec.magnitude are greater than the threshold, returns the Vector unmodified, otherwise returns Vector2.zero
		/// </summary>
		public static Vector2 DeadZone(this Vector2 vec, float threshold)
		{
			return (vec.x > threshold || vec.y > threshold || vec.magnitude > threshold)
				? vec
				: Vector2.zero
			;
		}
		/// <summary>
		/// If vec.x, vec.y, vec.z, or vec.magnitude are greater than the threshold, returns the Vector unmodified, otherwise returns Vector3.zero
		/// </summary>
		public static Vector3 DeadZone(this Vector3 vec, float threshold)
		{
			return (vec.x > threshold || vec.y > threshold || vec.z > threshold || vec.magnitude > threshold)
				? vec
				: Vector3.zero
			;
		}


		#endregion

		#region Angles and Dot Products

		/// <summary>
		/// Normalizes this vector and returns the Dot Product against Vector.up
		/// </summary>
		public static float DotUp(this Vector2 vec, bool abs = false) { 
			return abs ? Mathf.Abs(Vector2.Dot(vec.normalized, Vector2.up)) : Vector2.Dot(vec.normalized, Vector2.up); 
		}
		/// <summary>
		/// Normalizes this vector and returns the Dot Product against Vector.down
		/// </summary>
		public static float DotDown(this Vector2 vec, bool abs = false) { 
			return abs ? Mathf.Abs(Vector2.Dot(vec.normalized, Vector2.down)) : Vector2.Dot(vec.normalized, Vector2.down); 
		}
		/// <summary>
		/// Normalizes this vector and returns the Dot Product against Vector.left
		/// </summary>
		public static float DotLeft(this Vector2 vec, bool abs = false) { 
			return abs ? Mathf.Abs(Vector2.Dot(vec.normalized, Vector2.left)) : Vector2.Dot(vec.normalized, Vector2.left); 
		}
		/// <summary>
		/// Normalizes this vector and returns the Dot Product against Vector.right
		/// </summary>
		public static float DotRight(this Vector2 vec, bool abs = false) { 
			return abs ? Mathf.Abs(Vector2.Dot(vec.normalized, Vector2.right)) : Vector2.Dot(vec.normalized, Vector2.right); 
		}

		/// <summary>
		/// Normalizes this vector and returns the Dot Product against Vector.up
		/// </summary>
		public static float DotUp(this Vector3 vec, bool abs = false) { 
			return abs ? Mathf.Abs(Vector3.Dot(vec.normalized, Vector3.up)) : Vector3.Dot(vec.normalized, Vector3.up); 
		}
		/// <summary>
		/// Normalizes this vector and returns the Dot Product against Vector.down
		/// </summary>
		public static float DotDown(this Vector3 vec, bool abs = false) { 
			return abs ? Mathf.Abs(Vector3.Dot(vec.normalized, Vector3.down)) : Vector3.Dot(vec.normalized, Vector3.down); 
		}
		/// <summary>
		/// Normalizes this vector and returns the Dot Product against Vector.left
		/// </summary>
		public static float DotLeft(this Vector3 vec, bool abs = false) { 
			return abs ? Mathf.Abs(Vector3.Dot(vec.normalized, Vector3.left)) : Vector3.Dot(vec.normalized, Vector3.left); 
		}
		/// <summary>
		/// Normalizes this vector and returns the Dot Product against Vector.right
		/// </summary>
		public static float DotRight(this Vector3 vec, bool abs = false) { 
			return abs ? Mathf.Abs(Vector3.Dot(vec.normalized, Vector3.right)) : Vector3.Dot(vec.normalized, Vector3.right); 
		}
		/// <summary>
		/// Normalizes this vector and returns the Dot Product against Vector.forward
		/// </summary>
		public static float DotForward(this Vector3 vec, bool abs = false) { 
			return abs ? Mathf.Abs(Vector3.Dot(vec.normalized, Vector3.forward)) : Vector3.Dot(vec.normalized, Vector3.forward); 
		}
		/// <summary>
		/// Normalizes this vector and returns the Dot Product against Vector.back
		/// </summary>
		public static float DotBack(this Vector3 vec, bool abs = false) { 
			return abs ? Mathf.Abs(Vector3.Dot(vec.normalized, Vector3.back)) : Vector3.Dot(vec.normalized, Vector3.back); 
		}

		/// <summary>
		/// Returns true if this Vector2 is within 30° of <paramref name="otherVector"/>
		/// </summary>
		public static bool IsWithin30DegreesOf(this Vector2 vec, Vector2 otherVector)
		{
			return Vector2.Dot(vec.normalized, otherVector.normalized) > 0.8659f;
		}
		/// <summary>
		/// Returns true if this Vector2 is within 45° of <paramref name="otherVector"/>
		/// </summary>
		public static bool IsWithin45DegreesOf(this Vector2 vec, Vector2 otherVector)
		{
			return Vector2.Dot(vec.normalized, otherVector.normalized) > 0.707f;
		}
		/// <summary>
		/// Returns true if this Vector2 is within 90° of <paramref name="otherVector"/>
		/// </summary>
		public static bool IsWithin90DegreesOf(this Vector2 vec, Vector2 otherVector)
		{
			return Vector2.Dot(vec.normalized, otherVector.normalized) >= 0f;
		}

		#endregion

		#region Min, Max, Absolute Values

		/// <summary>
		/// Returns true if all of the Vector's components are greater than or equal to 0
		/// </summary>
		public static bool HasPositiveComponents(this Vector2 vec) { return vec.x >= 0f && vec.y >= 0f; }
		/// <summary>
		/// Returns true if all of the Vector's components are greater than or equal to 0
		/// </summary>
		public static bool HasPositiveComponents(this Vector3 vec) { return vec.x >= 0f && vec.y >= 0f && vec.z >= 0f; }

		/// <summary>
		/// Returns this Vector with each component set as its absolute value.
		/// </summary>
		public static Vector2 Abs(this Vector2 vec)
		{
			return new Vector2(Mathf.Abs(vec.x), Mathf.Abs(vec.y));
		}
		/// <summary>
		/// Returns this Vector with each component set as its absolute value.
		/// </summary>
		public static Vector3 Abs(this Vector3 vec)
		{
			return new Vector3(Mathf.Abs(vec.x), Mathf.Abs(vec.y), Mathf.Abs(vec.z));
		}

		/// <summary>
		/// Returns whichever component of the vector has the greatest value.
		/// </summary>
		public static float MaxComponent(this Vector2 vec) { return Mathf.Max(vec.x, vec.y); }
		/// <summary>
		/// Returns whichever component of the vector has the greatest value.
		/// </summary>
		public static float MaxComponent(this Vector3 vec) { return Mathf.Max(vec.x, vec.y, vec.z); }
		/// <summary>
		/// Returns whichever component of the vector has the smallest value.
		/// </summary>
		public static float MinComponent(this Vector2 vec) { return Mathf.Min(vec.x, vec.y); }
		/// <summary>
		/// Returns whichever component of the vector has the smallest value.
		/// </summary>
		public static float MinComponent(this Vector3 vec) { return Mathf.Min(vec.x, vec.y, vec.z); }

		/// <summary>
		/// Returns a vector which retains the value for the greatest component and replaces the other component with 0.
		/// </summary>
		public static Vector2 MaxComponentOnly(this Vector2 vec) { 
			return vec.x > vec.y ? new Vector2(vec.x, 0f) : new Vector2(0f, vec.y); 
		}		
		/// <summary>
		/// Returns a vector which retains the value for the smallest component and replaces the other component with 0.
		/// </summary>
		public static Vector3 MinComponentOnly(this Vector2 vec) { 
			return vec.x < vec.y ? new Vector2(vec.x, 0f) : new Vector2(0f, vec.y); 
		}

		#endregion

		#region Spacial Operations

		/// <summary>
		/// Returns the normalized Vector representing the direction from this Vector toward the <paramref name="target"/> Vector.
		/// </summary>
		public static Vector2 DirTo(this Vector2 vec, Vector2 target) { return (target - vec).normalized; }
		/// <summary>
		/// Returns the normalized Vector representing the direction from this Vector toward the <paramref name="target"/> Vector.
		/// </summary>
		public static Vector3 DirTo(this Vector3 vec, Vector3 target) { return (target - vec).normalized; }

		/// <summary>
		/// Returns the distance from this Vector to the <paramref name="target"/> Vector. Distance is always positive or 0.
		/// </summary>
		public static float DistTo(this Vector2 vec, Vector2 target) { return (target - vec).magnitude; }
		/// <summary>
		/// Returns the distance from this Vector to the <paramref name="target"/> Vector. Distance is always positive or 0.
		/// </summary>
		public static float DistTo(this Vector3 vec, Vector3 target) { return (target - vec).magnitude; }

		/// <summary>
		/// Returns the offset from this Vector to the <paramref name="target"/> Vector.
		/// </summary>
		public static Vector2 To(this Vector2 vec, Vector2 target) { return (target - vec); }
		/// <summary>
		/// Returns the offset from this Vector to the <paramref name="target"/> Vector.
		/// </summary>
		public static Vector3 To(this Vector3 vec, Vector3 target) { return (target - vec); }

		/// <summary>
		/// Returns the value of calling Vector2.Perpendicular() on this vector
		/// <br>The result is always rotated 90-degrees in a <b>counter-clockwise</b> direction for a 2D coordinate system where the positive Y axis goes up.</br>
		/// </summary>
		public static Vector2 Perpendicular(this Vector2 vec) { return Vector2.Perpendicular(vec); }

		/// <summary>
		/// Returns the opposite perpendicular vector that you would get from calling Vector2.Perpendicular() on this vector
		/// <br>The result is always rotated 90-degrees in a <b>clockwise</b> direction for a 2D coordinate system where the positive Y axis goes up.</br>
		/// </summary>
		public static Vector2 PerpendicularReverse(this Vector2 vec) { return Vector2.Perpendicular(-vec); }

		/// <summary>
		/// Returns true if <paramref name="a"/>.transform.position is closer than <paramref name="b"/>.transform.position
		/// </summary>
		public static bool IsCloser(this Transform t, Component a, Component b)
		{
			return Vector3.Distance(t.position, a.transform.position) <
				   Vector3.Distance(t.position, b.transform.position);
		}
		/// <summary>
		/// Returns true if <paramref name="a"/> is closer than <paramref name="b"/>
		/// </summary>
		public static bool IsCloser(this Transform t, Vector3 a, Vector3 b)
		{
			return Vector3.Distance(t.position, a) < Vector3.Distance(t.position, b);
		}
		/// <summary>
		/// Returns true if <paramref name="a"/> is closer than <paramref name="b"/> to this Component's transform.position.
		/// </summary>
		public static bool IsCloser(this Component c, Component a, Component b)
		{
			return Vector3.Distance(c.transform.position, a.transform.position) <
				   Vector3.Distance(c.transform.position, b.transform.position);
		}
		/// <summary>
		/// Returns true if <paramref name="a"/> is closer than <paramref name="b"/> to this Component's transform.position.
		/// </summary>
		public static bool IsCloser(this Component c, Vector3 a, Vector3 b)
		{
			return Vector3.Distance(c.transform.position, a) < Vector3.Distance(c.transform.position, b);
		}

		#endregion

		#region Math and Component Operations

		/// <summary>
		/// Returns the aspect ratio (Width/Height, or x/y) for this Vector2. Returns 0 if Y is 0.
		/// </summary>
		public static float AspectRatio(this Vector2 vec) { return vec.y == 0f ? 0f : vec.x / vec.y; }

		/// <summary>
		/// Casts <paramref name="vec3"/> to a Vector2, adds that value to this Vector2, and returns the result
		/// </summary>
		public static Vector2 Plus(this Vector2 vec, Vector3 vec3) { return vec + (Vector2)vec3; }
		/// <summary>
		/// Casts <paramref name="vec2"/> to a Vector3, adds that value to this Vector3, and returns the result
		/// </summary>
		public static Vector3 Plus(this Vector3 vec, Vector2 vec2) { return vec + (Vector3)vec2; }

		/// <summary>
		/// Casts <paramref name="vec3"/> to a Vector2, subtracts that value from this Vector2, and returns the result
		/// </summary>
		public static Vector2 Minus(this Vector2 vec, Vector3 vec3) { return vec - (Vector2)vec3; }
		/// <summary>
		/// Casts <paramref name="vec2"/> to a Vector3, subtracts that value from this Vector3, and returns the result
		/// </summary>
		public static Vector3 Minus(this Vector3 vec, Vector2 vec2) { return vec - (Vector3)vec2; }

		/// <summary>
		/// Returns this Vector multiplied by 0.5f
		/// </summary>
		public static Vector2 Halved(this Vector2 vec) { return vec * 0.5f; }
		/// <summary>
		/// Returns this Vector multiplied by 0.5f
		/// </summary>
		public static Vector3 Halved(this Vector3 vec) { return vec * 0.5f; }

		/// <summary>
		/// Multiplies Vector by -1f
		/// </summary>
		public static Vector2 Invert(this Vector2 vec) { return vec * -1f; }
		/// <summary>
		/// Multiplies Vector by -1f
		/// </summary>
		public static Vector3 Invert(this Vector3 vec) { return vec * -1f; }
		/// <summary>
		/// Multiplies Vector.x by -1f
		/// </summary>
		public static Vector2 InvertX(this Vector2 vec)
		{
			return new Vector2(vec.x * -1f, vec.y);
		}
		/// <summary>
		/// Multiplies Vector.y by -1f
		/// </summary>
		public static Vector2 InvertY(this Vector2 vec)
		{
			return new Vector2(vec.x, vec.y * -1f);
		}
		/// <summary>
		/// Multiplies Vector.x by -1f
		/// </summary>
		public static Vector3 InvertX(this Vector3 vec)
		{
			return new Vector3(vec.x * -1f, vec.y, vec.z);
		}
		/// <summary>
		/// Multiplies Vector.y by -1f
		/// </summary>
		public static Vector3 InvertY(this Vector3 vec)
		{
			return new Vector3(vec.x, vec.y * -1f, vec.z);
		}
		/// <summary>
		/// Multiplies Vector.z by -1f
		/// </summary>
		public static Vector3 InvertZ(this Vector3 vec)
		{
			return new Vector3(vec.x, vec.y, vec.z * -1f);
		}

		/// <summary>
		/// Returns a vector with the specified component replaced with the value provided, and all other components left unchanged.
		/// </summary>
		public static Vector2 WithX(this Vector2 vec, float x) { return new Vector2(x, vec.y); }
		/// <summary>
		/// Returns a vector with the specified component replaced with the value provided, and all other components left unchanged.
		/// </summary>
		public static Vector2 WithY(this Vector2 vec, float y) { return new Vector2(vec.x, y); }
		/// <summary>
		/// Returns a Vector3 using the existing x and y components and the z value provided.
		/// </summary>
		public static Vector3 WithZ(this Vector2 vec, float z) { return new Vector3(vec.x, vec.y, z); }
		/// <summary>
		/// Returns a vector with the specified component replaced with the value provided, and all other components left unchanged.
		/// </summary>
		public static Vector3 WithX(this Vector3 vec, float x) { return new Vector3(x, vec.y, vec.z); }
		/// <summary>
		/// Returns a vector with the specified component replaced with the value provided, and all other components left unchanged.
		/// </summary>
		public static Vector3 WithY(this Vector3 vec, float y) { return new Vector3(vec.x, y, vec.z); }
		/// <summary>
		/// Returns a vector with the specified component replaced with the value provided, and all other components left unchanged.
		/// </summary>
		public static Vector3 WithZ(this Vector3 vec, float z) { return new Vector3(vec.x, vec.y, z); }

		/// <summary>
		/// Returns Vector2(Vector.x + <paramref name="xOffset"/>, Vector.y + <paramref name="yOffset"/>)
		/// </summary>
		public static Vector2 Offset(this Vector2 vec, float xOffset, float yOffset)
		{
			return new Vector2(vec.x + xOffset, vec.y + yOffset);
		}
		/// <summary>
		/// Offsets Vector.x by <paramref name="value"/> and returns the modified Vector
		/// </summary>
		public static Vector2 OffsetX(this Vector2 vec, float value)
		{
			return new Vector2(vec.x + value, vec.y);
		}
		/// <summary>
		/// Offsets Vector.y by <paramref name="value"/> and returns the modified Vector
		/// </summary>
		public static Vector2 OffsetY(this Vector2 vec, float value)
		{
			return new Vector2(vec.x, vec.y + value);
		}
		/// <summary>
		/// Returns Vector3(Vector.x + <paramref name="xOffset"/>, Vector.y + <paramref name="yOffset"/>, Vector.z + <paramref name="zOffset"/>)
		/// </summary>
		public static Vector3 Offset(this Vector3 vec, float xOffset, float yOffset, float zOffset)
		{
			return new Vector3(vec.x + xOffset, vec.y + yOffset, vec.z + zOffset);
		}
		/// <summary>
		/// Offsets Vector.x by <paramref name="value"/> and returns the modified Vector
		/// </summary>
		public static Vector3 OffsetX(this Vector3 vec, float value)
		{
			return new Vector3(vec.x + value, vec.y, vec.z);
		}
		/// <summary>
		/// Offsets Vector.y by <paramref name="value"/> and returns the modified Vector
		/// </summary>
		public static Vector3 OffsetY(this Vector3 vec, float value)
		{
			return new Vector3(vec.x, vec.y + value, vec.z);
		}
		/// <summary>
		/// Offsets Vector.z by <paramref name="value"/> and returns the modified Vector
		/// </summary>
		public static Vector3 OffsetZ(this Vector3 vec, float value)
		{
			return new Vector3(vec.x, vec.y, vec.z + value);
		}

		/// <summary>
		/// Replaces Vector.x with (value - Vector.x)
		/// </summary>
		public static Vector2 SubtractXFrom(this Vector2 vec, float value)
		{
			return new Vector2(value - vec.x, vec.y);
		}
		/// <summary>
		/// Replaces Vector.y with (value - Vector.y)
		/// </summary>
		public static Vector2 SubtractYFrom(this Vector2 vec, float value)
		{
			return new Vector2(vec.x, value - vec.y);
		}
		/// <summary>
		/// Replaces Vector.x with (value - Vector.x)
		/// </summary>
		public static Vector3 SubtractXFrom(this Vector3 vec, float value)
		{
			return new Vector3(value - vec.x, vec.y, vec.z);
		}
		/// <summary>
		/// Replaces Vector.y with (value - Vector.y)
		/// </summary>
		public static Vector3 SubtractYFrom(this Vector3 vec, float value)
		{
			return new Vector3(vec.x, value - vec.y, vec.z);
		}
		/// <summary>
		/// Replaces Vector.z with (value - Vector.z)
		/// </summary>
		public static Vector3 SubtractZFrom(this Vector3 vec, float value)
		{
			return new Vector3(vec.x, vec.y, value - vec.z);
		}

		/// <summary>
		/// Flips the X component of a normalized Vector -> (1f - x)
		/// </summary>
		public static Vector2 InvertNormalX(this Vector2 vec)
		{
			return new Vector2(1f - vec.x, vec.y);
		}
		/// <summary>
		/// Flips the Y component of a normalized Vector -> (1f - y)
		/// </summary>
		public static Vector2 InvertNormalY(this Vector2 vec)
		{
			return new Vector2(vec.x, 1f - vec.y);
		}
		/// <summary>
		/// Flips the X component of a normalized Vector -> (1f - x)
		/// </summary>
		public static Vector3 InvertNormalX(this Vector3 vec)
		{
			return new Vector3(1f - vec.x, vec.y, vec.z);
		}
		/// <summary>
		/// Flips the Y component of a normalized Vector -> (1f - y)
		/// </summary>
		public static Vector3 InvertNormalY(this Vector3 vec)
		{
			return new Vector3(vec.x, 1f - vec.y, vec.z);
		}
		/// <summary>
		/// Flips the Z component of a normalized Vector -> (1f - z)
		/// </summary>
		public static Vector3 InvertNormalZ(this Vector3 vec)
		{
			return new Vector3(vec.x, vec.y, 1f - vec.z);
		}

		#endregion

		#region Collection Operations

		/// <summary>
		/// Returns the sum of all the vectors in the collection
		/// </summary>
		public static Vector2 Sum(this IEnumerable<Vector2> vecs)
		{
			Vector2 sum = Vector2.zero;
			foreach (Vector2 item in vecs) { sum += item; }
			return sum;
		}
		/// <summary>
		/// Returns the sum of all the vectors in the collection
		/// </summary>
		public static Vector3 Sum(this IEnumerable<Vector3> vecs)
		{
			Vector3 sum = Vector3.zero;
			foreach (Vector3 item in vecs) { sum += item; }
			return sum;
		}

		/// <summary>
		/// Returns the average of all the vectors in the collection
		/// </summary>
		public static Vector2 Average(this IEnumerable<Vector2> vecs) { return vecs.Sum() / vecs.Count(); }
		/// <summary>
		/// Returns the average of all the vectors in the collection
		/// </summary>
		public static Vector3 Average(this IEnumerable<Vector3> vecs) { return vecs.Sum() / vecs.Count(); }

		#endregion

		#region Lerps

		/// <summary>
		/// Lerps between Vector.x and Vector.y and returns the value at <paramref name="t"/>
		/// </summary>
		public static float LerpXY(this Vector2 vec, float t) { return Mathf.Lerp(vec.x, vec.y, t); }
		/// <summary>
		/// Lerps between Vector.y and Vector.x and returns the value at <paramref name="t"/>
		/// </summary>
		public static float LerpYX(this Vector2 vec, float t) { return Mathf.Lerp(vec.y, vec.x, t); }

		/// <summary>
		/// Lerps between Vector.x and Vector.y and returns the value at <paramref name="t"/>
		/// </summary>
		public static float LerpXY(this Vector3 vec, float t) { return Mathf.Lerp(vec.x, vec.y, t); }
		/// <summary>
		/// Lerps between Vector.y and Vector.x and returns the value at <paramref name="t"/>
		/// </summary>
		public static float LerpYX(this Vector3 vec, float t) { return Mathf.Lerp(vec.y, vec.x, t); }

		/// <summary>
		/// SmoothSteps between Vector.x and Vector.y and returns the value at <paramref name="t"/>
		/// </summary>
		public static float SmoothStepXY(this Vector2 vec, float t) { return Mathf.SmoothStep(vec.x, vec.y, t); }
		/// <summary>
		/// SmoothSteps between Vector.y and Vector.x and returns the value at <paramref name="t"/>
		/// </summary>
		public static float SmoothStepYX(this Vector2 vec, float t) { return Mathf.SmoothStep(vec.y, vec.x, t); }

		/// <summary>
		/// SmoothSteps between Vector.x and Vector.y and returns the value at <paramref name="t"/>
		/// </summary>
		public static float SmoothStepXY(this Vector3 vec, float t) { return Mathf.SmoothStep(vec.x, vec.y, t); }
		/// <summary>
		/// SmoothSteps between Vector.y and Vector.x and returns the value at <paramref name="t"/>
		/// </summary>
		public static float SmoothStepYX(this Vector3 vec, float t) { return Mathf.SmoothStep(vec.y, vec.x, t); }

		#endregion

	}

}