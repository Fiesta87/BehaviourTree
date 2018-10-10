using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BehaviourTreeControlNode))]
public class BehaviourTreeControlNodeEditor : Editor {

	public override void OnInspectorGUI() {

		base.OnInspectorGUI();

		BehaviourTreeControlNode node = (BehaviourTreeControlNode)target;

		
	}
}
