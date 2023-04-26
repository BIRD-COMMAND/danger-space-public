using Shapes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Zone
{

    public enum Type
    {
		Rally,
		Fallback,
		General1,
        General2,
        General3,
        General4,
        Support1,
        Support2,
        Support3,
        Support4,
        PlayerFront,
        PlayerBack,
        PlayerLeft,
        PlayerRight
	}

    public enum Shape
    {
        Circle,
        Rectangle,
    }

    public Shape shape;
    public bool active;
    public Vector2 size;
    public Vector2 position;
    /// <summary>
    /// Rotation of the zone in radians
    /// </summary>
    public float rotation;

    public Vector2 RandomPointInZone()
    {		
        // switch on shape and return a random point inside the zone depending on the shape
        switch (shape) {
            case Shape.Circle: return position + Random.insideUnitCircle * size.x;
            case Shape.Rectangle:
                Vector2 relativePoint = new Vector2(Random.Range(-0.5f, 0.5f) * size.x, Random.Range(-0.5f, 0.5f) * size.y);
				return position + new Vector2(
	                relativePoint.x * Mathf.Cos(rotation) - relativePoint.y * Mathf.Sin(rotation),
	                relativePoint.x * Mathf.Sin(rotation) + relativePoint.y * Mathf.Cos(rotation)
                );				
            default: return Vector2.zero;
        }
	}

    public void Draw(Color color)
    {
        switch (shape) {
            case Shape.Circle: Shapes.Draw.Ring(position, size.x, color); break;
            case Shape.Rectangle: Shapes.Draw.RectangleBorder(position, Quaternion.Euler(0f, 0f, rotation * Mathf.Rad2Deg), size, 0.3f, color); break;
            default: break;
        }
    }

}
