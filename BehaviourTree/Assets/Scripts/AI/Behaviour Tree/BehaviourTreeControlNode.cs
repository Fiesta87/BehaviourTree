using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTreeControlNode : BehaviourTreeNode {

    public BehaviourTreeControlNode.Type type;

    public List<BehaviourTreeNode> children;

    public override void Init () {
        foreach(BehaviourTreeNode child in children) {

            child.Init();
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

            BehaviourTree.Status childStatus = child.Tick();

            if(childStatus != BehaviourTree.Status.SUCCESS) {
                return childStatus;
            }
        }

        return BehaviourTree.Status.SUCCESS;
    }

    private BehaviourTree.Status SequenceTick () {

        foreach(BehaviourTreeNode child in children) {

            BehaviourTree.Status childStatus = child.Tick();

            if(childStatus != BehaviourTree.Status.FAILURE) {
                return childStatus;
            }
        }

        return BehaviourTree.Status.FAILURE;
    }

    public override string GetName () {
        switch(this.type) {

            case BehaviourTreeControlNode.Type.SELECTOR: return "Selector : " + this.name;

            case BehaviourTreeControlNode.Type.SEQUENCE: return "Sequence : " + this.name;

            default: return "Control : " + this.name;
        }
    }

    public override int ChildrenCount () {
        return children.Count;
    }

    public override List<BehaviourTreeNode> GetChildren () {
        return this.children;
    }

    public enum Type {
        SELECTOR,
        SEQUENCE
    }
}
