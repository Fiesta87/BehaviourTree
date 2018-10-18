using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTreeControlNode : BehaviourTreeNode {

    [SerializeField]
    public BehaviourTreeControlNode.Type type;

    [HideInInspector]
    [SerializeField]
    public List<BehaviourTreeNode> children;

    private BehaviourTreeNode currentTickedNode;

    public BehaviourTreeControlNode () {
        this.children = new List<BehaviourTreeNode>();
    }

    public override void Init (BehaviourTreeAgent agent) {

        this.currentTickedNode = null;

        foreach(BehaviourTreeNode child in children) {

            child.Init(agent);
        }
    }

    public override BehaviourTree.Status Tick () {

        switch(this.type) {

            case BehaviourTreeControlNode.Type.SELECTOR: return SelectorTick();

            case BehaviourTreeControlNode.Type.SEQUENCE: return SequenceTick();

            default: return BehaviourTree.Status.FAILURE;
        }
    }

    private BehaviourTree.Status SelectorTick () {

        foreach(BehaviourTreeNode child in children) {

            if(this.currentTickedNode == null || this.currentTickedNode == child) {
                
                BehaviourTree.Status childStatus = child.Tick();

                if(childStatus == BehaviourTree.Status.RUNNING) {
                    this.currentTickedNode = child;
                    return childStatus;
                }

                if(childStatus == BehaviourTree.Status.SUCCESS) {

                    this.currentTickedNode = null;
                    return childStatus;
                }
            }
        }

        this.currentTickedNode = null;

        return BehaviourTree.Status.FAILURE;
    }

    private BehaviourTree.Status SequenceTick () {

        foreach(BehaviourTreeNode child in children) {

            if(this.currentTickedNode == null || this.currentTickedNode == child) {

                BehaviourTree.Status childStatus = child.Tick();

                if(childStatus == BehaviourTree.Status.RUNNING) {
                    this.currentTickedNode = child;
                    return childStatus;
                }

                if(childStatus == BehaviourTree.Status.FAILURE) {
                    this.currentTickedNode = null;
                    return childStatus;
                }
            }
        }

        this.currentTickedNode = null;

        return BehaviourTree.Status.SUCCESS;
    }

    public override int ChildrenCount () {
        return children.Count;
    }

    public override List<BehaviourTreeNode> GetChildren () {
        return this.children;
    }

    public override void RemoveChild (BehaviourTreeNode child) {
        this.children.Remove(child);
    }

    public enum Type {
        SELECTOR,
        SEQUENCE
    }
}
