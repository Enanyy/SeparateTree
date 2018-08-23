﻿using UnityEngine;
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
        mTarget.attribute.name = EditorGUILayout.TextField("Name", mTarget.attribute.name);
        mTarget.attribute.bounds.size = EditorGUILayout.Vector3Field("Size", mTarget.attribute.bounds.size);
        mTarget.attribute.asyn = EditorGUILayout.Toggle("asyn", mTarget.attribute.asyn);

        base.OnInspectorGUI();

        if(mTarget == null)
        {
            return;
        }

        if (GUILayout.Button("Add Entity"))
        {
            AddSceneEntity();
        }

        for(int i = mTarget.attribute.entities.Count - 1; i  >=0; i --)
        {
            if(mTarget.attribute.entities[i] == null)
            {
                mTarget.attribute.entities.RemoveAt(i);
            }
        }

        if (mTarget.transform.childCount == 0)
        {
            if (GUILayout.Button("Auto Create Entity"))
            {
                AutoCreateSceneEntity();
            }
        }

        if(mTarget.transform.childCount > 0)
        {
            if (GUILayout.Button("Clear Entity"))
            {
                ClearEntity();
            }
        }

        if (GUILayout.Button("Save Xml"))
        {
            SaveXml();
        }
    }


    private void AddSceneEntity()
    {
        GameObject go = new GameObject("entity-" + (mTarget.attribute.entities.Count+1));

        go.transform.SetParent(mTarget.transform);
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;

        STSceneEntity entity = go.AddComponent<STSceneEntity>();

        mTarget.AddEntity(entity);
    }

    private void AutoCreateSceneEntity()
    {
        Vector3 size = mTarget.bounds.size * 0.5f;
        for(int i = 0; i < 200; i++)
        {
            float x = new System.Random(Guid.NewGuid().GetHashCode()).Next(-(int)size.x, (int)size.x);
            float z = new System.Random(Guid.NewGuid().GetHashCode()).Next(-(int)size.z, (int)size.z);

            GameObject go = new GameObject("entity-" + (mTarget.attribute.entities.Count + 1));

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

           mTarget.AddEntity(entity);
        }
    }

    private void ClearEntity()
    {
        for (int i = mTarget.transform.childCount -1; i >=0 ; --i)
        {
            DestroyImmediate(mTarget.transform.GetChild(i).gameObject);
        }
        mTarget.attribute.entities.Clear();
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

        string path = EditorUtility.SaveFilePanel("导出场景配置文件", Application.dataPath, "STScene" , "txt");
        EditorUtility.DisplayProgressBar("请稍候", "正在导出场景配置文件", 0.1f);
        if (!string.IsNullOrEmpty(path))
        {
            File.WriteAllText(path, xml, System.Text.Encoding.UTF8);

            ClearEntity();

        }
        EditorUtility.ClearProgressBar();
    }

}