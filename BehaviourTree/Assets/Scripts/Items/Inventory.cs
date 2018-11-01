using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {

	private Dictionary<Item, int> inventory;

	void Awake () {
		this.inventory = new Dictionary<Item, int>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void AddItem (Item newItem, int number = 1) {
		if(!ContainsItem(newItem)) {
			this.inventory[newItem] = number;
		} else {
			this.inventory[newItem] += number;
		}
		
	}

	public void RemoveItem (Item removedItem, int number = 1) {
		if(ContainsItem(removedItem, number)) {
			this.inventory[removedItem] -= number;
			if(this.inventory[removedItem] == 0) {
				this.inventory.Remove(removedItem);
			}
		}
	}

	public void Clear () {
		this.inventory.Clear();
	}

	public bool ContainsItem (Item desiredItem, int number = 1) {
		if( ! this.inventory.ContainsKey(desiredItem)) {
			return false;
		}
		return this.inventory[desiredItem] >= number;
	}

	public int GetItemCount (Item desiredItem) {
		if( ! this.inventory.ContainsKey(desiredItem)) {
			return 0;
		}
		return this.inventory[desiredItem];
	}
}
