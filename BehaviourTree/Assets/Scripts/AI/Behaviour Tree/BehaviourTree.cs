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

    public override int ChildrenCount () {
        return this.child != null ? 1 : 0;
    }

    public override List<BehaviourTreeNode> GetChildren () {

		List<BehaviourTreeNode> l = new List<BehaviourTreeNode>();
		l.Add(child);
        return l;
    }

	public override void RemoveChild (BehaviourTreeNode child) {
        this.child = null;
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
	private BehaviourTreeNode selectedNode;
	private Vector2 nodeSize = new Vector2(180, 70);
	private Vector2 rootPosition;
	private Vector2 windowMovement;
	private int nextWindowID;

	private Color colorBehaviourTreeNode = new Color(0.5f, 0.5f, 0.5f, 1.0f);
	private Color colorControlSelectorNode = new Color(0.0f, 0.8f, 0.0f, 1.0f);
	private Color colorControlSequenceNode = new Color(1.0f, 0.8f, 0.0f, 1.0f);
	private Color colorExecutionNode = new Color(1.0f, 0.0f, 1.0f, 1.0f);
	private Color colorLine = new Color(0.5f, 0.5f, 0.5f, 1.0f);
	private Color colorDefault = new Color(1.0f, 1.0f, 1.0f, 1.0f);
	private float zoomScale = 1.0f;

	[MenuItem("Window/Behaviour Tree")]
	public static void ShowWindow () {
		BehaviourTreeEditor editor = GetWindow<BehaviourTreeEditor>("BehaviourTree");
		editor.InitEditor(null);
	}
	
	public void InitEditor (BehaviourTree behaviourTree) {

		this.behaviourTree = behaviourTree;

		if(this.behaviourTree != null) {

			this.behaviourTree.Init();

			this.windowMovement = new Vector2(0.0f, 0.0f);

			this.rootPosition = new Vector2(this.position.width / 2 - (nodeSize.x * zoomScale) / 2, 10);

			this.behaviourTree.rect = new Rect (rootPosition.x, rootPosition.y, nodeSize.x, nodeSize.y);

			if(this.behaviourTree.child != null) {
				CreateNodeRect(this.behaviourTree.child, this.behaviourTree);
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

	void OnGUI () {

		this.nextWindowID = 0;

		if(this.behaviourTree != null) {

			BeginWindows();

			GUI.color = colorBehaviourTreeNode;
			
			this.behaviourTree.rect.x = this.position.width / 2 - (nodeSize.x * zoomScale) / 2 + this.windowMovement.x;
			this.behaviourTree.rect.y = 10.0f + this.windowMovement.y;
			this.behaviourTree.rect.width = nodeSize.x * zoomScale;
			this.behaviourTree.rect.height = nodeSize.y * zoomScale;
			int idRoot = GetNextWindowID();
			this.behaviourTree.ID = idRoot;
			this.behaviourTree.rect = GUI.Window(idRoot, this.behaviourTree.rect, RootWindowFunction, this.behaviourTree.name);

			DrawChildrenRecursively(this.behaviourTree);

			EndWindows();

			behaviourTreeSerializedAsset.ApplyModifiedProperties();

			Event current = Event.current;

			// zoom control with mouse wheel
			if(current.type.Equals(EventType.ScrollWheel)) {
				float zoomDelta = 0.1f;
				zoomDelta = current.delta.y < 0 ? zoomDelta : -zoomDelta;
				zoomScale += zoomDelta;
				zoomScale = Mathf.Clamp(zoomScale, 0.2f, 1.5f);
				current.Use();
			}
			
			// movement with mouse left drag
			else if(current.type.Equals(EventType.MouseDrag) && current.button == 0) {
				this.windowMovement += current.delta;
				current.Use();
			}
			
			// reset zoom and movement with mouse middle click
			else if(current.type.Equals(EventType.MouseDown) && current.button == 2) {
				this.windowMovement = new Vector2(0.0f, 0.0f);
				this.zoomScale = 1.0f;
				current.Use();
			}
		}
    }

	void DrawChildrenRecursively (BehaviourTreeNode node) {

		List<BehaviourTreeNode> children = node.GetChildren();

		for(int i=0; i<node.ChildrenCount(); i++) {
			CreateNodeRect(children[i], node);

			Handles.BeginGUI();

			Vector2 begin = new Vector2(node.rect.center.x, node.rect.yMax);
			Vector2 end = new Vector2(children[i].rect.center.x, children[i].rect.yMin + 2.0f);

			float xAverage = (node.rect.center.x + children[i].rect.center.x) / 2.0f;

			Vector2 beginTangente = new Vector2(xAverage, children[i].rect.yMin);
			Vector2 endTangente = new Vector2(xAverage, node.rect.yMax);

			Handles.DrawBezier(begin, 
			end, 
			beginTangente, 
			endTangente,
			Color.red,
			null,
			5f);

			Handles.EndGUI();
		}

		for(int i=0; i<node.ChildrenCount(); i++) {
			DrawChildrenRecursively(children[i]);
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
		AddChildToParent(newNode, this.selectedNode);
	}

	void SequenceCallback () {
		BehaviourTreeControlNode newNode = (BehaviourTreeControlNode)ScriptableObject.CreateInstance("BehaviourTreeControlNode");
		newNode.type = BehaviourTreeControlNode.Type.SEQUENCE;
		newNode.name = "New Sequence Node";
		AddChildToParent(newNode, this.selectedNode);
	}

	void ExecutionCallback () {
		BehaviourTreeExecutionNode newNode = (BehaviourTreeExecutionNode)ScriptableObject.CreateInstance("BehaviourTreeExecutionNode");
		newNode.name = "New Execution Node";
		AddChildToParent(newNode, this.selectedNode);
	}

	void SubTreeCallback () {
		Debug.Log("SubTreeCallback");
	}

	void DeleteNodeAndChildrenCallback () {
		BehaviourTreeNode parent = FindParentOfNodeByID(this.behaviourTree, this.selectedNode.ID);

		parent.RemoveChild(selectedNode);
	}

	void CreateNodeRect(BehaviourTreeNode node, BehaviourTreeNode nodeParent) {

		SetColor(node);

		Vector2 position = new Vector2(nodeParent.rect.x, nodeParent.rect.y + (nodeSize.y + 50.0f) * zoomScale);

		node.rect = new Rect (position.x, position.y, nodeSize.x * zoomScale, nodeSize.y * zoomScale);

		GUI.WindowFunction func = ControlWindowFunction;

		if(node is BehaviourTreeExecutionNode) {
			func = ExecutionWindowFunction;
		}

		int idNode = GetNextWindowID();
		node.ID = idNode;

		int nbChildren = nodeParent.ChildrenCount();

		float xStart;

		float nodeSizeWithChildren = SizeOfNode(nodeParent);

		xStart = nodeParent.rect.x - (nodeSizeWithChildren - nodeSize.x * zoomScale) / 2.0f;

		List<BehaviourTreeNode> children = nodeParent.GetChildren();

		for(int i=0; i<nbChildren; i++) {
			if(children[i].rect != null) {

				float sizeCurrentChild = SizeOfNode(children[i]);
				children[i].rect.x = xStart + (sizeCurrentChild - nodeSize.x * zoomScale) / 2.0f;
				xStart += sizeCurrentChild + 20.0f * zoomScale;
			}
		}
		node.rect = GUI.Window (idNode, node.rect, func, node.name);
	}

	int GetNextWindowID () {
		return nextWindowID++;
	}

	float SizeOfNode (BehaviourTreeNode node) {

		float size = 0.0f;

		if(node is BehaviourTreeExecutionNode) {
			return nodeSize.x * zoomScale;
		}

		int nbChildren = node.ChildrenCount();

		List<BehaviourTreeNode> children = node.GetChildren();

		for(int i=0; i<nbChildren; i++) {
			size += SizeOfNode(children[i]);
		}
		size += Mathf.Max(20.0f * (nbChildren-1) * zoomScale, 0.0f);

		return Mathf.Max(nodeSize.x * zoomScale, size);
	}

	BehaviourTreeNode FindNodeByID(BehaviourTreeNode node, int ID) {

		if(node.ID.Equals(ID)) {
			return node;
		}
		
		foreach(BehaviourTreeNode child in node.GetChildren()) {
			if(child.ID.Equals(ID)) {
				return child;
			}
			BehaviourTreeNode recursiveResult = FindNodeByID(child, ID);
			if(recursiveResult != null) {
				return recursiveResult;
			}
		}

		return null;
	}

	BehaviourTreeNode FindParentOfNodeByID(BehaviourTreeNode node, int ID) {

		if(node.ID.Equals(ID)) {
			return null;
		}
		
		foreach(BehaviourTreeNode child in node.GetChildren()) {
			if(child.ID.Equals(ID)) {
				return node;
			}
			BehaviourTreeNode recursiveResult = FindParentOfNodeByID(child, ID);
			if(recursiveResult != null) {
				return recursiveResult;
			}
		}

		return null;
	}

	void AddChildToParent (BehaviourTreeNode child, BehaviourTreeNode parent) {

		if(parent is BehaviourTree) {

			if(this.behaviourTree.child == null) {
				this.behaviourTree.child = child;
			}

		} else if(parent is BehaviourTreeControlNode) {

			BehaviourTreeControlNode controlNode = (BehaviourTreeControlNode)parent;

			controlNode.children.Add(child);

		}
	}

    void RootWindowFunction (int windowID) {

		Event current = Event.current;
		if(current.type == EventType.MouseDown && current.button == 1) {

			this.selectedNode = FindNodeByID(this.behaviourTree, windowID);
			
			GenericMenu menu = new GenericMenu();

			menu.AddItem(new GUIContent("New Selector Node"), false, SelectorCallback);
			menu.AddItem(new GUIContent("New Sequence Node"), false, SequenceCallback);
			menu.AddItem(new GUIContent("New Execution Node"), false, ExecutionCallback);
			menu.ShowAsContext();
			
			current.Use();
		}
    }

    void ControlWindowFunction (int windowID) {

		Event current = Event.current;
		if(current.type == EventType.MouseDown && current.button == 1) {

			this.selectedNode = FindNodeByID(this.behaviourTree, windowID);
			
			GenericMenu menu = new GenericMenu();

			menu.AddItem(new GUIContent("New Selector Node"), false, SelectorCallback);
			menu.AddItem(new GUIContent("New Sequence Node"), false, SequenceCallback);
			menu.AddItem(new GUIContent("New Execution Node"), false, ExecutionCallback);
			menu.AddItem(new GUIContent("New Sub-tree Node"), false, SubTreeCallback);
			menu.AddItem(new GUIContent("Delete Node and Children"), false, DeleteNodeAndChildrenCallback);
			menu.ShowAsContext();
			
			current.Use();
		}
    }

	void ExecutionWindowFunction (int windowID) {

		Event current = Event.current;
		if(current.type == EventType.MouseDown && current.button == 1) {

			this.selectedNode = FindNodeByID(this.behaviourTree, windowID);
			
			GenericMenu menu = new GenericMenu();

			menu.AddItem(new GUIContent("Delete Node"), false, DeleteNodeAndChildrenCallback);
			menu.ShowAsContext();
			
			current.Use();
		}
    }
}