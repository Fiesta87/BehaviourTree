using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTreeAgent : MonoBehaviour {

	public BehaviourTree behaviourTree;

	private BehaviourTree runningBehaviourTree;
	private Dictionary<string, object> context;

	void Awake () {
		this.context = new Dictionary<string, object>();
		if(this.behaviourTree != null) {
			this.runningBehaviourTree = BehaviourTreeCopier.Copy(this.behaviourTree);
		}
	}

	void Start () {
		if(this.runningBehaviourTree != null) {
			this.runningBehaviourTree.Init(this);
		}
	}
	
	void Update () {
		if(this.runningBehaviourTree != null) {
			if(this.runningBehaviourTree.Tick() == BehaviourTree.Status.FAILURE) {
				Debug.Log(this.gameObject.name + " behaviour : " + this.runningBehaviourTree.name + " return FAILURE.");
			}
		}
	}

	public Dictionary<string, object> GetContext () {
		return this.context;
	}
}
