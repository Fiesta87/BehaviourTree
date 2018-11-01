using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(BehaviourTree))]
public class BehaviourTreeNodeEditor : Editor {

	public void OnEnable () {
		Repaint();
	}

	public override void OnInspectorGUI() {
		
		Repaint();

		GUIStyle titleStyle = new GUIStyle();

		titleStyle.fontSize = 20;

		GUIStyle partStyle = new GUIStyle();

		partStyle.fontSize = 15;

		if(BehaviourTreeEditorWindow.Instance == null) {
			return;
		}

		BehaviourTreeNode selectedNode = BehaviourTreeEditorWindow.Instance.selectedNode;

		if(selectedNode == null) {
			return;
		}

		GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label(selectedNode.displayedName, titleStyle);
			GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();

		GUILayout.Space(30);

		if(selectedNode is BehaviourTreeControlNode) {

			BehaviourTreeControlNode node = (BehaviourTreeControlNode)selectedNode;

			GUI.color = Color.white;

			BehaviourTreeControlNode.Type initialTypeValue = node.type;

			BehaviourTreeControlNode.Type typeValue = (BehaviourTreeControlNode.Type)EditorGUILayout.EnumPopup("Type", initialTypeValue);

			if( typeValue != initialTypeValue) {
				node.type = typeValue;
				node.displayedName = typeValue.ToString().Replace('_', ' ');
				BehaviourTreeEditorWindow.SaveNodeAnChildren(node);
				BehaviourTreeEditorWindow.Instance.Repaint();
			}

			if(node.type != BehaviourTreeControlNode.Type.PARALLEL) {
				bool initialMemorizeValue = node.startFromFirstNodeEachTick;

				GUILayout.Space(5);

				GUILayout.BeginHorizontal();

					GUI.color = Color.black;
					GUILayout.Label("Don't memorize running node", GUI.skin.label);

					GUI.color = Color.white;
					bool memorizeValue = EditorGUILayout.Toggle(initialMemorizeValue);
					if( memorizeValue != initialMemorizeValue) {
						node.startFromFirstNodeEachTick = memorizeValue;
						BehaviourTreeEditorWindow.SaveNodeAnChildren(node);
						BehaviourTreeEditorWindow.Instance.Repaint();
					}
					
				GUILayout.EndHorizontal();
			}
		}

		else if (selectedNode is BehaviourTreeExecutionNode) {

			BehaviourTreeExecutionNode node = (BehaviourTreeExecutionNode)selectedNode;

			if(node.task != null) {

				// GUILayout.Space(nodeSize.y * zoomScale);

				PropertyReader.Variable[] variables = PropertyReader.GetFields(node.task.GetType());

				GUILayout.Label("PARAMETER", partStyle);

				foreach(PropertyReader.Variable variable in variables) {
					if(variable.name.StartsWith("param_")) {
						GUILayout.Space(5);
						AddParameterField(node, variable);
					}
				}

				GUILayout.Space(20);

				GUILayout.Label("INPUT", partStyle);

				foreach(PropertyReader.Variable variable in variables) {
					if(variable.name.StartsWith("in_")) {
						GUILayout.Space(5);
						AddContextField(node, variable);
					}
				}

				GUILayout.Space(20);

				GUILayout.Label("OUTPUT", partStyle);

				foreach(PropertyReader.Variable variable in variables) {
					if(variable.name.StartsWith("out_")) {
						GUILayout.Space(5);
						AddContextField(node, variable);
					}
				}
			}
		}

		else if (selectedNode is BehaviourTreeDecoratorNode) {

			BehaviourTreeDecoratorNode node = (BehaviourTreeDecoratorNode)selectedNode;

			GUI.color = Color.white;

			BehaviourTreeDecoratorNode.Type initialTypeValue = node.type;

			BehaviourTreeDecoratorNode.Type typeValue = (BehaviourTreeDecoratorNode.Type)EditorGUILayout.EnumPopup("Type", initialTypeValue);

			if( typeValue != initialTypeValue) {
				node.type = typeValue;
				node.displayedName = typeValue.ToString().Replace('_', ' ');
				BehaviourTreeEditorWindow.SaveNodeAnChildren(node);
				BehaviourTreeEditorWindow.Instance.Repaint();
			}
		}

		else if (selectedNode is BehaviourTreeSubTreeNode) {

			BehaviourTreeSubTreeNode node = (BehaviourTreeSubTreeNode)selectedNode;

			GUI.color = Color.white;

			BehaviourTree initialValue = node.subTree;

			BehaviourTree newValue = (BehaviourTree)EditorGUILayout.ObjectField(node.subTree, typeof(BehaviourTree), false);

			if( newValue != initialValue) {
				node.subTree = newValue;
				node.displayedName = newValue.ToString().Replace('_', ' ').Split('(')[0];
				BehaviourTreeEditorWindow.SaveNodeAnChildren(node);
				BehaviourTreeEditorWindow.Instance.Repaint();
			}
		}
	}

	void AddContextField (BehaviourTreeExecutionNode node, PropertyReader.Variable variable) {
		
		GUILayout.BeginHorizontal();

			try {

				if( ! node.contextLink.ContainsKey(variable.name)) {
					node.contextLink[variable.name] = variable.name.Split('_')[1];
				}
				string initialValue = node.contextLink[variable.name];

				GUI.color = Color.black;
				GUILayout.Label(variable.name.Split('_')[1], GUI.skin.label);

				GUI.color = Color.white;
				string value = EditorGUILayout.TextField(initialValue);

				node.contextLink[variable.name] = value;

				if( value != initialValue) {
					BehaviourTreeEditorWindow.SaveNodeAnChildren(node);
				}
			} catch (KeyNotFoundException e) {
				Debug.LogError("the key " + variable.name + " is not in the task context link array.");
				Debug.LogException(e);
			}
			
			
		GUILayout.EndHorizontal();
	}

	void AddParameterField (BehaviourTreeExecutionNode node, PropertyReader.Variable variable) {
		
		GUILayout.BeginHorizontal();

			object initialValue = PropertyReader.GetValue(node.task, variable.name);

			GUI.color = Color.black;
			GUILayout.Label(variable.name.Split('_')[1], GUI.skin.label);

			GUI.color = Color.white;

			object value = null;

			if(variable.type == typeof(string)) value = EditorGUILayout.TextField((string)initialValue);
			else if(variable.type == typeof(float)) value = EditorGUILayout.FloatField((float)initialValue);
			else if(variable.type == typeof(int)) value = EditorGUILayout.IntField((int)initialValue);
			else if(variable.type == typeof(double)) value = EditorGUILayout.DoubleField((double)initialValue);
			else if(variable.type == typeof(bool)) value = EditorGUILayout.Toggle((bool)initialValue);
			
			if(value == null) {
				GUILayout.EndHorizontal();
				return;
			}

			if( value != initialValue) {

				SerializedObject taskSerializedAsset = new SerializedObject(node.task);

				taskSerializedAsset.Update();

				PropertyReader.SetValue(node.task, variable.name, value);

				SerializedProperty p = taskSerializedAsset.FindProperty(variable.name);

				if(value is string) p.stringValue = (string)value;
				else if(value is float) p.floatValue = (float)value;
				else if(value is int) p.intValue = (int)value;
				else if(value is double) p.doubleValue = (double)value;
				else if(value is bool) p.boolValue = (bool)value;

				taskSerializedAsset.ApplyModifiedProperties();
				
				BehaviourTreeEditorWindow.SaveNodeAnChildren(node);
			}
			
		GUILayout.EndHorizontal();
	}
}
