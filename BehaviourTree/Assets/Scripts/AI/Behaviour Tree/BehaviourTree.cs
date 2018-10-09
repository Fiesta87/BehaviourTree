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

			editor.InitEditor(behaviourTree);

			return true; //catch open file
		}        

		return false; // let unity open the file
    }

    public override void Init () {
		if(child != null) {
			child.Init();
		}
    }

    public override BehaviourTree.Status Tick () {
        return child.Tick();
    }

	public override string GetName () {
        return this.name;
    }

    public override int ChildrenCount () {
        return this.child != null ? 1 : 0;
    }

    public override List<BehaviourTreeNode> GetChildren () {

		List<BehaviourTreeNode> l = new List<BehaviourTreeNode>();
		l.Add(child);
        return l;
    }

	public enum Status {
		RUNNING,
		SUCCESS,
		FAILURE
	}
}

public class BehaviourTreeEditor : EditorWindow {

	private SerializedObject behaviourTreeSerializedAsset;
	private BehaviourTree behaviourTree;

	//private BehaviourTreeNode newNode;
	private BehaviourTreeNode newNodeParent;
	private Vector2 nodeSize = new Vector2(180, 70);
	private Vector2 rootPosition;

	//private List<Rect> rects;
	private int nextWindowID;
	private Dictionary<int, BehaviourTreeNode> idToNode;

	private Color colorBehaviourTreeNode = new Color(0.5f, 0.5f, 0.5f, 1.0f);
	private Color colorControlSelectorNode = new Color(0.0f, 0.8f, 0.0f, 1.0f);
	private Color colorControlSequenceNode = new Color(1.0f, 0.8f, 0.0f, 1.0f);
	private Color colorExecutionNode = new Color(1.0f, 0.0f, 1.0f, 1.0f);
	private Color colorLine = new Color(0.5f, 0.5f, 0.5f, 1.0f);
	private Color colorDefault = new Color(1.0f, 1.0f, 1.0f, 1.0f);

	[MenuItem("Window/Behaviour Tree")]
	public static void ShowWindow () {
		BehaviourTreeEditor editor = GetWindow<BehaviourTreeEditor>("BehaviourTree");
		editor.InitEditor(null);
	}
	
	public void InitEditor (BehaviourTree behaviourTree) {

		this.behaviourTree = behaviourTree;

		//this.newNode = null;

		//this.rects = new List<Rect>();

		if(this.behaviourTree != null) {

			this.behaviourTree.Init();

			this.rootPosition = new Vector2(this.position.width / 2 - nodeSize.x / 2, 10);

			this.behaviourTree.rect = new Rect (rootPosition.x, rootPosition.y, nodeSize.x, nodeSize.y);

			if(this.behaviourTree.child != null) {
				CreateNodeRect(this.behaviourTree.child, this.behaviourTree);
				//this.behaviourTree.child.rect = new Rect (this.position.width / 2 - 90, 170, nodeSize.x, nodeSize.y);
			}

			// string filePath = "Assets/Datas/Behaviour Tree/" + this.behaviourTree.GetName();
			// try {
			// 	if (!Directory.Exists(filePath)) {
			// 		Directory.CreateDirectory(filePath);
			// 		AssetDatabase.Refresh();
			// 	}
			// }
			// catch (IOException ex) {
			// 	Debug.Log(ex.Message);
			// }

			this.behaviourTreeSerializedAsset = new SerializedObject(this.behaviourTree);
		}
	}

	// public static bool IsMouseOverWindow(EditorWindow window) {
    // 	return mouseOverWindow && window.GetType().FullName == mouseOverWindow.ToString().Split('(', ')')[1];
	// }

