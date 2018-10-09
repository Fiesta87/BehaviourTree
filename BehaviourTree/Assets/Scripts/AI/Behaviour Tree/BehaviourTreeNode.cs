using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BehaviourTreeNode : ScriptableObject {

	public abstract void Init ();
	public abstract BehaviourTree.Status Tick ();

	public abstract string GetName ();
}

