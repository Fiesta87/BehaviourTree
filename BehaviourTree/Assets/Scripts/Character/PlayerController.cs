using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///	<summary>
///	This class makes a character controled by the Player.
///	</summary>  
public class PlayerController : MonoBehaviour {

	public static PlayerController Instance;

	private CharacterMovementController characterMovementController;
	private int layerMaskAllExceptPlayer;
	private int layerMaskInteractive;

	void Awake () {
		this.characterMovementController = GetComponent<CharacterMovementController>();
		PlayerController.Instance = this;
	}

	void Start () {
		this.layerMaskAllExceptPlayer = 1 << LayerMask.NameToLayer("Player");
		this.layerMaskAllExceptPlayer = ~this.layerMaskAllExceptPlayer;

		this.layerMaskInteractive = LayerMask.NameToLayer("Interactive");
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0)) {	// mouse left button
			
			RaycastHit hit;

			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100.0f, this.layerMaskAllExceptPlayer)) {	// get clicked point in 3D space

				if(hit.transform.gameObject.layer == this.layerMaskInteractive) {
					InteractWith(hit.transform.gameObject.GetComponent<IInteractive>());
				} else {
					this.characterMovementController.WalkTo(hit.point);	// walk to that point
				}
			}
		}
	}

	private void InteractWith (IInteractive obj) {
		
		if(obj == null) {
			return;
		}

		GameObject go = obj.GetGameObject();

		if(this.characterMovementController.DistanceTo(go.transform.position) > 3.0f) {
			this.characterMovementController.WalkTo(go.transform.position);	// walk to that point
		} else {
			obj.Interact(this.gameObject);
		}
	}
}
