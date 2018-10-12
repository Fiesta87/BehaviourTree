using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BehaviourTreeNode : ScriptableObject {

	[HideInInspector]
	public Rect rect;
	[HideInInspector]
	[SerializeField]
	public int ID;
	[SerializeField]
	public string displayedName;

	public abstract void Init (BehaviourTreeAgent agent);
	public abstract BehaviourTree.Status Tick ();
	public abstract int ChildrenCount ();
	public abstract List<BehaviourTreeNode> GetChildren ();
	public abstract void RemoveChild (BehaviourTreeNode child);
}

