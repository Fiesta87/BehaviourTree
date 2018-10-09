using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public abstract class BehaviourTreeTask {

	public abstract BehaviourTree.Status Begin ();
	public abstract BehaviourTree.Status Update ();
	public abstract void Finish ();

	[MenuItem("Assets/Create/Behaviour Tree/Task")]
    public static void CreateTask () {

        string path = AssetDatabase.GetAssetPath(Selection.activeObject);

        if (File.Exists(path)) {
            path = Path.GetDirectoryName(path);
        }
        if (string.IsNullOrEmpty(path)){
            path = "Assets/";
        }
        
        File.Copy("Assets/Scripts/AI/Behaviour Tree/BehaviourTreeTaskTemplate.cs", Path.Combine(path, "NewScript.cs"));
        AssetDatabase.Refresh();
    }
}
