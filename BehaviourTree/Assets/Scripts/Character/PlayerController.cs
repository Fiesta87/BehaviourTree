using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///	<summary>
///	This class makes a character controled by the Player.
///	</summary>  
public class PlayerController : MonoBehaviour {

	public static PlayerController Instance;

	private CharacterMovementController characterMovementController;
	private int layerMask;

	void Awake () {
		this.characterMovementController = GetComponent<CharacterMovementController>();
		PlayerController.Instance = this;
	}

	void Start () {
		this.layerMask = 1 << LayerMask.NameToLayer("Player");
		this.layerMask = ~this.layerMask;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButton(0)) {	// mouse left button

			RaycastHit hit;

			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100.0f, this.layerMask)) {	// get clicked point in 3D space
				this.characterMovementController.WalkTo(hit.point);	// walk to that point
			}
		}
	}
}
