using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTreeControlNode : BehaviourTreeNode {

    [HideInInspector]
    [SerializeField]
    public BehaviourTreeControlNode.Type type;

    [HideInInspector]
    [SerializeField]
    public List<BehaviourTreeNode> children;

    [HideInInspector]
    [SerializeField]
    public bool startFromFirstNodeEachTick = false;

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

        if(this.type == BehaviourTreeControlNode.Type.PARALLEL) {
            return ParallelTick();
        }

        if(this.startFromFirstNodeEachTick) {
            switch(this.type) {

                case BehaviourTreeControlNode.Type.SELECTOR: return SelectorTickFromFirstNode();

                case BehaviourTreeControlNode.Type.SEQUENCE: return SequenceTickFromFirstNode();

                default: return BehaviourTree.Status.FAILURE;
            }
        } else {
            switch(this.type) {

                case BehaviourTreeControlNode.Type.SELECTOR: return SelectorTick();

                case BehaviourTreeControlNode.Type.SEQUENCE: return SequenceTick();

                default: return BehaviourTree.Status.FAILURE;
            }
        }
    }

    private BehaviourTree.Status SelectorTickFromFirstNode () {

        foreach(BehaviourTreeNode child in children) {

            BehaviourTree.Status childStatus = child.Tick();

            if(childStatus != BehaviourTree.Status.FAILURE) {
                return childStatus;
            }
        }

        return BehaviourTree.Status.FAILURE;
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

    private BehaviourTree.Status SequenceTickFromFirstNode () {

        foreach(BehaviourTreeNode child in children) {

            BehaviourTree.Status childStatus = child.Tick();

            if(childStatus != BehaviourTree.Status.SUCCESS) {
                return childStatus;
            }
        }

        return BehaviourTree.Status.SUCCESS;
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

    private BehaviourTree.Status ParallelTick () {
        
        bool firstChild = true;

        foreach(BehaviourTreeNode child in children) {
            
            if(firstChild) {
                firstChild = false;
                BehaviourTree.Status firstChildStatus = child.Tick();

                if(firstChildStatus != BehaviourTree.Status.RUNNING) {
                    return firstChildStatus;
                }
            }
            else {
                child.Tick();
            }
        }

        return BehaviourTree.Status.RUNNING;
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
        SEQUENCE,
        PARALLEL
    }
}
