using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTreeAgent : MonoBehaviour {

	public BehaviourTree behaviourTree;

	private Dictionary<string, object> context;

	void Awake () {
		this.context = new Dictionary<string, object>();
	}

	void Start () {
		if(this.behaviourTree != null){
			this.behaviourTree.Init(this);
		}
	}
	
	void Update () {
		if(this.behaviourTree != null){
			// Debug.Log(this.behaviourTree.Tick());
			this.behaviourTree.Tick();
		}
	}

	public Dictionary<string, object> GetContext () {
		return this.context;
	}
}
