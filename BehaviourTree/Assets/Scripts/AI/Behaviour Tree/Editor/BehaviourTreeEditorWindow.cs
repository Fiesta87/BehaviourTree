using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.ProjectWindowCallback;
using System.IO;
using System.Linq;

public class BehaviourTreeEditorWindow : EditorWindow {

	public static BehaviourTreeEditorWindow Instance;
	public static BehaviourTree behaviourTree;

	[SerializeField]
	public BehaviourTree behaviourTreeSerializedSaved;

	private string behaviourTreeAssetFilesPath;
	public BehaviourTreeNode selectedNode;
	private Vector2 nodeSize = new Vector2(180, 60);

	[SerializeField]
	public Vector2 windowMovement = new Vector2(0.0f, 0.0f);
	

	private Color colorBehaviourTreeNode = new Color(0.5f, 0.5f, 0.5f, 1.0f);
	private Color colorControlSelectorNode = new Color(1.0f, 0.6f, 0.2f, 1.0f);
	private Color colorControlSequenceNode = new Color(1.0f, 0.8f, 0.2f, 1.0f);
	private Color colorControlParallelNode = new Color(1.0f, 1.0f, 0.2f, 1.0f);
	private Color colorExecutionNode = new Color(1.0f, 0.0f, 1.0f, 1.0f);
	private Color colorDecoratorNode = new Color(0.0f, 0.5f, 1.0f, 1.0f);
	private Color colorLine = new Color(0.3f, 0.3f, 0.3f, 1.0f);
	private Color colorDefault = new Color(1.0f, 1.0f, 1.0f, 1.0f);

	[SerializeField]
	public float zoomScale = 1.0f;
	private float minZoomForDisplayTitle = 0.4f;

	[SerializeField]
	public string folderToOpenPath = null;

	[SerializeField]
	public GUIStyle style;

    [OnOpenAsset(1)]
    public static bool OnOpenAsset (int instanceID, int line) {

		BehaviourTree behaviourTree = Selection.activeObject as BehaviourTree;

		if (behaviourTree != null) {

			Instance = EditorWindow.GetWindow<BehaviourTreeEditorWindow>("BehaviourTree");

			Instance.InitEditor(behaviourTree);
			Instance.selectedNode = BehaviourTreeEditorWindow.behaviourTree;

			return true; //catch open file
		}        

		return false; // let unity open the file
    }

	[MenuItem("Window/Behaviour Tree")]
	public static void ShowWindow () {
		
		Instance = GetWindow<BehaviourTreeEditorWindow>(false, "BehaviourTree", true);
		Instance.InitEditor(null);
	}

