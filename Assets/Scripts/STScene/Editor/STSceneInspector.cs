using UnityEngine;
using UnityEditor;
using System;
using System.Xml;
using System.IO;
using System.Collections.Generic;

[CustomEditor(typeof (STScene))]
public class STSceneInspector : Editor
{
    private STScene mTarget;

    void OnEnable()
    {
        mTarget = target as STScene;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (mTarget == null)
        {
            return;
        }
        mTarget.attribute.name = EditorGUILayout.TextField("Name", mTarget.attribute.name);
        mTarget.attribute.bounds.size = EditorGUILayout.Vector3Field("Size", mTarget.attribute.bounds.size);
        mTarget.attribute.asyn = EditorGUILayout.Toggle("asyn", mTarget.attribute.asyn);
        mTarget.attribute.maxCreateCount = EditorGUILayout.IntField("maxCreateCount", mTarget.attribute.maxCreateCount);
        mTarget.attribute.minCreateCount = EditorGUILayout.IntField("minCreateCount", mTarget.attribute.minCreateCount);
        mTarget.attribute.refreshTime = EditorGUILayout.FloatField("refreshTime", mTarget.attribute.refreshTime);
        mTarget.attribute.destroyTime = EditorGUILayout.FloatField("destroyTime", mTarget.attribute.destroyTime);
        mTarget.attribute.treeDepth = EditorGUILayout.IntField("treeDepth", mTarget.attribute.treeDepth);

        if (GUILayout.Button("Add Ground"))
        {
            AddSTComponent<STSceneGround>();
        }
        if (GUILayout.Button("Add Entity"))
        {
            AddSTComponent<STSceneEntity>();
        }


        for(int i = mTarget.attribute.components.Count - 1; i  >=0; i --)
        {
            if(mTarget.attribute.components[i] == null)
            {
                mTarget.attribute.components.RemoveAt(i);
            }
        }

        if (mTarget.transform.childCount == 0)
        {
            if (GUILayout.Button("Auto Create Entity"))
            {
                AutoCreateSceneEntity();
            }

            if (GUILayout.Button("Load Xml"))
            {
                LoadXml();
            }
        }

        if (mTarget.transform.childCount > 0)
        {
            if (GUILayout.Button("Clear"))
            {
                ClearEntity();
            }
            if (GUILayout.Button("Save Xml"))
            {
                SaveXml();
            }
        }
       
    }


    private void AddSTComponent<T>() where T:STComponent
    {
        GameObject go = new GameObject(typeof(T).ToString());

        go.transform.SetParent(mTarget.transform);
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;

        STComponent component = go.AddComponent<T>() as STComponent;

        mTarget.AddSTComponent(component);
    }

    private void AutoCreateSceneEntity()
    {
        Vector3 size = mTarget.bounds.size * 0.5f;
        for(int i = 0; i < 300; i++)
        {
            float x = new System.Random(Guid.NewGuid().GetHashCode()).Next(-(int)size.x, (int)size.x);
            float z = new System.Random(Guid.NewGuid().GetHashCode()).Next(-(int)size.z, (int)size.z);

            GameObject go = new GameObject("entity-" + (mTarget.attribute.components.Count + 1));

            go.transform.SetParent(mTarget.transform);
            go.transform.localPosition = new Vector3(x, 0, z);
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;

            STSceneEntity entity = go.AddComponent<STSceneEntity>();
            int scale = new System.Random(Guid.NewGuid().GetHashCode()).Next(1, 4);
            entity.attribute.bounds.size = (new Vector3(scale, 0, scale));
            string prefab = "Capsule";
            float y = scale;
            if(i % 3 == 1)
            {
                prefab = "Cube";
                y = scale * 0.5f;
            }
            else if( i % 3 == 2)
            {
                prefab = "Sphere";
                y = scale * 0.5f;
            }

            entity.attribute.path = "Prefabs/" + prefab;
            entity.attribute.localScale = Vector3.one * scale;
            entity.attribute.localPosition = new Vector3(0, y, 0);

           mTarget.AddSTComponent(entity);
        }
    }

    private void ClearEntity()
    {
        for (int i = mTarget.transform.childCount -1; i >=0 ; --i)
        {
            DestroyImmediate(mTarget.transform.GetChild(i).gameObject);
        }
        mTarget.attribute.components.Clear();
    }

    private void SaveXml()
    {
        XmlDocument doc = new XmlDocument();
        XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", "utf-8", "yes");
        doc.InsertBefore(dec, doc.DocumentElement);

        doc.AppendChild(mTarget.ToXml(doc));

        MemoryStream ms = new MemoryStream();
        XmlTextWriter xw = new XmlTextWriter(ms, System.Text.Encoding.UTF8);
        xw.Formatting = Formatting.Indented;
        doc.Save(xw);

        ms = (MemoryStream)xw.BaseStream;
        byte[] bytes = ms.ToArray();
        string xml = System.Text.Encoding.UTF8.GetString(bytes, 3, bytes.Length - 3);

        string path = EditorUtility.SaveFilePanel("导出场景配置文件", Application.dataPath + "/Resources/Congfigs/Scene/", "STScene" , "txt");
        EditorUtility.DisplayProgressBar("请稍候", "正在导出场景配置文件", 0.1f);
        if (!string.IsNullOrEmpty(path))
        {
            File.WriteAllText(path, xml, System.Text.Encoding.UTF8);

            ClearEntity();

        }
        EditorUtility.ClearProgressBar();
    }

    [MenuItem("STScene/Open")]
    static void LoadXml()
    {
        string path = EditorUtility.OpenFilePanel("Select a scene config", Application.dataPath+"/Resources/Congfigs/Scene/", "txt");

        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        Debug.Log(path);


        string text = File.ReadAllText(path);


        var scene = GameObject.FindObjectOfType<STScene>();
        if (scene == null)
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
        scene.attribute.components.Clear();

        scene.LoadXml(text);


    }
}