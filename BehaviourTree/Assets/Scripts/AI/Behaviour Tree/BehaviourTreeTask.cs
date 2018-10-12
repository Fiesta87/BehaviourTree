using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is an executable task in a behaviour tree.
/// </summary>
public abstract class BehaviourTreeTask : ScriptableObject {

    protected BehaviourTreeAgent agent;

    public void Init (BehaviourTreeAgent agent) {
        Debug.Log("BehaviourTreeTask Init");
        this.agent = agent;
    }

	public abstract void Begin ();
	public abstract BehaviourTree.Status Update ();
	public abstract void FinishSuccess ();
	public abstract void FinishFailure ();
}
