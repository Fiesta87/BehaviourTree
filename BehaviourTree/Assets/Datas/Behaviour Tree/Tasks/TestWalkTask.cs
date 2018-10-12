using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestWalkTask : BehaviourTreeTask {

    private CharacterMovementController characterMovementController;
    private bool canWalkToDestination;
	
    // Use this methode to init your task, this code will be executed only once when starting this task
    public override void Begin () {
        this.characterMovementController = agent.GetComponent<CharacterMovementController>();
        this.canWalkToDestination = this.characterMovementController.WalkTo(new Vector3(-2.5f, 0.0f, 0.0f));
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
