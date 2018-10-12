using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEditor;

public class BehaviourTreeExecutionNode : BehaviourTreeNode {

    // [SerializeField]
    // public Object taskScriptAsset;

    [SerializeField]
    // [HideInInspector]
    public BehaviourTreeTask task;

    private bool firstTick;

    public override void Init (BehaviourTreeAgent agent) {
        this.firstTick = true;
        this.task.Init(agent);
    }
    
    public override BehaviourTree.Status Tick() {
        
        BehaviourTree.Status result;

        if(this.firstTick) {
            
            this.firstTick = false;
            
            this.task.Begin();
        }

        result = this.task.Update();

        if(result == BehaviourTree.Status.SUCCESS) {
            this.task.FinishSuccess();
        } else if(result == BehaviourTree.Status.FAILURE) {
            this.task.FinishFailure();
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
