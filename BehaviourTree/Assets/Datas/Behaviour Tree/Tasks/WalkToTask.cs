using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkToTask : BehaviourTreeTask {

    public Vector3 in_postion;

    private CharacterMovementController characterMovementController;
    private bool canWalkToDestination;

    // Use this methode to setup the object link for your task, this code will be executed only once when the BehaviourTreeAgent Start methode is called.
    // eg. this.myCustomMonoBehaviourScript = this.agent.GetComponent<CustomMonoBehaviourScript>();
    public override void InitOnStart () {
        this.characterMovementController = agent.GetComponent<CharacterMovementController>();
    }
	
    // Use this methode to init your task, this code will be executed every time a parent start this task.
    public override void Begin () {
        this.canWalkToDestination = this.characterMovementController.WalkTo(in_postion);
    }

    // Use this methode to perform your task
    public override BehaviourTree.Status Update () {
        
        if(!canWalkToDestination) {
            return BehaviourTree.Status.FAILURE;
        }

        if(this.characterMovementController.IsAtDestination()) {
            return BehaviourTree.Status.SUCCESS;
        }

        return BehaviourTree.Status.RUNNING;
    }

    public override void FinishSuccess () {
		
    }

    public override void FinishFailure () {
		
    }
}
