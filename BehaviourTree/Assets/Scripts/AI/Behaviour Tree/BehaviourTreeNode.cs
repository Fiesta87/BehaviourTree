using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BehaviourTreeNode : ScriptableObject {

	public Rect rect;

	public abstract void Init ();
	public abstract BehaviourTree.Status Tick ();
	public abstract string GetName ();
	public abstract int ChildrenCount ();
	public abstract List<BehaviourTreeNode> GetChildren ();
}

