using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BehaviourTreeNode : ScriptableObject {

	[HideInInspector]
	[SerializeField]
	public Rect rect;
	
	[HideInInspector]
	[SerializeField]
	public int ID;

	
	[HideInInspector]
	[SerializeField]
	public string displayedName;

	public abstract void Init (BehaviourTreeAgent agent);
	public abstract BehaviourTree.Status Tick ();
	public abstract void Kill ();
	public abstract int ChildrenCount ();
	public abstract List<BehaviourTreeNode> GetChildren ();
	public abstract void RemoveChild (BehaviourTreeNode child);
	public abstract void AddChild (BehaviourTreeNode child);
	public abstract void ReplaceChild (BehaviourTreeNode oldChild, BehaviourTreeNode newChild);
}

