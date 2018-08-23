using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(STSceneEntity))]
public class STSceneEntityEditor:Editor
{
    private STSceneEntity mTarget;

    void OnEnable()
    {
        mTarget = target as STSceneEntity;
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

