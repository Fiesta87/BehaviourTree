using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTreeAgent : MonoBehaviour {

	public BehaviourTree behaviourTree;
	public string test;

	// private List<BehaviourTreeTask> tasks;

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
}
