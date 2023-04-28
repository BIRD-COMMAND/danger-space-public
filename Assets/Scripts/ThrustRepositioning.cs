using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class ThrustRepositioning : MonoBehaviour
{

	[SerializeField] private Transform shipTransform;
	[SerializeField] private bool flipDirection = false;
	[SerializeField] private float maxAngleOffset = 15f;
	[SerializeField] private float slerpFactor = 0.05f;

	private Vector3 startingPosition;
	private Quaternion startingRotation;

	private void Start()
	{
		shipTransform = transform.parent;
		startingPosition = transform.localPosition;
		startingRotation = transform.localRotation;
	}

	public void ApplyThrust(float thrust)
	{
		
		float angleOffset = maxAngleOffset * thrust * (flipDirection ? -1 : 1);
		Quaternion targetRotation = startingRotation * Quaternion.Euler(0, 0, angleOffset);
		transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, slerpFactor);

		float angleDifference = Vector3.SignedAngle(transform.up, shipTransform.right, Vector3.forward);
		float targetAngle = startingRotation.eulerAngles.z + angleOffset - angleDifference;
		float targetX = startingPosition.x + Mathf.Cos(Mathf.Deg2Rad * targetAngle) * thrust;
		float targetY = startingPosition.y + Mathf.Sin(Mathf.Deg2Rad * targetAngle) * thrust;
		Vector3 targetPosition = new Vector3(targetX, targetY, startingPosition.z);

		transform.localPosition = Vector3.Slerp(transform.localPosition, targetPosition, slerpFactor);

	}

}
