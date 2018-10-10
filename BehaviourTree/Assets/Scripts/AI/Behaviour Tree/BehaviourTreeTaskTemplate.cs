using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTreeTaskTemplate : BehaviourTreeTask {
	
    public override void Begin() {
        
    }

    public override BehaviourTree.Status Update() {
        return BehaviourTree.Status.SUCCESS;
    }

    public override void FinishSuccess() {
		
    }

    public override void FinishFailure() {
		
    }
}
