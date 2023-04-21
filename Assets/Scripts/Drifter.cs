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

	public bool visualizePath = true;
	public Pattern pattern;

	public float sineWaveAmplitude = 1.0f;
	public float sineWaveFrequency = 1.0f;

	private void FixedUpdate()
	{
		switch (pattern) {
			default: case Pattern.None: break;
			case Pattern.Sine:
				if (!pathing && Idle) { 
					GeneratePath();
					PathSetNextPosition(true);
					targetPosition *= new Vector2(-1f, 1f);
					_State = State.Self_Patrol;
					pathing = true;
				}
				break;
			case Pattern.Spiral: break;
		}
	}

	private void GeneratePath()
	{
		switch (pattern) {
			default: case Pattern.None: break;
			case Pattern.Sine:
				path.Clear();
				Vector2 start = transform.position, end = Target;
				float distance = Vector2.Distance(start, end);
				int numberOfPoints = Mathf.CeilToInt(distance / 0.5f);
				Vector2 direction = (end - start).normalized;
				float angle = Mathf.Atan2(direction.y, direction.x), t, yOffset;
				int numberOfCycles = Mathf.FloorToInt(distance / (Mathf.PI * sineWaveAmplitude * 2f));
				for (int i = 0; i <= numberOfPoints; i++) {
					t = (float)i / numberOfPoints;
					yOffset = sineWaveAmplitude * Mathf.Sin(t * Mathf.PI * numberOfCycles * 2f);
					path.Add(Vector2.Lerp(start, end, t) + new Vector2(yOffset * Mathf.Cos(angle - Mathf.PI / 2), yOffset * Mathf.Sin(angle - Mathf.PI / 2)));
				}
				break;
			case Pattern.Spiral: break;
		}

	}

	private void OnDrawGizmos()
	{
		//Shapes.Draw.UseDashes = true; Shapes.Draw.DashStyle = Shapes.DashStyle.defaultDashStyle;
		//Shapes.Draw.DashSizeUniform *= 8f; Shapes.Draw.Thickness = 0.04f;
		//Shapes.Draw.Line(transform.position, Target);
		//Shapes.Draw.UseDashes = false;
		if (!visualizePath) { return; }
		for (int i = 1; i < path.Count; i++) {
			Debug.DrawLine(path[i - 1], path[i], Color.HSVToRGB((float)i / path.Count, 1f, 1f));
		}
	}

}
