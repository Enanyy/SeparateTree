using UnityEngine;
using UnityEditor;

[CustomEditor(typeof (Example))]
public class ExampleEditor : Editor
{
    private Example mTarget;

    void OnEnable()
    {
        mTarget = target as Example;
    }

    public override void OnInspectorGUI()
    {

        mTarget.Size(EditorGUILayout.Vector3Field("Size", mTarget.bounds.size));

        base.OnInspectorGUI();


        if (GUILayout.Button("Add SceneEntity"))
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
    }


    private void AddSceneEntity()
    {
        GameObject go = new GameObject("entity-" + (mTarget.entities.Count+1));

        go.transform.SetParent(mTarget.transform);
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;

        SceneEntity entity = go.AddComponent<SceneEntity>();

        mTarget.AddEntity(entity);
    }

}