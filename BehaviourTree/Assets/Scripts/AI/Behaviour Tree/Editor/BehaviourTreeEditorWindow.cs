using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.ProjectWindowCallback;
using System.IO;
using System.Linq;

public class BehaviourTreeEditorWindow : EditorWindow {

	public static BehaviourTreeEditorWindow behaviourTreeEditorWindow;
	public static BehaviourTree behaviourTree;

	[SerializeField]
	public BehaviourTree behaviourTreeSerializedSaved;

	private string assetsFilesPath;
	private BehaviourTreeNode selectedNode;
	private Vector2 nodeSize = new Vector2(180, 70);
	private Vector2 rootPosition;
	private Vector2 windowMovement;
	

	private Color colorBehaviourTreeNode = new Color(0.5f, 0.5f, 0.5f, 1.0f);
	private Color colorControlSelectorNode = new Color(1.0f, 0.6f, 0.2f, 1.0f);
	private Color colorControlSequenceNode = new Color(1.0f, 0.8f, 0.2f, 1.0f);
	private Color colorControlParallelNode = new Color(1.0f, 1.0f, 0.2f, 1.0f);
	private Color colorExecutionNode = new Color(1.0f, 0.0f, 1.0f, 1.0f);
	private Color colorDecoratorNode = new Color(0.0f, 0.5f, 1.0f, 1.0f);
	private Color colorLine = new Color(0.5f, 0.5f, 0.5f, 1.0f);
	private Color colorDefault = new Color(1.0f, 1.0f, 1.0f, 1.0f);
	private float zoomScale = 1.0f;

    [OnOpenAsset(1)]
    public static bool OnOpenAsset (int instanceID, int line) {

		BehaviourTree behaviourTree = Selection.activeObject as BehaviourTree;

		if (behaviourTree != null) {

			BehaviourTreeEditorWindow editor = EditorWindow.GetWindow<BehaviourTreeEditorWindow>("BehaviourTree");

			editor.InitEditor(behaviourTree);

			return true; //catch open file
		}        

		return false; // let unity open the file
    }

	[MenuItem("Window/Behaviour Tree")]
	public static void ShowWindow () {
		
		behaviourTreeEditorWindow = GetWindow<BehaviourTreeEditorWindow>(false, "BehaviourTree", true);
		behaviourTreeEditorWindow.InitEditor(BehaviourTreeEditorWindow.behaviourTree);
	}

	public void OnEnable () {
		
		if(behaviourTreeSerializedSaved == null) {
			behaviourTreeSerializedSaved = BehaviourTreeEditorWindow.behaviourTree;
		}
		InitEditor(behaviourTreeSerializedSaved);
	}

	public void OnDisable () {
		
		behaviourTreeSerializedSaved = BehaviourTreeEditorWindow.behaviourTree;
	}
	
	public void InitEditor (BehaviourTree behaviourTree) {

		if(behaviourTree != null) {
			BehaviourTreeEditorWindow.behaviourTree = behaviourTree;
		}

		if(BehaviourTreeEditorWindow.behaviourTree != null) {

			BehaviourTreeEditorWindow.behaviourTree.ID = 0;

			if(BehaviourTreeEditorWindow.behaviourTree.displayedName == null || "".Equals(BehaviourTreeEditorWindow.behaviourTree.displayedName)) {
				BehaviourTreeEditorWindow.behaviourTree.displayedName = BehaviourTreeEditorWindow.behaviourTree.name;
			}

			this.windowMovement = new Vector2(0.0f, 0.0f);

			this.rootPosition = new Vector2(this.position.width / 2 - (nodeSize.x * zoomScale) / 2, 10);

			BehaviourTreeEditorWindow.behaviourTree.rect = new Rect (rootPosition.x, rootPosition.y, nodeSize.x, nodeSize.y);

			// string filePath = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(BehaviourTreeEditorWindow.behaviourTree)); // Assets/Scrpits/AI/BT/BT.cs

			assetsFilesPath = AssetDatabase.GetAssetPath(BehaviourTreeEditorWindow.behaviourTree); // Assets/Datas/BT/First.asset
			
			assetsFilesPath = assetsFilesPath.Split('.')[0];

			try {
				if (!Directory.Exists(assetsFilesPath)) {
					Directory.CreateDirectory(assetsFilesPath);
					AssetDatabase.Refresh();
				}
			}
			catch (IOException ex) {
				Debug.Log(ex.Message);
			}

			SaveBehaviourTree();
		}
	}

