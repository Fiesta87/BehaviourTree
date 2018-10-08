using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

///	<summary>
///	This class makes sure the sprite always face the camera (billboard) and makes it cast and receve shadow.
///	</summary>  
public class Billboard : MonoBehaviour {

	void Awake () {

		SpriteRenderer sr = GetComponent<SpriteRenderer>();
		
		sr.shadowCastingMode = ShadowCastingMode.On;
		sr.receiveShadows = true;
	}
	void Update () {
		this.transform.forward = Camera.main.transform.forward;	// face the camera
	}
}
