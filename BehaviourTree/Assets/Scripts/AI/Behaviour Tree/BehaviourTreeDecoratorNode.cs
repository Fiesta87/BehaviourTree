using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTreeDecoratorNode : BehaviourTreeNode {

    [HideInInspector]
    [SerializeField]
    public BehaviourTreeDecoratorNode.Type type;

    [HideInInspector]
    [SerializeField]
   public BehaviourTreeNode child;

    public override void Init (BehaviourTreeAgent agent) {
        if(child != null) {
			child.Init(agent);
		}
    }

    public override BehaviourTree.Status Tick () {

        switch(this.type) {

            case BehaviourTreeDecoratorNode.Type.INVERTER: return InverterTick();

            case BehaviourTreeDecoratorNode.Type.SUCCEEDER: return SucceederTick();

            default: return BehaviourTree.Status.FAILURE;
        }
    }

    private BehaviourTree.Status InverterTick () {

        BehaviourTree.Status result = child.Tick();

        if(result == BehaviourTree.Status.SUCCESS) {
            return BehaviourTree.Status.FAILURE;
        }
        
        if (result == BehaviourTree.Status.FAILURE) {
            return BehaviourTree.Status.SUCCESS;
        }

        return BehaviourTree.Status.RUNNING;
    }

    private BehaviourTree.Status SucceederTick () {
        child.Tick();
        return BehaviourTree.Status.SUCCESS;
    }

    public override int ChildrenCount () {
        return this.child != null ? 1 : 0;
    }

    public override List<BehaviourTreeNode> GetChildren () {

		List<BehaviourTreeNode> l = new List<BehaviourTreeNode>();
		if(child != null) {
			l.Add(child);
		}
        return l;
    }

	public override void RemoveChild (BehaviourTreeNode child) {
        this.child = null;
    }

    public enum Type {
        INVERTER,
        SUCCEEDER
    }
}
