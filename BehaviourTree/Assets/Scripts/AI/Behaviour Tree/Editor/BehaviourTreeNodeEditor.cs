using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BehaviourTree))]
public class BehaviourTreeNodeEditor : Editor {

	public override void OnInspectorGUI() {

		base.OnInspectorGUI();

		BehaviourTree node = (BehaviourTree)target;
	}
}
