using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTreeTaskTemplate : BehaviourTreeTask {
	
    public override BehaviourTree.Status Begin() {
        return BehaviourTree.Status.SUCCESS;
    }

    public override BehaviourTree.Status Update() {
        return BehaviourTree.Status.SUCCESS;
    }

    public override void Finish() {
		
    }
}
