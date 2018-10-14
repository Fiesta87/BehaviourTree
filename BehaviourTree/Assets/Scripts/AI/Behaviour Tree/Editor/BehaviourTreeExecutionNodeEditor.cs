using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BehaviourTreeExecutionNode))]
public class BehaviourTreeExecutionNodeEditor : Editor {

	// public Object previousTaskScript;

	public override void OnInspectorGUI() {

		base.OnInspectorGUI();

		BehaviourTreeExecutionNode node = (BehaviourTreeExecutionNode)target;

		// if(previousTaskScript != node.taskScriptAsset) {
		// 	previousTaskScript = node.taskScriptAsset;

		// 	BehaviourTreeEditorWindow.SaveBehaviourTree();
		// }

		// DragAndDrop.visualMode = DragAndDropVisualMode.Generic;

		// Event current = Event.current;

		// if(current.type.Equals(EventType.DragExited)) {

		// 	Object taskScriptAsset = DragAndDrop.objectReferences[0];
			
        //     node.task = ScriptableObject.CreateInstance((taskScriptAsset as MonoScript).GetClass()) as BehaviourTreeTask;

		// 	BehaviourTreeEditorWindow.SaveBehaviourTree();

		// 	current.Use();
		// }
	}
}
