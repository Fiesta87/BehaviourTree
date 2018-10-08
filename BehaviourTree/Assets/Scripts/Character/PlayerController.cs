using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	private CharacterMovementController characterMovementController;

	void Awake () {
		this.characterMovementController = GetComponent<CharacterMovementController>();
	}

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0)) {

			RaycastHit hit;
			
			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit)) {
				this.characterMovementController.WalkTo(hit.point);
			}
		}
	}
}
