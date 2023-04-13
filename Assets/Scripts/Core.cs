using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;
using Shapes;

[RequireComponent(typeof(Rigidbody2D))]
public class Core : Module
{

	[SerializeField, HideInInspector] private float rotationSpeed = 200.0f;
    public Rigidbody2D body;

	private float movementInput;
	private float rotationInput;

	// Start is called before the first frame update
	void Start() { core = this; body = GetComponent<Rigidbody2D>(); }
	void OnValidate() { Start(); }

	// Update is called once per frame
	void Update() {
		body.drag = Mathf.Lerp(0f, 4f, Pad.LTrigger);
		movementInput = Mathf.Lerp(0f, thrust, Pad.RTrigger);
		rotationInput = Pad.RStick.DeadZone(0.1f).x;
	}


	void FixedUpdate() {
		Move();
		Rotate();
	}

	private void Move() {
		Vector2 force = transform.up * movementInput;
		body.AddForce(force);
	}

	private void Rotate() {
		transform.RotateAround(
			transform.TransformPoint(centroid), 
			Vector3.forward, -rotationInput * rotationSpeed * Time.fixedDeltaTime
		); 
		//body.rotation -= rotation;
	}

	[ContextMenu("Remove Active Modules")]
	public void RemoveActiveModules() { RemoveModules(false); Recalculate(); }
	[ContextMenu("Deactivate Active Modules")]
	public void DeactivateActiveModules() { RemoveModules(true); Recalculate(); }

	[ContextMenu("Add Active Modules")]
	public void AddActiveModules() { AddModules(false); Recalculate(); }
	[ContextMenu("Activate Inactive Modules")]
	public void AddInactiveModules() { AddModules(true); Recalculate(); }


	private void Recalculate()
	{
		// calculate mass
		massCurrent = SumMass(this, mass);
		// calculate centroid
		List<Vector2> centroids = new List<Vector2>();
		foreach (Module module in GetComponentsInChildren<Module>(false)) {
			if (module.effectCenterOfMass && module.ActiveInParent) {
				centroids.Add(transform.InverseTransformPoint(module.transform.TransformPoint(module.centroid)));
			}
		}
		if (centroids.Count == 0) { CalculateCentroid(); }
		else { centroid = centroids.Average(); }
	}

	

	private void OnDrawGizmos()
	{
		Draw.UseDashes = true;
		Draw.Ring(transform.TransformPoint(centroid), 0.001f, Color.red);
	}

}
