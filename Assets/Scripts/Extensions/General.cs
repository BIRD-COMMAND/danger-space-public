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
			return;
		}
		/// <summary>
		/// Equivalent to Transform.LookAt, but points the transform's Up vector at the target
		/// </summary>
		public static void LookUp(this Transform t, Vector3 target) {
			t.rotation = Quaternion.FromToRotation(t.up, target - t.position) * t.rotation; 
			return;
		}



	}
}
