using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Behaviour Tree", menuName = "Behaviour Tree/Behaviour Tree", order = 1)]
public class BehaviourTree : BehaviourTreeNode {
	
	[HideInInspector]
	[SerializeField]
	public BehaviourTreeNode child;
	[HideInInspector]
	[SerializeField]
	public int nextWindowID = 1;

    public override void Init (BehaviourTreeAgent agent) {
		if(child != null) {
			child.Init(agent);
		}
    }

    public override BehaviourTree.Status Tick () {
        return child.Tick();
    }

	public override void Kill () {
        this.child.Kill();
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

	public enum Status {
		RUNNING,
		SUCCESS,
		FAILURE
	}
}