	public void OnEnable () {
		Instance = EditorWindow.GetWindow<BehaviourTreeEditorWindow>("BehaviourTree");
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

			behaviourTreeAssetFilesPath = AssetDatabase.GetAssetPath(BehaviourTreeEditorWindow.behaviourTree); // Assets/Datas/BT/First.asset

			SaveBehaviourTree();

			Selection.activeObject = BehaviourTreeEditorWindow.behaviourTree;
		}
	}

	void OnSelectionChange () {
        if(Selection.activeObject != BehaviourTreeEditorWindow.behaviourTree && Selection.activeObject is BehaviourTree) {
			OnOpenAsset(0, 0);
		}
    }

	void OnGUI () {

		if(BehaviourTreeEditorWindow.behaviourTree == null) {
			return;
		}

		if(folderToOpenPath != null) {
			ProjectViewHelper.ShowFolderContents(folderToOpenPath);
			folderToOpenPath = null;
		}

		style = new GUIStyle(GUI.skin.box);
		style.alignment = TextAnchor.MiddleCenter;
		style.fontSize = 14;
		style.clipping = TextClipping.Clip;
		style.wordWrap = false;

		BeginWindows();

		GUI.color = colorBehaviourTreeNode;
		
		BehaviourTreeEditorWindow.behaviourTree.rect.x = this.position.width / 2 - (nodeSize.x * zoomScale) / 2 + this.windowMovement.x;
		BehaviourTreeEditorWindow.behaviourTree.rect.y = 10.0f + this.windowMovement.y;
		BehaviourTreeEditorWindow.behaviourTree.rect.width = nodeSize.x * zoomScale;
		BehaviourTreeEditorWindow.behaviourTree.rect.height = nodeSize.y * zoomScale;

		BehaviourTreeEditorWindow.behaviourTree.rect = GUI.Window(BehaviourTreeEditorWindow.behaviourTree.ID, BehaviourTreeEditorWindow.behaviourTree.rect, RootWindowFunction, (zoomScale <= minZoomForDisplayTitle) ? "" : BehaviourTreeEditorWindow.behaviourTree.displayedName, style);

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

		// suppr key
		else if(current.type.Equals(EventType.KeyDown) && current.keyCode == KeyCode.Delete) {

			if(this.selectedNode != null && this.selectedNode != BehaviourTreeEditorWindow.behaviourTree) {

				if(this.selectedNode.ChildrenCount() == 0) {
					if(EditorUtility.DisplayDialog("Delete node ?",
						"Are you sure you want to delete the node " + this.selectedNode.displayedName + " ?",
						"Yes, delete this node", "No")) {
						
						DeleteNodeCallback();
					}
				} else {
					if(EditorUtility.DisplayDialog("Delete branch ?",
						"Are you sure you want to delete the node " + this.selectedNode.displayedName + " and his children ?",
						"Yes, delete this branch", "No")) {
						
						DeleteBranchCallback();
					}
				}
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
		} else if(node is BehaviourTreeSubTreeNode) {
			GUI.color = colorBehaviourTreeNode;
		}
	}

	void AddNodeToAssets(BehaviourTreeNode node) {
		AssetDatabase.AddObjectToAsset(node, behaviourTreeAssetFilesPath);
		AssetDatabase.ImportAsset(behaviourTreeAssetFilesPath);
		AssetDatabase.Refresh();
	}

	void NewChildControl (BehaviourTreeControlNode.Type type) {
		BehaviourTreeControlNode newNode = (BehaviourTreeControlNode)ScriptableObject.CreateInstance("BehaviourTreeControlNode");
		newNode.type = type;
		newNode.ID = GetNextWindowID();
		newNode.displayedName = newNode.type.ToString();
		this.selectedNode.AddChild(newNode);
		AddNodeToAssets(newNode);
		SaveBehaviourTree();
		this.selectedNode = newNode;
	}

	void NewChildControlSelectorCallback () {
		NewChildControl(BehaviourTreeControlNode.Type.SELECTOR);
	}

	void NewChildControlSequenceCallback () {
		NewChildControl(BehaviourTreeControlNode.Type.SEQUENCE);
	}

	void NewChildControlParallelCallback () {
		NewChildControl(BehaviourTreeControlNode.Type.PARALLEL);
	}

	void NewChildDecorator (BehaviourTreeDecoratorNode.Type type) {
		BehaviourTreeDecoratorNode newNode = (BehaviourTreeDecoratorNode)ScriptableObject.CreateInstance("BehaviourTreeDecoratorNode");
		newNode.type = type;
		newNode.ID = GetNextWindowID();
		newNode.displayedName = newNode.type.ToString();
		this.selectedNode.AddChild(newNode);
		AddNodeToAssets(newNode);
		SaveBehaviourTree();
		this.selectedNode = newNode;
	}

	void NewChildDecoratorInverterCallback () {
		NewChildDecorator(BehaviourTreeDecoratorNode.Type.INVERTER);
	}

	void NewChildDecoratorSucceederCallback () {
		NewChildDecorator(BehaviourTreeDecoratorNode.Type.SUCCEEDER);
	}

	void NewChildDecoratorLoserCallback () {
		NewChildDecorator(BehaviourTreeDecoratorNode.Type.LOSER);
	}

	void NewChildDecoratorIgnoreSuccessCallback () {
		NewChildDecorator(BehaviourTreeDecoratorNode.Type.IGNORE_SUCCESS);
	}

	void NewChildDecoratorIgnoreFailureCallback () {
		NewChildDecorator(BehaviourTreeDecoratorNode.Type.IGNORE_FAILURE);
	}

	void NewParentControl (BehaviourTreeControlNode.Type type) {
		BehaviourTreeControlNode newNode = (BehaviourTreeControlNode)ScriptableObject.CreateInstance("BehaviourTreeControlNode");
		newNode.type = type;
		newNode.ID = GetNextWindowID();
		newNode.displayedName = newNode.type.ToString().Replace('_', ' ');
		BehaviourTreeNode parent = FindParentOfNodeByID(BehaviourTreeEditorWindow.behaviourTree, this.selectedNode.ID);
		parent.ReplaceChild(this.selectedNode, newNode);
		newNode.AddChild(this.selectedNode);
		AddNodeToAssets(newNode);
		SaveBehaviourTree();
		this.selectedNode = newNode;
	}

	void NewParentControlSelectorCallback () {
		NewParentControl(BehaviourTreeControlNode.Type.SELECTOR);
	}

	void NewParentControlSequenceCallback () {
		NewParentControl(BehaviourTreeControlNode.Type.SEQUENCE);
	}

	void NewParentControlParallelCallback () {
		NewParentControl(BehaviourTreeControlNode.Type.PARALLEL);
	}

	void NewParentDecorator (BehaviourTreeDecoratorNode.Type type) {
		BehaviourTreeDecoratorNode newNode = (BehaviourTreeDecoratorNode)ScriptableObject.CreateInstance("BehaviourTreeDecoratorNode");
		newNode.type = type;
		newNode.ID = GetNextWindowID();
		newNode.displayedName = newNode.type.ToString().Replace('_', ' ');
		BehaviourTreeNode parent = FindParentOfNodeByID(BehaviourTreeEditorWindow.behaviourTree, this.selectedNode.ID);
		parent.ReplaceChild(this.selectedNode, newNode);
		newNode.AddChild(this.selectedNode);
		AddNodeToAssets(newNode);
		SaveBehaviourTree();
		this.selectedNode = newNode;
	}

	void NewParentDecoratorInverterCallback () {
		NewParentDecorator(BehaviourTreeDecoratorNode.Type.INVERTER);
	}

	void NewParentDecoratorSucceederCallback () {
		NewParentDecorator(BehaviourTreeDecoratorNode.Type.SUCCEEDER);
	}

	void NewParentDecoratorLoserCallback () {
		NewParentDecorator(BehaviourTreeDecoratorNode.Type.LOSER);
	}

	void NewParentDecoratorIgnoreSuccessCallback () {
		NewParentDecorator(BehaviourTreeDecoratorNode.Type.IGNORE_SUCCESS);
	}

	void NewParentDecoratorIgnoreFailureCallback () {
		NewParentDecorator(BehaviourTreeDecoratorNode.Type.IGNORE_FAILURE);
	}

	void ExecutionCallback () {
		BehaviourTreeExecutionNode newNode = (BehaviourTreeExecutionNode)ScriptableObject.CreateInstance("BehaviourTreeExecutionNode");
		newNode.ID = GetNextWindowID();
		this.selectedNode.AddChild(newNode);
		newNode.displayedName = "EXECUTION";
		AddNodeToAssets(newNode);
		SaveBehaviourTree();
		this.selectedNode = newNode;
	}

	void SubTreeCallback () {
		BehaviourTreeSubTreeNode newNode = (BehaviourTreeSubTreeNode)ScriptableObject.CreateInstance("BehaviourTreeSubTreeNode");
		newNode.ID = GetNextWindowID();
		this.selectedNode.AddChild(newNode);
		newNode.displayedName = "SUB TREE";
		AddNodeToAssets(newNode);
		SaveBehaviourTree();
		this.selectedNode = newNode;
	}

	void DeleteNodeCallback () {
		if( ! EditorUtility.DisplayDialog("Delete node ?",
			"Are you sure you want to delete the node " + this.selectedNode.displayedName + " ?",
			"Yes, delete this node", "No")) {
			
			return;
		}

		BehaviourTreeNode parent = FindParentOfNodeByID(BehaviourTreeEditorWindow.behaviourTree, this.selectedNode.ID);

		if(this.selectedNode.ChildrenCount() > 0) {
			parent.ReplaceChild(this.selectedNode, this.selectedNode.GetChildren()[0]);
		} else {
			parent.RemoveChild(this.selectedNode);
		}

		DeleteNodeAsset(this.selectedNode);
		AssetDatabase.Refresh();
		SaveBehaviourTree();
	}

	void DeleteBranchCallback () {
		if( ! EditorUtility.DisplayDialog("Delete branch ?",
			"Are you sure you want to delete the node " + this.selectedNode.displayedName + " and his children ?",
			"Yes, delete this branch", "No")) {
			
			return;
		}
		
		BehaviourTreeNode parent = FindParentOfNodeByID(BehaviourTreeEditorWindow.behaviourTree, this.selectedNode.ID);
		parent.RemoveChild(selectedNode);
		DeleteRecursivelyBranchAssets(selectedNode);
		AssetDatabase.Refresh();
		SaveBehaviourTree();
	}

	void DeleteRecursivelyBranchAssets (BehaviourTreeNode node) {

		DeleteNodeAsset(node);

		foreach(BehaviourTreeNode child in node.GetChildren()) {
			DeleteRecursivelyBranchAssets(child);
		}
	}

	void DeleteNodeAsset (BehaviourTreeNode node) {

		if(node is BehaviourTreeExecutionNode) {
			BehaviourTreeExecutionNode execNode = (BehaviourTreeExecutionNode)node;
			if(execNode.task != null) {
				DestroyImmediate(execNode.task, true);
			}
		}

		DestroyImmediate(node, true);
		AssetDatabase.ImportAsset(behaviourTreeAssetFilesPath);
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
		} else if (node is BehaviourTreeSubTreeNode) {
			func = SubTreeWindowFunction;
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
		node.rect = GUI.Window (node.ID, node.rect, func, (zoomScale <= minZoomForDisplayTitle) ? "" : node.displayedName, style);
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

	void SelectNodeButDontChangeProjectView () {
		string path = AssetDatabase.GetAssetPath(Selection.activeObject);

		if(!Directory.Exists(path)) {
            if (File.Exists(path)) {
                path = Path.GetDirectoryName(path);
            }
            if (string.IsNullOrEmpty(path)){
                path = Path.GetDirectoryName(AssetDatabase.GetAssetPath(BehaviourTreeEditorWindow.behaviourTree));
            }
        }

		folderToOpenPath = path;

		Selection.activeObject = BehaviourTreeEditorWindow.behaviourTree;
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

			if(Selection.activeObject != BehaviourTreeEditorWindow.behaviourTree) {
				SelectNodeButDontChangeProjectView();
			}

			current.Use();
		}
    }

    void ControlWindowFunction (int windowID) {

		BehaviourTreeControlNode node = (BehaviourTreeControlNode)FindNodeByID(BehaviourTreeEditorWindow.behaviourTree, windowID);

		Event current = Event.current;

		if(current.type == EventType.MouseDown && current.button == 1) {

			this.selectedNode = node;

			if(Selection.activeObject != BehaviourTreeEditorWindow.behaviourTree) {
				SelectNodeButDontChangeProjectView();
			}
			
			GenericMenu menu = new GenericMenu();

			AddNewChildOption(menu, true);
			AddInsertNewParentOptions(menu);
			AddMoveOption(menu);
			if(this.selectedNode.ChildrenCount() <= 1) {
				menu.AddItem(new GUIContent("Delete Node"), false, DeleteNodeCallback);
				menu.AddDisabledItem(new GUIContent("Delete Branch"));
			} else {
				menu.AddDisabledItem(new GUIContent("Delete Node"));
				menu.AddItem(new GUIContent("Delete Branch"), false, DeleteBranchCallback);
			}
			
			menu.ShowAsContext();
			
			current.Use();

		} else if(current.type == EventType.MouseDown && current.button == 0) {

			this.selectedNode = node;

			if(Selection.activeObject != BehaviourTreeEditorWindow.behaviourTree) {
				SelectNodeButDontChangeProjectView();
			}

			current.Use();
		}
    }

	void ExecutionWindowFunction (int windowID) {

		BehaviourTreeExecutionNode node = (BehaviourTreeExecutionNode)FindNodeByID(BehaviourTreeEditorWindow.behaviourTree, windowID);

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

			if(Selection.activeObject != BehaviourTreeEditorWindow.behaviourTree) {
				SelectNodeButDontChangeProjectView();
			}
			
			GenericMenu menu = new GenericMenu();

			AddInsertNewParentOptions(menu);
			AddMoveOption(menu);
			menu.AddItem(new GUIContent("Delete Node"), false, DeleteNodeCallback);
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

			BehaviourTreeTask oldTaskToRemove = node.task;
			
			node.task = so as BehaviourTreeTask;

			node.contextLink.Clear();

			PropertyReader.Variable[] variables = PropertyReader.GetFields(node.task.GetType());

			foreach(PropertyReader.Variable variable in variables) {
				if(variable.name.StartsWith("in_") || variable.name.StartsWith("out_")) {
					node.contextLink.Add(variable.name, variable.name.Split('_')[1]);
				}
			}

			node.displayedName = taskType.ToString();

			AddTaskToAssets(node, oldTaskToRemove);

			BehaviourTreeEditorWindow.SaveBehaviourTree();

			this.selectedNode = node;

			Selection.activeObject = taskScriptAsset;

			SelectNodeButDontChangeProjectView();

			current.Use();

		} else if(current.type == EventType.MouseDown && current.button == 0 && current.clickCount == 2) {

			this.selectedNode = node;

			if(Selection.activeObject != BehaviourTreeEditorWindow.behaviourTree) {
				SelectNodeButDontChangeProjectView();
			}
			
			MonoScript monoscript = MonoScript.FromScriptableObject(node.task);
			string scriptPath = AssetDatabase.GetAssetPath(monoscript);
			UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(scriptPath, 0);

			current.Use();

		} else if(current.type == EventType.MouseDown && current.button == 0) {

			this.selectedNode = node;

			if(Selection.activeObject != BehaviourTreeEditorWindow.behaviourTree) {
				SelectNodeButDontChangeProjectView();
			}

			current.Use();
		}
    }

	void DecoratorWindowFunction (int windowID) {

		BehaviourTreeDecoratorNode node = (BehaviourTreeDecoratorNode)FindNodeByID(BehaviourTreeEditorWindow.behaviourTree, windowID);

		Event current = Event.current;

		if(current.type == EventType.MouseDown && current.button == 1) {

			this.selectedNode = node;

			if(Selection.activeObject != BehaviourTreeEditorWindow.behaviourTree) {
				SelectNodeButDontChangeProjectView();
			}
			
			GenericMenu menu = new GenericMenu();

			AddNewChildOption(menu, node.child == null);
			AddInsertNewParentOptions(menu);
			AddMoveOption(menu);
			menu.AddItem(new GUIContent("Delete Node"), false, DeleteNodeCallback);
			menu.AddItem(new GUIContent("Delete Branch"), false, DeleteBranchCallback);
			menu.ShowAsContext();
			
			current.Use();

		} else if(current.type == EventType.MouseDown && current.button == 0) {

			this.selectedNode = node;

			if(Selection.activeObject != BehaviourTreeEditorWindow.behaviourTree) {
				SelectNodeButDontChangeProjectView();
			}

			current.Use();
		}
    }

	void SubTreeWindowFunction (int windowID) {
		BehaviourTreeSubTreeNode node = (BehaviourTreeSubTreeNode)FindNodeByID(BehaviourTreeEditorWindow.behaviourTree, windowID);

		Event current = Event.current;

		if(current.mousePosition.x >= 0.0f && current.mousePosition.x <= nodeSize.x * zoomScale &&
		current.mousePosition.y >= 0.0f && current.mousePosition.y <= nodeSize.y * zoomScale &&
		DragAndDrop.objectReferences.Length == 1) {

			Object subTreeScriptAsset = DragAndDrop.objectReferences[0];

			if( ! (subTreeScriptAsset is MonoScript)) {
				DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
			} else {
				DragAndDrop.visualMode = DragAndDropVisualMode.Link;
			}
		}

		if(current.type == EventType.MouseDown && current.button == 1) {

			this.selectedNode = node;

			if(Selection.activeObject != BehaviourTreeEditorWindow.behaviourTree) {
				SelectNodeButDontChangeProjectView();
			}
			
			GenericMenu menu = new GenericMenu();

			AddInsertNewParentOptions(menu);
			AddMoveOption(menu);
			menu.AddItem(new GUIContent("Delete Node"), false, DeleteNodeCallback);
			menu.ShowAsContext();
			
			current.Use();

		} else if(current.type.Equals(EventType.DragExited)) {

			Object subTreeScriptAsset = DragAndDrop.objectReferences[0];

			if( ! (subTreeScriptAsset is MonoScript)) {
				Debug.Log("not a MonoScript");
				current.Use();
				return;
			}

			System.Type subTreeType = (subTreeScriptAsset as MonoScript).GetClass();

			ScriptableObject so = ScriptableObject.CreateInstance(subTreeType);

			if( ! (so is BehaviourTree)) {
				Debug.Log("not a BehaviourTree");
				current.Use();
				return;
			}
			
			node.subTree = so as BehaviourTree;

			node.displayedName = subTreeType.ToString();

			BehaviourTreeEditorWindow.SaveBehaviourTree();

			this.selectedNode = node;

			if(Selection.activeObject != BehaviourTreeEditorWindow.behaviourTree) {
				SelectNodeButDontChangeProjectView();
			}

			current.Use();

		} else if(current.type == EventType.MouseDown && current.button == 0 && current.clickCount == 2) {

			Selection.activeObject = node.subTree;

			OnOpenAsset(0, 0);

			current.Use();

		} else if(current.type == EventType.MouseDown && current.button == 0) {

			this.selectedNode = node;

			if(Selection.activeObject != BehaviourTreeEditorWindow.behaviourTree) {
				SelectNodeButDontChangeProjectView();
			}

			current.Use();
		}
	}

	void AddNewChildOption (GenericMenu menu, bool enabled) {

		if(enabled) {
			menu.AddItem(new GUIContent("New Child/Control/Selector"), false, NewChildControlSelectorCallback);
			menu.AddItem(new GUIContent("New Child/Control/Sequence"), false, NewChildControlSequenceCallback);
			menu.AddItem(new GUIContent("New Child/Control/Parallel"), false, NewChildControlParallelCallback);
			menu.AddItem(new GUIContent("New Child/Decorator/Inverter"), false, NewChildDecoratorInverterCallback);
			menu.AddItem(new GUIContent("New Child/Decorator/Succeeder"), false, NewChildDecoratorSucceederCallback);
			menu.AddItem(new GUIContent("New Child/Decorator/Loser"), false, NewChildDecoratorLoserCallback);
			menu.AddItem(new GUIContent("New Child/Decorator/Ignore Success"), false, NewChildDecoratorIgnoreSuccessCallback);
			menu.AddItem(new GUIContent("New Child/Decorator/Ignore Failure"), false, NewChildDecoratorIgnoreFailureCallback);
			menu.AddItem(new GUIContent("New Child/Execution"), false, ExecutionCallback);
			menu.AddItem(new GUIContent("New Child/Sub-tree"), false, SubTreeCallback);
		} else {
			menu.AddDisabledItem(new GUIContent("New Child"));
		}
		
	}

	void AddInsertNewParentOptions (GenericMenu menu) {
		menu.AddItem(new GUIContent("Insert New Parent/Control/Selector"), false, NewParentControlSelectorCallback);
		menu.AddItem(new GUIContent("Insert New Parent/Control/Sequence"), false, NewParentControlSequenceCallback);
		menu.AddItem(new GUIContent("Insert New Parent/Control/Parallel"), false, NewParentControlParallelCallback);
		menu.AddItem(new GUIContent("Insert New Parent/Decorator/Inverter"), false, NewParentDecoratorInverterCallback);
		menu.AddItem(new GUIContent("Insert New Parent/Decorator/Succeeder"), false, NewParentDecoratorSucceederCallback);
		menu.AddItem(new GUIContent("Insert New Parent/Decorator/Loser"), false, NewParentDecoratorLoserCallback);
		menu.AddItem(new GUIContent("Insert New Parent/Decorator/Ignore Success"), false, NewParentDecoratorIgnoreSuccessCallback);
		menu.AddItem(new GUIContent("Insert New Parent/Decorator/Ignore Failure"), false, NewParentDecoratorIgnoreFailureCallback);
	}

	void AddMoveOption (GenericMenu menu) {
		menu.AddItem(new GUIContent("Move/First"), false, MoveFirstCallback);
		menu.AddItem(new GUIContent("Move/Left"), false, MoveLeftCallback);
		menu.AddItem(new GUIContent("Move/Right"), false, MoveRightCallback);
		menu.AddItem(new GUIContent("Move/Last"), false, MoveLastCallback);
	}

	/*********************************************************************************/
	/****************************** MOVE NODE CALLBACKS ******************************/
	/*********************************************************************************/

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

		string templatePath = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(ScriptableObject.CreateInstance("BehaviourTreeTaskTemplate")));
        
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<EditFileContent>(), filePath, EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D, templatePath);
    }

    internal class EditFileContent : EndNameEditAction {

        public override void Action (int instanceId, string pathName, string resourceFile) {

            File.Copy(resourceFile, pathName);

            string text = File.ReadAllText(pathName);

            string[] temps = pathName.Split('/');

            string newName = temps.Last().Split('.')[0];

            text = text.Replace("BehaviourTreeTaskTemplate", newName);
            File.WriteAllText(pathName, text);

            AssetDatabase.Refresh();

            Selection.objects = AssetDatabase.LoadAllAssetsAtPath(pathName);
        }
    }

	void AddTaskToAssets (BehaviourTreeExecutionNode node, BehaviourTreeTask oldTaskToRemove) {

		if(oldTaskToRemove != null) {
			DestroyImmediate(oldTaskToRemove, true);
		}

		AssetDatabase.AddObjectToAsset(node.task, behaviourTreeAssetFilesPath);
		AssetDatabase.ImportAsset(behaviourTreeAssetFilesPath);
		AssetDatabase.Refresh();
	}
}