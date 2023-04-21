using Extensions;
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
		body.LookAt((Vector2)transform.position + body.velocity, 0.2f);
	}

	private void FixedUpdate()
	{
        if (active) { _State = State.Enemy_Seek; }
        else { _State = State.Self_Idle; return; }
        body.AddForce(Seek() + Flock());
	}

    public void Fire(Transform target) { 
        targetTransform = target;
        targetBody = target.GetComponent<Rigidbody2D>();
        targetPosition = target.position;
        active = true;
    }
    public void Fire(Transform target, List<Rigidbody2D> flock) {
        this.flock = flock; if (flock != null && !flock.Contains(body)) { flock.Add(body); } Fire(target);
    }

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.transform.GetComponentInParent<PlayerController>() && collision.transform.GetComponent<ShapeRenderer>()) {
            collision.transform.GetComponent<ShapeRenderer>().DrawShapeHighlight(Color.red.WithAlpha(0.3f));
			if (flock != null && flock.Contains(body)) { flock.Remove(body); } Destroy(gameObject);
		}
	}

}
