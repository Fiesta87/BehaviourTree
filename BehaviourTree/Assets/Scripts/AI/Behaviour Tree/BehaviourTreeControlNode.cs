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

        switch(this.type) {

            case BehaviourTreeControlNode.Type.SELECTOR: return SelectorTick();

            case BehaviourTreeControlNode.Type.SEQUENCE: return SequenceTick();

            case BehaviourTreeControlNode.Type.PARALLEL: return ParallelTick();

            default: return BehaviourTree.Status.FAILURE;
        }
    }

	public override void Kill () {
        foreach(BehaviourTreeNode child in children) {
            if(this.type == BehaviourTreeControlNode.Type.PARALLEL || this.currentTickedNode == child) {
                child.Kill();
            }
        }
    }

    private BehaviourTree.Status SelectorTick () {

        foreach(BehaviourTreeNode child in children) {

            if(this.startFromFirstNodeEachTick || this.currentTickedNode == null || this.currentTickedNode == child) {
                
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

            if(this.startFromFirstNodeEachTick || this.currentTickedNode == null || this.currentTickedNode == child) {

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

	public override void AddChild (BehaviourTreeNode child) {
        this.children.Add(child);
	}
	public override void ReplaceChild (BehaviourTreeNode oldChild, BehaviourTreeNode newChild) {

		for(int i=0; i<this.children.Count; i++) {

            if(this.children[i] == oldChild) {
                this.children[i] = newChild;
                return;
            }
        }
	}

    public enum Type {
        SELECTOR,
        SEQUENCE,
        PARALLEL
    }
}
