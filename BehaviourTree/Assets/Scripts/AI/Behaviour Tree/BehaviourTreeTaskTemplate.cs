using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTreeTaskTemplate : BehaviourTreeTask {
	
    // Use this methode to init your task, this code will be executed only once when starting this task
    public override void Begin () {
        
    }

    // Use this methode to perform your task
    public override BehaviourTree.Status Update () {
        return BehaviourTree.Status.SUCCESS;
    }

    public override void FinishSuccess () {
		
    }

    public override void FinishFailure () {
		
    }
}