	void OnGUI () {

		//GUI.backgroundColor

		nextWindowID = 0;

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

			GUI.color = colorBehaviourTreeNode;
			
			this.behaviourTree.rect.x = this.position.width / 2 - nodeSize.x / 2;
			this.behaviourTree.rect.y = 10;
			this.behaviourTree.rect = GUI.Window(GetNextWindowID(), this.behaviourTree.rect, RootWindowFunction, this.behaviourTree.GetName());

			GUI.color = colorDefault;

			// if(this.newNode != null) {

			// 	SetColor(this.newNode);

			// 	Rect newNodeRect = new Rect (this.position.width / 2 - 90, 330, 180, 70);
			// 	newNodeRect = GUI.Window (1, newNodeRect, WindowFunction, this.newNode.GetName());
			// }

			DrawChildrenRecursively(this.behaviourTree);
			
			if(this.behaviourTree.child != null) {

				//SetColor(this.behaviourTree.child);

				//CreateNodeRect(this.behaviourTree.child, this.behaviourTree);
				//this.behaviourTree.child.rect = GUI.Window (2, this.behaviourTree.child.rect, ControlWindowFunction, this.behaviourTree.child.GetName());

				
			}

			

			EndWindows();

			behaviourTreeSerializedAsset.ApplyModifiedProperties();
		}
    }

	void DrawChildrenRecursively (BehaviourTreeNode node) {

		List<BehaviourTreeNode> children = node.GetChildren();

		for(int i=0; i<node.ChildrenCount(); i++) {
			CreateNodeRect(children[i], node);
		}

	}

	void SetColor(BehaviourTreeNode node) {

		if(node is BehaviourTreeControlNode) {

			BehaviourTreeControlNode controlNode = (BehaviourTreeControlNode)node;

			switch(controlNode.type) {
				case BehaviourTreeControlNode.Type.SELECTOR: GUI.color = colorControlSelectorNode; break;
				case BehaviourTreeControlNode.Type.SEQUENCE: GUI.color = colorControlSequenceNode; break;
				default: GUI.color = colorDefault; break;
			}

		} else if(node is BehaviourTreeExecutionNode) {
			GUI.color = colorExecutionNode;
		} else {
			GUI.color = colorDefault;
		}
	}

	// void AddNodeToAssets(BehaviourTreeNode node) {
	// 	AssetDatabase.CreateAsset(node, "Assets/Datas/Behaviour Tree/" + this.behaviourTree.name + "/" + node.name + ".asset");
	// 	AssetDatabase.Refresh();
	// }

	void SelectorCallback () {
		BehaviourTreeControlNode newNode = (BehaviourTreeControlNode)ScriptableObject.CreateInstance("BehaviourTreeControlNode");
		newNode.type = BehaviourTreeControlNode.Type.SELECTOR;
		newNode.name = "New Selector Node";
		// this.newNode = asset;
		AddChildToParent(newNode, this.newNodeParent);
	}

	void SequenceCallback () {
		BehaviourTreeControlNode newNode = (BehaviourTreeControlNode)ScriptableObject.CreateInstance("BehaviourTreeControlNode");
		newNode.type = BehaviourTreeControlNode.Type.SEQUENCE;
		newNode.name = "New Sequence Node";
		// this.newNode = asset;
		AddChildToParent(newNode, this.newNodeParent);
	}

	void ExecutionCallback () {
		BehaviourTreeExecutionNode newNode = (BehaviourTreeExecutionNode)ScriptableObject.CreateInstance("BehaviourTreeExecutionNode");
		newNode.name = "New Execution Node";
		AddChildToParent(newNode, this.newNodeParent);
	}

	void SubTreeCallback () {
		Debug.Log("SubTreeCallback");
	}

	void DeleteNodeAndChildrenCallback () {
		Debug.Log("DeleteNodeAndChildrenCallback");
	}

	void CreateNodeRect(BehaviourTreeNode node, BehaviourTreeNode nodeParent) {

		SetColor(node);

		Vector2 position = new Vector2(0.0f, nodeParent.rect.y + nodeSize.y + 50.0f);

		node.rect = new Rect (position.x, position.y, nodeSize.x, nodeSize.y);

		GUI.WindowFunction func = ControlWindowFunction;

		if(node is BehaviourTreeExecutionNode) {
			func = ExecutionWindowFunction;
		}

		node.rect = GUI.Window (GetNextWindowID(), node.rect, func, this.behaviourTree.child.GetName());

		int nbChildren = nodeParent.ChildrenCount();

		float xStart;

		float nodeSizeWithChildren = SizeOfNode(nodeParent);

		//xStart = nodeParent.rect.x - ((float)(nbChildren-1) / 2.0f) * (nodeSize.x + 20.0f);

		xStart = nodeParent.rect.x - nodeSizeWithChildren / 2.0f;

		List<BehaviourTreeNode> children = nodeParent.GetChildren();

		for(int i=0; i<nbChildren; i++) {
			if(children[i].rect != null) {

				float sizeCurrentChild = SizeOfNode(children[i]);
				children[i].rect.x = xStart + sizeCurrentChild / 2.0f;
				xStart += sizeCurrentChild;
			}
		}
	}

	int GetNextWindowID () {
		return nextWindowID++;
	}

	float SizeOfNode (BehaviourTreeNode node) {

		float size = 0.0f;

		if(node is BehaviourTreeExecutionNode) {
			return nodeSize.x;
		}

		int nbChildren = node.ChildrenCount();

		List<BehaviourTreeNode> children = node.GetChildren();

		for(int i=0; i<nbChildren; i++) {
			size += SizeOfNode(children[i]);
		}

		return Mathf.Max(nodeSize.x, size);
	}

	void AddChildToParent (BehaviourTreeNode child, BehaviourTreeNode parent) {

		if(parent is BehaviourTree) {

			if(this.behaviourTree.child == null) {
				//child.rect = new Rect (this.position.width / 2 - 90, 170, nodeSize.x, nodeSize.y);
				this.behaviourTree.child = child;
			}

		} else if(parent is BehaviourTreeControlNode) {

			BehaviourTreeControlNode controlNode = (BehaviourTreeControlNode)parent;

			controlNode.children.Add(child);

		}/* else if(node is BehaviourTreeExecutionNode) {
			GUI.color = colorExecutionNode;
		} else {
			GUI.color = colorDefault;
		}*/
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
		
        //GUI.DragWindow();
    }

    void ControlWindowFunction (int windowID) {

		Event current = Event.current;
		if(current.type == EventType.MouseDown && current.button == 1) {
			
			GenericMenu menu = new GenericMenu();

			//menu.AddDisabledItem(new GUIContent("I clicked on a thing"));
			menu.AddItem(new GUIContent("New Selector Node"), false, SelectorCallback);
			menu.AddItem(new GUIContent("New Sequence Node"), false, SequenceCallback);
			menu.AddItem(new GUIContent("New Execution Node"), false, ExecutionCallback);
			menu.AddItem(new GUIContent("New Sub-tree Node"), false, SubTreeCallback);
			menu.AddItem(new GUIContent("Delete Node and Children"), false, DeleteNodeAndChildrenCallback);
			menu.ShowAsContext();
			
			current.Use();
		}
		
        //GUI.DragWindow();
    }

	void ExecutionWindowFunction (int windowID) {

		Event current = Event.current;
		if(current.type == EventType.MouseDown && current.button == 1) {
			
			GenericMenu menu = new GenericMenu();

			menu.AddItem(new GUIContent("Delete Node"), false, DeleteNodeAndChildrenCallback);
			menu.ShowAsContext();
			
			current.Use();
		}
		
        //GUI.DragWindow();
    }
}