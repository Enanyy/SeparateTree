using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SeparateEntityController))]
public class SeparateEntityControllerEditor : Editor {

    private SeparateEntityController mTarget;

    void OnEnable()
    {
        mTarget = (SeparateEntityController) target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
#if UNITY_EDITOR
        GUILayout.Label("调试：");
        bool drawTree = GUILayout.Toggle(mTarget.debug_DrawMinDepth >= 0 && mTarget.debug_DrawMaxDepth >= 0, "显示四叉树包围盒");
        if (drawTree == false)
        {
            mTarget.debug_DrawMaxDepth = -1;
            mTarget.debug_DrawMinDepth = -1;
        }
        else
        {
            mTarget.debug_DrawMaxDepth = mTarget.debug_DrawMaxDepth < 0 ? 0 : mTarget.debug_DrawMaxDepth;
            mTarget.debug_DrawMinDepth = mTarget.debug_DrawMinDepth < 0 ? 0 : mTarget.debug_DrawMinDepth;
        }
        mTarget.debug_DrawEntity = GUILayout.Toggle(mTarget.debug_DrawEntity, "显示场景对象包围盒");
        if (drawTree)
        {
            GUILayout.Label("显示四叉树深度范围：");
            mTarget.debug_DrawMinDepth = Mathf.Max(0, EditorGUILayout.IntField("最小深度", mTarget.debug_DrawMinDepth));
            mTarget.debug_DrawMaxDepth = Mathf.Max(0, EditorGUILayout.IntField("最大深度", mTarget.debug_DrawMaxDepth));
        }
#endif
    }
}
