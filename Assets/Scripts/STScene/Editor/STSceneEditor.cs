using System;
using UnityEngine;
using UnityEditor;
using System.IO;
using Mono.Xml;


public class STSceneEditor
{
    [MenuItem("STScene/Open")]
    static void OpenSTScene()
    {
        string path = EditorUtility.OpenFilePanel("Select a scene config", Application.dataPath, "txt");
        Debug.Log(path);

       
        string text = File.ReadAllText(path);
        

        var scene = GameObject.FindObjectOfType<STScene>();
        if(scene == null)
        {
            GameObject go = new GameObject(typeof(STScene).ToString());
            scene = go.AddComponent<STScene>();
            go.transform.position = Vector3.zero;
            go.transform.rotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;
        }

        for (int i = scene.transform.childCount - 1; i >= 0; --i)
        {
            GameObject.DestroyImmediate(scene.transform.GetChild(i).gameObject);
        }
        scene.attribute.entities.Clear();

        scene.LoadXml(text);
       

    }
}


