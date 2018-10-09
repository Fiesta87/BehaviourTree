using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTreeExecutionNode : BehaviourTreeNode {

    public BehaviourTreeTask task;

    private bool firstTick;

    public override void Init () {
        this.firstTick = true;
    }

    public override BehaviourTree.Status Tick() {
        
        BehaviourTree.Status result;

        if(this.firstTick) {
            
            this.firstTick = false;
            
            result = task.Begin();

            if(result != BehaviourTree.Status.RUNNING) {
                return result;
            }
        }

        result = task.Update();

        if(result != BehaviourTree.Status.RUNNING) {
            task.Finish();
        }

        return result;
    }

    public override string GetName () {
        return "Execution : " + this.name;
    }

    public override int ChildrenCount () {
        return 0;
    }

    public override List<BehaviourTreeNode> GetChildren () {
        return new List<BehaviourTreeNode>();
    }
}
