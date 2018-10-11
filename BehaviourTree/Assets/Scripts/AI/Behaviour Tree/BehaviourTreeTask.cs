using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using System.IO;
using System.Linq;

/// <summary>
/// This class is an executable task in a behaviour tree.
/// </summary>
public abstract class BehaviourTreeTask : ScriptableObject {

	public abstract void Begin ();
	public abstract BehaviourTree.Status Update ();
	public abstract void FinishSuccess ();
	public abstract void FinishFailure ();



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

        // ProjectWindowUtil.CreateAsset((BehaviourTreeTaskTemplate)ScriptableObject.CreateInstance("BehaviourTreeTaskTemplate"), path);

        // DirectoryInfo directory = new DirectoryInfo("C:\\MyDirectory");

        // FileInfo myFile = (from f in directory.GetFiles() orderby f.LastWriteTime descending select f).First();

        // Debug.Log(myFile.FullName);
        
        string filePath = Path.Combine(path, "NewTask.cs");
        // File.Copy("Assets/Scripts/AI/Behaviour Tree/BehaviourTreeTaskTemplate.cs", filePath);
        // AssetDatabase.Refresh();

        // Object fileObject = AssetDatabase.LoadAllAssetsAtPath(filePath)[0];

        // Debug.Log(AssetDatabase.LoadAllAssetsAtPath(filePath).Length);

        // string text = File.ReadAllText(filePath);
        // text = text.Replace("BehaviourTreeTaskTemplate", "NewTask");
        // File.WriteAllText(filePath, text);

        // EditFileContent action = new EditFileContent();

        // ProjectWindowUtil.StartNameEditingIfProjectWindowExists(fileObject.GetInstanceID(), new EditFileContent(), filePath, /*AssetPreview.GetMiniThumbnail(fileObject)*/null, null);
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<EditFileContent>(), filePath, EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D, "Assets/Scripts/AI/Behaviour Tree/BehaviourTreeTaskTemplate.cs");
    }

    internal class EditFileContent : EndNameEditAction {
        
        // private string pathName;

        // public EditFileContent(string pathName) {
        //     this.pathName = pathName;
        // }
        public override void Action(int instanceId, string pathName, string resourceFile) {

            File.Copy("Assets/Scripts/AI/Behaviour Tree/BehaviourTreeTaskTemplate.cs", pathName);

            string text = File.ReadAllText(pathName);

            string[] temps = pathName.Split('/');

            string newName = temps.Last().Split('.')[0];

            text = text.Replace("BehaviourTreeTaskTemplate", newName);
            File.WriteAllText(pathName, text);

            AssetDatabase.Refresh();

            // Object fileObject = [0];

            Selection.objects = AssetDatabase.LoadAllAssetsAtPath(pathName);

            // Object o = ProjectWindowUtil.CreateScriptAssetFromTemplate(pathName, resourceFile);
            //     ProjectWindowUtil.ShowCreatedAsset(o);
        }
    }
}
