using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public static PlayerController Instance;

	private CharacterMovementController characterMovementController;

	void Awake () {
		this.characterMovementController = GetComponent<CharacterMovementController>();
		PlayerController.Instance = this;
	}

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButton(0)) {

			RaycastHit hit;

			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100.0f)) {
				this.characterMovementController.WalkTo(hit.point);
			}
		}
	}
}
