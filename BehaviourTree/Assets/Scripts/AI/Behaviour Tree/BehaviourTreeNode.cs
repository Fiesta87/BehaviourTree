using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BehaviourTreeNode : ScriptableObject {

	public Rect rect;
	public int ID;

	public abstract void Init ();
	public abstract BehaviourTree.Status Tick ();
	public abstract int ChildrenCount ();
	public abstract List<BehaviourTreeNode> GetChildren ();
	public abstract void RemoveChild (BehaviourTreeNode child);
}

