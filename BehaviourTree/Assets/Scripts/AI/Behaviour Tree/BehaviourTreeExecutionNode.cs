using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTreeExecutionNode : BehaviourTreeNode {

    [SerializeField]
    public BehaviourTreeTask task;

    private bool firstTick;

    public override void Init () {
        this.firstTick = true;
    }

    public override BehaviourTree.Status Tick() {
        
        BehaviourTree.Status result;

        if(this.firstTick) {
            
            this.firstTick = false;
            
            task.Begin();
        }

        result = task.Update();

        if(result == BehaviourTree.Status.SUCCESS) {
            task.FinishSuccess();
        } else if(result == BehaviourTree.Status.FAILURE) {
            task.FinishFailure();
        }

        return result;
    }

    public override int ChildrenCount () {
        return 0;
    }

    public override List<BehaviourTreeNode> GetChildren () {
        return new List<BehaviourTreeNode>();
    }

    public override void RemoveChild (BehaviourTreeNode child) {
        
    }
}
