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
        Player1,
        Player2,
        Player3,
        Player4,
        Player5,
        Player6,
        Player7,
        Player8,
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
    public float rotation;

    public Vector2 RandomPointInZone()
    {
		
        // switch on shape and return a random point inside the zone depending on the shape
        switch (shape) {
            case Shape.Circle: return position + Random.insideUnitCircle * size.x;
            case Shape.Rectangle: return position + new Vector2(Random.Range(-0.5f, 0.5f) * size.x, Random.Range(-0.5f, 0.5f) * size.y);
            default: return Vector2.zero;
        }

	}

    public void Draw(Color color)
    {
        switch (shape) {
            case Shape.Circle: Shapes.Draw.Ring(position, size.x, color); break;
            case Shape.Rectangle: Shapes.Draw.RectangleBorder(position, Quaternion.Euler(0f, 0f, rotation), size, 0.3f, color); break;
            default: break;
        }
    }

}
