using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BehaviourTreeSubTreeNode : BehaviourTreeNode {

    [HideInInspector]
    [SerializeField]
    public BehaviourTree subTree;

    [NonSerialized]
    private BehaviourTreeAgent agent;

    public override void Init (BehaviourTreeAgent agent) {
        this.agent = agent;
        this.subTree.Init(agent);
    }
    
    public override BehaviourTree.Status Tick() {

        return this.subTree.Tick();
    }

	public override void Kill () {
        this.subTree.Kill();
    }

    public override int ChildrenCount () {
        return 0;
    }

    public override List<BehaviourTreeNode> GetChildren () {
        return new List<BehaviourTreeNode>();
    }

    public override void RemoveChild (BehaviourTreeNode child) {
        
    }

	public override void AddChild (BehaviourTreeNode child) {
		
	}
	public override void ReplaceChild (BehaviourTreeNode oldChild, BehaviourTreeNode newChild) {
		
	}
}
