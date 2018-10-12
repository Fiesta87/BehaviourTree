using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogTask : BehaviourTreeTask {

    // Use this methode to init your task, this code will be executed only once when starting this task
    public override void Begin () {
        
    }

    // Use this methode to perform your task
    public override BehaviourTree.Status Update () {
        Debug.Log(this.agent.ToString());
        return BehaviourTree.Status.SUCCESS;
    }

    public override void FinishSuccess () {
		
    }

    public override void FinishFailure () {
		
    }
}