	void OnGUI () {

		if(BehaviourTreeEditorWindow.behaviourTree == null) {
			return;
		}

		BeginWindows();

		GUI.color = colorBehaviourTreeNode;
		
		BehaviourTreeEditorWindow.behaviourTree.rect.x = this.position.width / 2 - (nodeSize.x * zoomScale) / 2 + this.windowMovement.x;
		BehaviourTreeEditorWindow.behaviourTree.rect.y = 10.0f + this.windowMovement.y;
		BehaviourTreeEditorWindow.behaviourTree.rect.width = nodeSize.x * zoomScale;
		BehaviourTreeEditorWindow.behaviourTree.rect.height = nodeSize.y * zoomScale;

		BehaviourTreeEditorWindow.behaviourTree.rect = GUI.Window(BehaviourTreeEditorWindow.behaviourTree.ID, BehaviourTreeEditorWindow.behaviourTree.rect, RootWindowFunction, BehaviourTreeEditorWindow.behaviourTree.displayedName);

		DrawChildrenRecursively(BehaviourTreeEditorWindow.behaviourTree);

		EndWindows();

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

		else if(current.type.Equals(EventType.KeyDown) && current.keyCode == KeyCode.Delete) {

			if(this.selectedNode != null && this.selectedNode != BehaviourTreeEditorWindow.behaviourTree) {
				DeleteNodeAndChildrenCallback();
			}
			
			current.Use();
		}
    }

	public static void SaveBehaviourTree () {
		SaveNodeAnChildren(BehaviourTreeEditorWindow.behaviourTree);
	}

	public static void SaveNodeAnChildren (BehaviourTreeNode node) {

		if(node == null) {
			throw new System.NullReferenceException("null node can't be saved");
		}

		SerializedObject nodeSerializedAsset = new SerializedObject(node);

		nodeSerializedAsset.Update();

		SerializedProperty p = nodeSerializedAsset.FindProperty("displayedName");

		p.stringValue = "";

		p.stringValue = node.displayedName;

		nodeSerializedAsset.ApplyModifiedProperties();

		foreach(BehaviourTreeNode child in node.GetChildren()) {
			SaveNodeAnChildren(child);
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

			Handles.DrawBezier(begin, end, beginTangente, endTangente, colorLine, null, 5f);

			Handles.EndGUI();
		}

		for(int i=0; i<node.ChildrenCount(); i++) {
			DrawChildrenRecursively(children[i]);
		}
	}

	void SetColor(BehaviourTreeNode node) {

		GUI.color = colorDefault;

		if(node is BehaviourTreeControlNode) {

			BehaviourTreeControlNode controlNode = (BehaviourTreeControlNode)node;

			switch(controlNode.type) {
				case BehaviourTreeControlNode.Type.SELECTOR: GUI.color = colorControlSelectorNode; break;
				case BehaviourTreeControlNode.Type.SEQUENCE: GUI.color = colorControlSequenceNode; break;
				case BehaviourTreeControlNode.Type.PARALLEL: GUI.color = colorControlParallelNode; break;
				default: GUI.color = colorDefault; break;
			}

		} else if(node is BehaviourTreeExecutionNode) {
			GUI.color = colorExecutionNode;
		} else if(node is BehaviourTreeDecoratorNode) {
			GUI.color = colorDecoratorNode;
		}
	}

