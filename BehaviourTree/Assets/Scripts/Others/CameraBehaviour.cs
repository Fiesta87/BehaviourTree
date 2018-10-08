using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///	<summary>
///	This class move the camera smoothly to track the player with a zoom/dezoom on mouse wheel.
///	</summary>  
public class CameraBehaviour : MonoBehaviour {

	public float minDezoom = 0.4f;
	public float maxDezoom = 2.0f;
    public float smoothTime = 0.3f;
	private Vector3 positionRelativeToPlayer;
    private Vector3 velocity = Vector3.zero;
	private float dezoom = 1.0f;

	void Start () {
		this.positionRelativeToPlayer = this.transform.position - PlayerController.Instance.transform.position;
	}
	
	void LateUpdate () {

		this.dezoom -= Input.GetAxis("Mouse ScrollWheel");

		if (Input.GetMouseButtonDown(2)) {
			this.dezoom = 1.0f;
		}

		this.dezoom = Mathf.Clamp(this.dezoom, minDezoom, maxDezoom);

		this.transform.position = Vector3.SmoothDamp(this.transform.position, PlayerController.Instance.transform.position + this.positionRelativeToPlayer * this.dezoom, ref velocity, smoothTime);
	}
}
