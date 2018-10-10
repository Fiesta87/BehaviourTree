using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BehaviourTreeExecutionNode))]
public class BehaviourTreeExecutionNodeEditor : Editor {

	public override void OnInspectorGUI() {

		base.OnInspectorGUI();

		BehaviourTreeExecutionNode node = (BehaviourTreeExecutionNode)target;

		
	}
}
