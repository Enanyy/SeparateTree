using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(SceneEntity))]
public class SceneEntityEditor:Editor
{
    private SceneEntity mTarget;

    void OnEnable()
    {
        mTarget = target as SceneEntity;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (mTarget == null)
        {
            return;
        }



        mTarget.Size(EditorGUILayout.Vector3Field("Size", mTarget.bounds.size));

        mTarget.mPath = EditorGUILayout.TextField("Path", mTarget.mPath);
        if (string.IsNullOrEmpty(mTarget.mPath) == false)
        {
            if (GUILayout.Button("Load"))
            {
                if (mTarget.mGo)
                {
                    DestroyImmediate(mTarget.mGo);
                }

                mTarget.OnShow();
            }
        }

        if (mTarget.mGo != null)
        {
            if (GUILayout.Button("Clear"))
            {
                if (mTarget.mGo)
                {
                    DestroyImmediate(mTarget.mGo);
                }

            }
        }
    }
}

