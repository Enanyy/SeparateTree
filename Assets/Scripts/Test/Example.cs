using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Example : MonoBehaviour
{
    public string desc;

    public List<SceneEntity> entities;

    private Bounds mBounds = new Bounds(Vector3.zero, Vector3.one * 50);

    public Bounds bounds
    {
        get
        {
            mBounds.center = transform.position;
            return mBounds;
        }
    }

    public bool asyn;

    public SeparateDetector detector;

    private SeparateEntityController mController;

    public void Size(Vector3 size)
    {
        mBounds.size = size;
        mBounds.extents = new Vector3(size.x, -size.y, size.z) * 0.5f;
      
    }

    void Start()
    {
        mController = gameObject.GetComponent<SeparateEntityController>();
        if (mController == null)
            mController = gameObject.AddComponent<SeparateEntityController>();
        mController.Init(bounds.center, bounds.size, asyn, SeparateTreeType.QuadTree);


        for (int i = 0; i < entities.Count; i++)
        {
            mController.AddSceneEntity(entities[i]);
        }
    }

    public void AddEntity(SceneEntity entity)
    {
        if (entities.Contains(entity) == false)
        {
            entities.Add(entity);
        }
    }


    void OnGUI()
    {
        GUI.color = Color.red;
        GUILayout.Label(desc);
    }
    
    void Update()
    {
        mController.RefreshDetector(detector);
    }

    void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
        }
        else
        {
            bounds.DrawBounds(Color.green);
        }
    }
}
