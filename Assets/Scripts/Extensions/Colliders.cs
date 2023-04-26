using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Extensions
{
	public static class Colliders
	{

		public static List<Collider2D> query = new List<Collider2D>(20);

		public static List<Collider2D> AttachedRigidbodies(this Rigidbody2D body) {
			body.GetAttachedColliders(query); return query;
		}


	}
}
