using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;

[CreateAssetMenu(fileName = "New Behaviour Tree", menuName = "Behaviour Tree/Behaviour Tree", order = 1)]
public class BehaviourTree : BehaviourTreeNode {

	public BehaviourTreeNode child;

	[OnOpenAsset(1)]
    public static bool OnOpenAsset (int instanceID, int line) {

		BehaviourTree behaviourTree = Selection.activeObject as BehaviourTree;

		if (behaviourTree != null) {

			BehaviourTreeEditor editor = EditorWindow.GetWindow<BehaviourTreeEditor>("BehaviourTree");

			editor.init(behaviourTree);

			return true; //catch open file
		}        

		return false; // let unity open the file
    }

    public override void Init () {
        child.Init();
    }

    public override BehaviourTree.Status Tick () {
        return child.Tick();
    }

	public override string GetName () {
        return this.name;
    }

	public enum Status {
	RUNNING,
	SUCCESS,
	FAILURE
}
}

public class BehaviourTreeEditor : EditorWindow {

	private BehaviourTree behaviourTree;

	private BehaviourTreeNode newNode;
	private BehaviourTreeNode newNodeParent;

	[MenuItem("Window/Behaviour Tree")]
	public static void ShowWindow () {
		GetWindow<BehaviourTreeEditor>("BehaviourTree");
	}
	
	public void init (BehaviourTree behaviourTree) {

		this.behaviourTree = behaviourTree;

		this.newNode = null;

		if(this.behaviourTree != null) {

			string filePath = "Assets/Datas/Behaviour Tree/" + this.behaviourTree.GetName();
			try {
				if (!Directory.Exists(filePath)) {
					Directory.CreateDirectory(filePath);
					AssetDatabase.Refresh();
				}
			}
			catch (IOException ex) {
				Debug.Log(ex.Message);
			}
		}
	}

	public static bool IsMouseOverWindow(EditorWindow window) {
    	return mouseOverWindow && window.GetType().FullName == mouseOverWindow.ToString().Split('(', ')')[1];
	}

	void OnGUI () {

		if(this.behaviourTree != null) {

			Handles.BeginGUI();

		/*	Handles.DrawBezier(root.center, 
			windowRect2.center, 
			new Vector2(root.xMax + 50f,root.center.y), 
			new Vector2(windowRect2.xMin - 50f,windowRect2.center.y),
			Color.red,
			null,
			5f);*/

			Handles.EndGUI();

			BeginWindows();

			Rect root = new Rect (this.position.width / 2 - 90, 10, 180, 70);
			root = GUI.Window(0, root, WindowFunction, this.behaviourTree.GetName());

			if(this.newNode != null) {
				Rect newNodeRect = new Rect (this.position.width / 2 - 90, 330, 180, 70);
				newNodeRect = GUI.Window (1, newNodeRect, WindowFunction, this.newNode.GetName());
			}
			
			if(this.behaviourTree.child != null) {
				Rect child1 = new Rect (this.position.width / 2 - 90, 170, 180, 70);
				child1 = GUI.Window (2, child1, WindowFunction, this.behaviourTree.child.GetName());
			}

			
			//windowRect2 = GUI.Window (1, windowRect2, WindowFunction, "Box2");

			EndWindows();
		}
    }

	void AddNodeToAssets(BehaviourTreeNode node) {
		AssetDatabase.CreateAsset(node, "Assets/Datas/Behaviour Tree/" + this.behaviourTree.name + "/" + node.name + ".asset");
		AssetDatabase.Refresh();
	}

	void SelectorCallback () {
		BehaviourTreeControlNode asset = (BehaviourTreeControlNode)ScriptableObject.CreateInstance("BehaviourTreeControlNode");
		asset.type = BehaviourTreeControlNode.Type.SELECTOR;
		asset.name = "New Selector Node";
		this.newNode = asset;
	}

	void SequenceCallback () {
		BehaviourTreeControlNode asset = (BehaviourTreeControlNode)ScriptableObject.CreateInstance("BehaviourTreeControlNode");
		asset.type = BehaviourTreeControlNode.Type.SEQUENCE;
		asset.name = "New Sequence Node";
		this.newNode = asset;
	}

	void ExecutionCallback () {
		Debug.Log("ExecutionCallback");
	}

	void SubTreeCallback () {
		Debug.Log("SubTreeCallback");
	}

    void RootWindowFunction (int windowID) {

		Event current = Event.current;
		if(current.type == EventType.MouseDown && current.button == 1) {

			this.newNodeParent = this.behaviourTree;
			
			GenericMenu menu = new GenericMenu();

			//menu.AddDisabledItem(new GUIContent("I clicked on a thing"));
			menu.AddItem(new GUIContent("New Selector Node"), false, SelectorCallback);
			menu.AddItem(new GUIContent("New Sequence Node"), false, SequenceCallback);
			menu.AddItem(new GUIContent("New Execution Node"), false, ExecutionCallback);
			menu.ShowAsContext();
			
			current.Use();
		}
		
        GUI.DragWindow();
    }

    void WindowFunction (int windowID) {

		Event current = Event.current;
		if(current.type == EventType.MouseDown && current.button == 1) {
			
			GenericMenu menu = new GenericMenu();

			//menu.AddDisabledItem(new GUIContent("I clicked on a thing"));
			menu.AddItem(new GUIContent("New Selector Node"), false, SelectorCallback);
			menu.AddItem(new GUIContent("New Sequence Node"), false, SequenceCallback);
			menu.AddItem(new GUIContent("New Execution Node"), false, ExecutionCallback);
			menu.ShowAsContext();
			
			current.Use();
		}
		
        GUI.DragWindow();
    }
}