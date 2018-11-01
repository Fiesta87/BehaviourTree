using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour, IInteractive {

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Interact(GameObject actor) {

        actor.GetComponent<Inventory>().AddItem(Item.WOOD);
		Debug.Log(actor.name + " chopped 1 " + Item.WOOD);
    }

	public GameObject GetGameObject () {
		return this.gameObject;
	}
}
