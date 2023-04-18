using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drifter : AI
{

	public enum Pattern
	{
		None,
		Sine,
		Spiral,
	}

	public Pattern pattern;

	public float sineWaveAmplitude = 1.0f;
	public float sineWaveFrequency = 1.0f;

	private void FixedUpdate()
	{
		Vector2 seekForce = Vector2.zero;
		switch (pattern) {
			default: case Pattern.None: break;
			case Pattern.Sine:
				if (Vector2.Distance(transform.position, Target) < 10f) { targetPosition *= new Vector2(-1f, 1f); }
				float time = Time.time % (2 * Mathf.PI / sineWaveFrequency);
				float sineWaveOffset = Mathf.Sin(time * sineWaveFrequency) * sineWaveAmplitude;
				Vector2 sineWaveTargetPosition = Target + new Vector2(0, sineWaveOffset);
				seekForce = Seek(sineWaveTargetPosition);
				break;
			case Pattern.Spiral:
				break;
		}
		rb.AddForce(seekForce);
	}

	private void OnDrawGizmos()
	{
		Shapes.Draw.UseDashes = true; Shapes.Draw.DashStyle = Shapes.DashStyle.defaultDashStyle;
		Shapes.Draw.DashSizeUniform *= 8f; Shapes.Draw.Thickness = 0.04f;
		Shapes.Draw.Line(transform.position, Target);
	}

}