	void AddNodeToAssets(BehaviourTreeNode node) {
		AssetDatabase.CreateAsset(node, assetsFilesPath + "/Node" + node.ID + ".asset");
		AssetDatabase.Refresh();
	}

	void ControlSelectorCallback () {
		BehaviourTreeControlNode newNode = (BehaviourTreeControlNode)ScriptableObject.CreateInstance("BehaviourTreeControlNode");
		newNode.type = BehaviourTreeControlNode.Type.SELECTOR;
		newNode.ID = GetNextWindowID();
		newNode.displayedName = newNode.type.ToString();
		AddChildToParent(newNode, this.selectedNode);
		AddNodeToAssets(newNode);
		SaveBehaviourTree();
	}

	void ControlSequenceCallback () {
		BehaviourTreeControlNode newNode = (BehaviourTreeControlNode)ScriptableObject.CreateInstance("BehaviourTreeControlNode");
		newNode.type = BehaviourTreeControlNode.Type.SEQUENCE;
		newNode.ID = GetNextWindowID();
		newNode.displayedName = newNode.type.ToString();
		AddChildToParent(newNode, this.selectedNode);
		AddNodeToAssets(newNode);
		SaveBehaviourTree();
	}

	void ControlParallelCallback () {
		BehaviourTreeControlNode newNode = (BehaviourTreeControlNode)ScriptableObject.CreateInstance("BehaviourTreeControlNode");
		newNode.type = BehaviourTreeControlNode.Type.PARALLEL;
		newNode.ID = GetNextWindowID();
		newNode.displayedName = newNode.type.ToString();
		AddChildToParent(newNode, this.selectedNode);
		AddNodeToAssets(newNode);
		SaveBehaviourTree();
	}

	void DecoratorInverterCallback () {
		BehaviourTreeDecoratorNode newNode = (BehaviourTreeDecoratorNode)ScriptableObject.CreateInstance("BehaviourTreeDecoratorNode");
		newNode.type = BehaviourTreeDecoratorNode.Type.INVERTER;
		newNode.ID = GetNextWindowID();
		newNode.displayedName = newNode.type.ToString();
		AddChildToParent(newNode, this.selectedNode);
		AddNodeToAssets(newNode);
		SaveBehaviourTree();
	}

	void DecoratorSucceederCallback () {
		BehaviourTreeDecoratorNode newNode = (BehaviourTreeDecoratorNode)ScriptableObject.CreateInstance("BehaviourTreeDecoratorNode");
		newNode.type = BehaviourTreeDecoratorNode.Type.SUCCEEDER;
		newNode.ID = GetNextWindowID();
		newNode.displayedName = newNode.type.ToString();
		AddChildToParent(newNode, this.selectedNode);
		AddNodeToAssets(newNode);
		SaveBehaviourTree();
	}

	void ExecutionCallback () {
		BehaviourTreeExecutionNode newNode = (BehaviourTreeExecutionNode)ScriptableObject.CreateInstance("BehaviourTreeExecutionNode");
		newNode.ID = GetNextWindowID();
		AddChildToParent(newNode, this.selectedNode);
		newNode.displayedName = "Execution";
		AddNodeToAssets(newNode);
		SaveBehaviourTree();
	}

	void SubTreeCallback () {
		Debug.Log("SubTreeCallback");
	}

	void DeleteNodeAndChildrenCallback () {
		BehaviourTreeNode parent = FindParentOfNodeByID(BehaviourTreeEditorWindow.behaviourTree, this.selectedNode.ID);
		parent.RemoveChild(selectedNode);
		DeleteRecursivelyNodeAndChildrenAssets(selectedNode);
		AssetDatabase.Refresh();
		SaveBehaviourTree();
	}

	void DeleteRecursivelyNodeAndChildrenAssets (BehaviourTreeNode node) {

		AssetDatabase.DeleteAsset(assetsFilesPath + "/Node" + node.ID + ".asset");

		if(node is BehaviourTreeExecutionNode && File.Exists(assetsFilesPath + "/Node" + node.ID + "Task.asset")) {

			AssetDatabase.DeleteAsset(assetsFilesPath + "/Node" + node.ID + "Task.asset");
		}

		foreach(BehaviourTreeNode child in node.GetChildren()) {
			DeleteRecursivelyNodeAndChildrenAssets(child);
		}
	}

