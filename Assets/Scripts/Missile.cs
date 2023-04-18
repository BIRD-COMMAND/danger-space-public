using Shapes;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Missile : AI
{

    bool active = false;

	private void Update()
	{

		// Look in the direction of travel
		transform.rotation = Quaternion.Slerp(
			transform.rotation,
			Quaternion.FromToRotation(transform.up, ((Vector2)transform.position + rb.velocity).normalized) * transform.rotation,
			0.1f
		);

	}

	private void FixedUpdate()
	{
        if (active) { _State = State.Enemy_Seek; }
        else { _State = State.Self_Idle; return; }
        rb.AddForce(Seek() + Flock());
	}

    public void Fire(Transform target) { 
        targetTransform = target;
        targetBody = target.GetComponent<Rigidbody2D>();
        targetPosition = target.position;
        active = true;
    }
    public void Fire(Transform target, List<Rigidbody2D> flock) {
        this.flock = flock; if (!flock.Contains(rb)) { flock.Add(rb); } Fire(target);
    }

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.transform.GetComponentInParent<PlayerController>() && collision.transform.GetComponent<ShapeRenderer>()) {
            collision.transform.GetComponent<ShapeRenderer>().DrawShapeHighlight(Color.red.WithAlpha(0.3f));
			if (flock != null && flock.Contains(rb)) { flock.Remove(rb); } Destroy(gameObject);
		}
	}

}
