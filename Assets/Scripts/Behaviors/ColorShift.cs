using Shapes;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(ShapeRenderer))]
public class ColorShift : MonoBehaviour
{
	[Min(0f)]
	public float offset = 0f;
	[Min(0.001f)]
    public float speed = 1f;

    private ShapeRenderer shape;
	private void Start() { shape = GetComponent<ShapeRenderer>(); }

	private static float a;

	// Update is called once per frame
	void Update() { 
		switch (shape) {
			case Disc:
				switch ((shape as Disc).ColorMode) {
					case Disc.DiscColorMode.Single:
						a = (shape as Disc).Color.a;
						shape.Color = Color.HSVToRGB(((Time.time * speed) + offset) % 1f, 1f, 1f).WithAlpha(a);
						break;
					case Disc.DiscColorMode.Radial:
						a = (shape as Disc).ColorInner.a;
						(shape as Disc).ColorInner = Color.HSVToRGB(((Time.time * speed) + offset) % 1f, 1f, 1f).WithAlpha(a);
						a = (shape as Disc).ColorOuter.a;
						(shape as Disc).ColorOuter = Color.HSVToRGB(((Time.time * speed) + offset) % 1f, 1f, 1f).WithAlpha(a);
						break;
					case Disc.DiscColorMode.Angular:
						break;
					case Disc.DiscColorMode.Bilinear:
						break;
					default:
						break;
				}
				break;
			default:
				a = (shape as Disc).Color.a;
				shape.Color = Color.HSVToRGB(((Time.time * speed) + offset) % 1f, 1f, 1f).WithAlpha(a);
				break;
		}
	}

}
