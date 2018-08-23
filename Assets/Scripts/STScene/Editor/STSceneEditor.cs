using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof (STScene))]
public class STSceneEditor : Editor
{
    private STScene mTarget;

    void OnEnable()
    {
        mTarget = target as STScene;
    }

    public override void OnInspectorGUI()
    {

        mTarget.Size(EditorGUILayout.Vector3Field("Size", mTarget.bounds.size));

        base.OnInspectorGUI();

        if(mTarget == null)
        {
            return;
        }

        if (GUILayout.Button("Add Entity"))
        {
            AddSceneEntity();
        }

        for(int i = mTarget.entities.Count - 1; i  >=0; i --)
        {
            if(mTarget.entities[i] == null)
            {
                mTarget.entities.RemoveAt(i);
            }
        }

        if (mTarget.transform.childCount == 0)
        {
            if (GUILayout.Button("Test Entity"))
            {
                TestSceneEntity();
            }
        }

        if(mTarget.transform.childCount > 0)
        {
            if (GUILayout.Button("Clear Entity"))
            {
                ClearEntity();
            }
        }
    }


    private void AddSceneEntity()
    {
        GameObject go = new GameObject("entity-" + (mTarget.entities.Count+1));

        go.transform.SetParent(mTarget.transform);
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;

        STSceneEntity entity = go.AddComponent<STSceneEntity>();

        mTarget.AddEntity(entity);
    }

    private void TestSceneEntity()
    {
        Vector3 size = mTarget.bounds.size * 0.5f;
        for(int i = 0; i < 200; i++)
        {
            float x = new System.Random(Guid.NewGuid().GetHashCode()).Next(-(int)size.x, (int)size.x);
            float z = new System.Random(Guid.NewGuid().GetHashCode()).Next(-(int)size.z, (int)size.z);

            GameObject go = new GameObject("entity-" + (mTarget.entities.Count + 1));

            go.transform.SetParent(mTarget.transform);
            go.transform.localPosition = new Vector3(x, 0, z);
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;

            STSceneEntity entity = go.AddComponent<STSceneEntity>();
            int scale = new System.Random(Guid.NewGuid().GetHashCode()).Next(1, 4);
            entity.Size(new Vector3(scale, 0, scale));
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

            entity.path = "Prefabs/" + prefab;
            entity.localScale = Vector3.one * scale;
            entity.localPosition = new Vector3(0, y, 0);

           mTarget.AddEntity(entity);
        }
    }

    private void ClearEntity()
    {
        for (int i = mTarget.transform.childCount -1; i >=0 ; --i)
        {
            DestroyImmediate(mTarget.transform.GetChild(i).gameObject);
        }
        mTarget.entities.Clear();
    }
}