using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Extensions;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;

public class Approach
{

	public Type type = Type.None;
	public Vector2 start, end;
	public Transform item;
	public Transform target;
	public bool targetRight, complete;
	public float curveFactor = 0.5f, t = 0f;

	private Freya.Bezier2D curve = new Freya.Bezier2D(new Vector2[3] { Vector2.zero, Vector2.zero, Vector2.zero });
	private Vector2 dirToTarget; private float distToTarget = 0f, totalDistance = 0f;

	public Approach(Transform item, Transform target, Type type) {
		this.type = type; this.item = item; this.target = target; Reset();
	}

	public void Reset() {
		start = item.position;
		curve[0] = start;
		end = target.position;
		totalDistance = start.DistTo(end);
		t = 0f;
		complete = false;
	}

	public void Update(bool approach)
	{

		if (complete || !target) { return; }

		dirToTarget = item.DirTo(target);
		distToTarget = item.DistTo(target);

		switch (type) {
			default: case Type.None: return;
			case Type.CircleLeft:	curve[1] = (Vector2)target.position + (2f * totalDistance *  start.DirTo(end).Perpendicular()); curve[2] = start + (start.To(end) * 1.9f); break;
			case Type.CircleRight:	curve[1] = (Vector2)target.position + (2f * totalDistance * -start.DirTo(end).Perpendicular()); curve[2] = start + (start.To(end) * 1.9f); break;
			case Type.CurveLeft:	curve[1] = ((start + (Vector2)target.position) * 0.5f) + (totalDistance * curveFactor *  dirToTarget.Perpendicular()); curve[2] = target.position; break;
			case Type.CurveRight:	curve[1] = ((start + (Vector2)target.position) * 0.5f) + (totalDistance * curveFactor * -dirToTarget.Perpendicular()); curve[2] = target.position; break;
			case Type.Direct:		curve[1] =  (start + (Vector2)target.position) * 0.5f; curve[2] = target.position; break;
		}

		Debug.DrawLine(curve[0], curve[1], Color.red);
		Debug.DrawLine(curve[1], curve[2], Color.red);

		curve.Draw();

		if (approach) { t += 0.01f;
			if (t < 1f) { item.position = curve.Eval(t); }
			else { 
				/*item.position = target.position;*/ 
				complete = true; 
				if (type != Type.CircleLeft && type != Type.CircleRight) {
					item.transform.position = new Vector2(-60f, 30f); 
				}
				Reset();
			}
		}

	}

	public enum Type
	{
		None,
		CircleLeft,
		CircleRight,
		CurveLeft,
		CurveRight,
		Direct
	}

}