	void CreateNodeRect(BehaviourTreeNode node, BehaviourTreeNode nodeParent) {

		SetColor(node);

		Vector2 position = new Vector2(nodeParent.rect.x, nodeParent.rect.y + (nodeSize.y + 50.0f) * zoomScale);

		node.rect = new Rect (position.x, position.y, nodeSize.x * zoomScale, nodeSize.y * zoomScale);

		GUI.WindowFunction func = ControlWindowFunction;

		if(node is BehaviourTreeExecutionNode) {
			func = ExecutionWindowFunction;
		} else if (node is BehaviourTreeDecoratorNode) {
			func = DecoratorWindowFunction;
		}

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
		node.rect = GUI.Window (node.ID, node.rect, func, node.displayedName);
	}

	int GetNextWindowID () {
		return BehaviourTreeEditorWindow.behaviourTree.nextWindowID++;
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

			if(BehaviourTreeEditorWindow.behaviourTree.child == null) {
				BehaviourTreeEditorWindow.behaviourTree.child = child;
			}

		} else if(parent is BehaviourTreeControlNode) {

			BehaviourTreeControlNode controlNode = (BehaviourTreeControlNode)parent;

			controlNode.children.Add(child);

		} else if(parent is BehaviourTreeDecoratorNode) {

			BehaviourTreeDecoratorNode decoratorNode = (BehaviourTreeDecoratorNode)parent;

			if(decoratorNode.child == null) {
				decoratorNode.child = child;
			}
		}
	}

    void RootWindowFunction (int windowID) {

		Event current = Event.current;
		if(current.type == EventType.MouseDown && current.button == 1) {

			this.selectedNode = FindNodeByID(BehaviourTreeEditorWindow.behaviourTree, windowID);
			
			GenericMenu menu = new GenericMenu();

			AddNewChildOption(menu, BehaviourTreeEditorWindow.behaviourTree.child == null);
			menu.ShowAsContext();
			
			current.Use();

		} else if(current.type == EventType.MouseDown && current.button == 0) {

			this.selectedNode = FindNodeByID(BehaviourTreeEditorWindow.behaviourTree, windowID);

			current.Use();
		}
    }

    void ControlWindowFunction (int windowID) {

		BehaviourTreeControlNode node = (BehaviourTreeControlNode)FindNodeByID(BehaviourTreeEditorWindow.behaviourTree, windowID);

		GUI.color = Color.white;

		BehaviourTreeControlNode.Type initialTypeValue = node.type;

		BehaviourTreeControlNode.Type typeValue = (BehaviourTreeControlNode.Type)EditorGUILayout.EnumPopup("Type", initialTypeValue);

		if( typeValue != initialTypeValue) {
			node.type = typeValue;
			node.displayedName = typeValue.ToString();
			SaveNodeAnChildren(node);
		}

		bool initialMemorizeValue = node.startFromFirstNodeEachTick;

		GUILayout.BeginHorizontal();

			GUI.color = Color.black;
			GUILayout.Label("Don't memorize running node");

			GUI.color = Color.white;
			bool memorizeValue = EditorGUILayout.Toggle(initialMemorizeValue);
			if( memorizeValue != initialMemorizeValue) {
				node.startFromFirstNodeEachTick = memorizeValue;
				SaveNodeAnChildren(node);
			}
			
		GUILayout.EndHorizontal();

		Event current = Event.current;

		if(current.type == EventType.MouseDown && current.button == 1) {

			this.selectedNode = node;
			
			GenericMenu menu = new GenericMenu();

			AddNewChildOption(menu, true);
			menu.AddItem(new GUIContent("New Child/Sub-tree"), false, SubTreeCallback);
			AddMoveOption(menu);
			menu.AddItem(new GUIContent("Delete Node and Children"), false, DeleteNodeAndChildrenCallback);
			menu.ShowAsContext();
			
			current.Use();

		} else if(current.type == EventType.MouseDown && current.button == 0) {

			this.selectedNode = FindNodeByID(BehaviourTreeEditorWindow.behaviourTree, windowID);

			current.Use();
		}
    }

	void ExecutionWindowFunction (int windowID) {

		BehaviourTreeExecutionNode node = (BehaviourTreeExecutionNode)FindNodeByID(BehaviourTreeEditorWindow.behaviourTree, windowID);

		if(node.task != null) {

			PropertyReader.Variable[] variables = PropertyReader.GetFields(node.task.GetType());

			GUILayout.Label("PARAMETER");

			foreach(PropertyReader.Variable variable in variables) {
				if(variable.name.StartsWith("param_")) {
					AddParameterField(node, variable);
				}
			}

			GUILayout.Label("INPUT");

			foreach(PropertyReader.Variable variable in variables) {
				if(variable.name.StartsWith("in_")) {
					AddContextField(node, variable);
				}
			}

			GUILayout.Label("OUTPUT");

			foreach(PropertyReader.Variable variable in variables) {
				if(variable.name.StartsWith("out_")) {
					AddContextField(node, variable);
				}
			}
		}

		Event current = Event.current;

		if(current.mousePosition.x >= 0.0f && current.mousePosition.x <= nodeSize.x * zoomScale &&
		current.mousePosition.y >= 0.0f && current.mousePosition.y <= nodeSize.y * zoomScale &&
		DragAndDrop.objectReferences.Length == 1) {

			Object taskScriptAsset = DragAndDrop.objectReferences[0];

			if( ! (taskScriptAsset is MonoScript)) {
				DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
			} else {
				DragAndDrop.visualMode = DragAndDropVisualMode.Link;
			}
		}
		
		if(current.type == EventType.MouseDown && current.button == 1) {

			this.selectedNode = node;
			
			GenericMenu menu = new GenericMenu();

			AddMoveOption(menu);
			menu.AddItem(new GUIContent("Delete Node"), false, DeleteNodeAndChildrenCallback);
			menu.ShowAsContext();
			
			current.Use();

		} else if(current.type.Equals(EventType.DragExited)) {

			Object taskScriptAsset = DragAndDrop.objectReferences[0];

			if( ! (taskScriptAsset is MonoScript)) {
				current.Use();
				return;
			}

			System.Type taskType = (taskScriptAsset as MonoScript).GetClass();

			ScriptableObject so = ScriptableObject.CreateInstance(taskType);

			if( ! (so is BehaviourTreeTask)) {
				current.Use();
				return;
			}
			
			node.task = so as BehaviourTreeTask;

			node.contextLink.Clear();

			PropertyReader.Variable[] variables = PropertyReader.GetFields(node.task.GetType());

			foreach(PropertyReader.Variable variable in variables) {
				if(variable.name.StartsWith("in_") || variable.name.StartsWith("out_")) {
					node.contextLink.Add(variable.name, variable.name.Split('_')[1]);
				}
			}

			node.displayedName = taskType.ToString();

			AddTaskToAssets(node);

			BehaviourTreeEditorWindow.SaveBehaviourTree();

			current.Use();

		} else if(current.type == EventType.MouseDown && current.button == 0) {

			this.selectedNode = FindNodeByID(BehaviourTreeEditorWindow.behaviourTree, windowID);

			current.Use();
		}
    }

	void DecoratorWindowFunction (int windowID) {

		BehaviourTreeDecoratorNode node = (BehaviourTreeDecoratorNode)FindNodeByID(BehaviourTreeEditorWindow.behaviourTree, windowID);

		GUI.color = Color.white;

		BehaviourTreeDecoratorNode.Type initialTypeValue = node.type;

		BehaviourTreeDecoratorNode.Type typeValue = (BehaviourTreeDecoratorNode.Type)EditorGUILayout.EnumPopup("Type", initialTypeValue);

		if( typeValue != initialTypeValue) {
			node.type = typeValue;
			node.displayedName = typeValue.ToString();
			SaveNodeAnChildren(node);
		}

		Event current = Event.current;

		if(current.type == EventType.MouseDown && current.button == 1) {

			this.selectedNode = node;
			
			GenericMenu menu = new GenericMenu();

			AddNewChildOption(menu, node.child == null);
			if(node.child == null) {
				menu.AddItem(new GUIContent("New Child/Sub-tree"), false, SubTreeCallback);
			}
			AddMoveOption(menu);
			menu.AddItem(new GUIContent("Delete Node and Children"), false, DeleteNodeAndChildrenCallback);
			menu.ShowAsContext();
			
			current.Use();

		} else if(current.type == EventType.MouseDown && current.button == 0) {

			this.selectedNode = FindNodeByID(BehaviourTreeEditorWindow.behaviourTree, windowID);

			current.Use();
		}
    }

	void AddNewChildOption (GenericMenu menu, bool enabled) {

		if(enabled) {
			menu.AddItem(new GUIContent("New Child/Control/Selector"), false, ControlSelectorCallback);
			menu.AddItem(new GUIContent("New Child/Control/Sequence"), false, ControlSequenceCallback);
			menu.AddItem(new GUIContent("New Child/Control/Parallel"), false, ControlParallelCallback);
			menu.AddItem(new GUIContent("New Child/Decorator/Inverter"), false, DecoratorInverterCallback);
			menu.AddItem(new GUIContent("New Child/Decorator/Succeeder"), false, DecoratorSucceederCallback);
			menu.AddItem(new GUIContent("New Child/Execution"), false, ExecutionCallback);
		} else {
			menu.AddDisabledItem(new GUIContent("New Child"));
		}
		
	}

	void AddMoveOption (GenericMenu menu) {
		menu.AddItem(new GUIContent("Move/First"), false, MoveFirstCallback);
		menu.AddItem(new GUIContent("Move/Left"), false, MoveLeftCallback);
		menu.AddItem(new GUIContent("Move/Right"), false, MoveRightCallback);
		menu.AddItem(new GUIContent("Move/Last"), false, MoveLastCallback);
	}

	void MoveFirstCallback () {
		BehaviourTreeNode parent = FindParentOfNodeByID(BehaviourTreeEditorWindow.behaviourTree, this.selectedNode.ID);
		
		int selectedNodeIndice = parent.GetChildren().IndexOf(this.selectedNode);
		
		if(selectedNodeIndice > 0) {
			BehaviourTreeNode node = parent.GetChildren()[selectedNodeIndice];
			parent.GetChildren().Remove(node);
			parent.GetChildren().Insert(0, node);
		}

		SaveBehaviourTree();
	}

	void MoveLeftCallback () {
		BehaviourTreeNode parent = FindParentOfNodeByID(BehaviourTreeEditorWindow.behaviourTree, this.selectedNode.ID);
		
		int selectedNodeIndice = parent.GetChildren().IndexOf(this.selectedNode);
		
		if(selectedNodeIndice > 0) {
			BehaviourTreeNode node = parent.GetChildren()[selectedNodeIndice];
			parent.GetChildren().Remove(node);
			parent.GetChildren().Insert(selectedNodeIndice-1, node);
		}

		SaveBehaviourTree();
	}

	void MoveRightCallback () {
		BehaviourTreeNode parent = FindParentOfNodeByID(BehaviourTreeEditorWindow.behaviourTree, this.selectedNode.ID);
		
		int selectedNodeIndice = parent.GetChildren().IndexOf(this.selectedNode);
		
		if(selectedNodeIndice < parent.ChildrenCount()-1) {
			BehaviourTreeNode node = parent.GetChildren()[selectedNodeIndice];
			parent.GetChildren().Remove(node);
			parent.GetChildren().Insert(selectedNodeIndice+1, node);
		}

		SaveBehaviourTree();
	}

	void MoveLastCallback () {
		BehaviourTreeNode parent = FindParentOfNodeByID(BehaviourTreeEditorWindow.behaviourTree, this.selectedNode.ID);
		
		int selectedNodeIndice = parent.GetChildren().IndexOf(this.selectedNode);
		
		if(selectedNodeIndice < parent.ChildrenCount()-1) {
			BehaviourTreeNode node = parent.GetChildren()[selectedNodeIndice];
			parent.GetChildren().Remove(node);
			parent.GetChildren().Insert(parent.ChildrenCount(), node);
		}

		SaveBehaviourTree();
	}

	void AddContextField (BehaviourTreeExecutionNode node, PropertyReader.Variable variable) {
		
		GUILayout.BeginHorizontal();

			try {
				string initialValue = node.contextLink[variable.name];

				GUI.color = Color.black;
				GUILayout.Label(variable.name.Split('_')[1]);

				GUI.color = Color.white;
				string value = EditorGUILayout.TextField(initialValue);

				node.contextLink[variable.name] = value;

				if( value != initialValue) {
					SaveNodeAnChildren(node);
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
			GUILayout.Label(variable.name.Split('_')[1]);

			GUI.color = Color.white;

			object value = null;

			if(initialValue is string) value = EditorGUILayout.TextField((string)initialValue);
			else if(initialValue is float) value = EditorGUILayout.FloatField((float)initialValue);
			else if(initialValue is int) value = EditorGUILayout.IntField((int)initialValue);
			else if(initialValue is double) value = EditorGUILayout.DoubleField((double)initialValue);
			else if(initialValue is bool) value = EditorGUILayout.Toggle((bool)initialValue);
			
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
				
				SaveNodeAnChildren(node);
			}
			
		GUILayout.EndHorizontal();
	}

	void AddTaskToAssets (BehaviourTreeExecutionNode node) {

		string path = assetsFilesPath + "/Node" + node.ID + "Task.asset";

		if(File.Exists(path)) {

			AssetDatabase.DeleteAsset(path);
		}

		AssetDatabase.CreateAsset(node.task, path);
		AssetDatabase.Refresh();
	}

	void SelectNodeInInspector (BehaviourTreeNode node) {
		Object[] sel = new Object[1];
		sel[0] = node;
		Selection.objects = sel;
	}

	/*********************************************************************************/
	/************************* CREATION OF A TASK ASSET FILE *************************/
	/*********************************************************************************/

	[MenuItem("Assets/Create/Behaviour Tree/Task")]
    public static void CreateTask () {

        string path = AssetDatabase.GetAssetPath(Selection.activeObject);

        if(!Directory.Exists(path)) {
            if (File.Exists(path)) {
                path = Path.GetDirectoryName(path);
            }
            if (string.IsNullOrEmpty(path)){
                path = "Assets/";
            }
        }
        
        string filePath = Path.Combine(path, "NewTask.cs");
        
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<EditFileContent>(), filePath, EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D, "Assets/Scripts/AI/Behaviour Tree/BehaviourTreeTaskTemplate.cs");
    }

    internal class EditFileContent : EndNameEditAction {

        public override void Action (int instanceId, string pathName, string resourceFile) {

			string templatePath = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject((BehaviourTreeTaskTemplate)ScriptableObject.CreateInstance("BehaviourTreeTaskTemplate")));

            File.Copy(templatePath, pathName);

            string text = File.ReadAllText(pathName);

            string[] temps = pathName.Split('/');

            string newName = temps.Last().Split('.')[0];

            text = text.Replace("BehaviourTreeTaskTemplate", newName);
            File.WriteAllText(pathName, text);

            AssetDatabase.Refresh();

            Selection.objects = AssetDatabase.LoadAllAssetsAtPath(pathName);
        }
    }
}