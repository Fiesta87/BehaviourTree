using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkToTask : BehaviourTreeTask {

    public float param_distance = 1.5f;
    public Vector3 in_position;

    private CharacterMovementController characterMovementController;
    private bool canWalkToDestination;
    private bool isAtDestination;

    // Use this methode to setup the object link for your task, this code will be executed only once when the BehaviourTreeAgent Start methode is called.
    // eg. this.myCustomMonoBehaviourScript = this.agent.GetComponent<CustomMonoBehaviourScript>();
    public override void InitOnStart () {
        this.characterMovementController = agent.GetComponent<CharacterMovementController>();
    }
	
    // Use this methode to init your task, this code will be executed every time a parent start this task.
    public override void Begin () {

        this.isAtDestination = this.characterMovementController.DistanceTo(in_position) <= param_distance;

        if(!isAtDestination) {
            this.canWalkToDestination = this.characterMovementController.WalkTo(in_position);
        }
    }

    // Use this methode to perform your task
    public override BehaviourTree.Status Update () {

        if(isAtDestination) {
            return BehaviourTree.Status.SUCCESS;
        }
        
        if(!canWalkToDestination) {
            return BehaviourTree.Status.FAILURE;
        }

        if(this.characterMovementController.IsAtDestination(param_distance)) {
            this.characterMovementController.Stop();
            return BehaviourTree.Status.SUCCESS;
        }

        return BehaviourTree.Status.RUNNING;
    }

    public override void FinishSuccess () {
		
    }

    public override void FinishFailure () {
		
    }

	public override void Kill () {
        this.characterMovementController.Stop();
    }
}
