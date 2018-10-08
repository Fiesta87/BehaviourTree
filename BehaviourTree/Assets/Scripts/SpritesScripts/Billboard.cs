using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Billboard : MonoBehaviour {

	void Awake () {

		SpriteRenderer sr = GetComponent<SpriteRenderer>();
		
		sr.shadowCastingMode = ShadowCastingMode.On;
		sr.receiveShadows = true;
	}
	void Start () {
		this.transform.forward = Camera.main.transform.forward;
	}
}
