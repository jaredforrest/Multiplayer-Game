using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StayAboveTarget : MonoBehaviour
{
	public Transform targetToFollow;

	Vector3 Offset = new Vector3(0f, 1.2f, 0f);

	void Start ()
	{

	}

	void LateUpdate ()
	{
		transform.position = targetToFollow.position + Offset;	
	}
}