using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractTask : BehaviourTreeTask {

    public IInteractive in_interactiveObject;

    // Use this methode to setup the object link for your task, this code will be executed only once when the BehaviourTreeAgent Start methode is called.
    // eg. this.myCustomMonoBehaviourScript = this.agent.GetComponent<CustomMonoBehaviourScript>();
    public override void InitOnStart () {

    }
	
    // Use this methode to init your task, this code will be executed every time a parent start this task.
    public override void Begin () {
        
    }

    // Use this methode to perform your task
    public override BehaviourTree.Status Update () {
        in_interactiveObject.Interact(this.agent.gameObject);
        return BehaviourTree.Status.RUNNING;
    }

    public override void FinishSuccess () {
		
    }

    public override void FinishFailure () {
		
    }

	public override void Kill () {
        
    }
}
