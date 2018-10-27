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

            case BehaviourTreeDecoratorNode.Type.LOSER: return LoserTick();

            case BehaviourTreeDecoratorNode.Type.IGNORE_SUCCESS: return IgnoreSuccessTick();

            case BehaviourTreeDecoratorNode.Type.IGNORE_FAILURE: return IgnoreFailureTick();

            default: return BehaviourTree.Status.FAILURE;
        }
    }

	public override void Kill () {
        this.child.Kill();
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

    private BehaviourTree.Status LoserTick () {
        child.Tick();
        return BehaviourTree.Status.FAILURE;
    }

    private BehaviourTree.Status IgnoreSuccessTick () {
        BehaviourTree.Status result = child.Tick();
        if(result == BehaviourTree.Status.SUCCESS) {
            return BehaviourTree.Status.RUNNING;
        }
        return result;
    }

    private BehaviourTree.Status IgnoreFailureTick () {
        BehaviourTree.Status result = child.Tick();
        if(result == BehaviourTree.Status.FAILURE) {
            return BehaviourTree.Status.RUNNING;
        }
        return result;
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

	public override void AddChild (BehaviourTreeNode child) {
		if(this.child == null) {
			this.child = child;
		}
	}
    
	public override void ReplaceChild (BehaviourTreeNode oldChild, BehaviourTreeNode newChild) {
		this.child = newChild;
	}

    public enum Type {
        INVERTER,
        SUCCEEDER,
        LOSER,
        IGNORE_SUCCESS,
        IGNORE_FAILURE
    }
}
