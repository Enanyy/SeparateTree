using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class STScene : MonoBehaviour
{
    public string desc;

    [SerializeField]
    public List<STSceneEntity> entities;

    private Bounds mBounds = new Bounds(Vector3.zero, new Vector3(200,0,200));

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
    }

    void Start()
    {
        mController = gameObject.GetComponent<SeparateEntityController>();
        if (mController == null)
            mController = gameObject.AddComponent<SeparateEntityController>();
        mController.Init(bounds.center, bounds.size, asyn, SeparateTreeType.QuadTree);

        entities.Clear();
        for(int i = 0; i < transform.childCount; ++i)
        {
            var entity = transform.GetChild(i).GetComponent<STSceneEntity>();
            if(entity)
            {
                AddEntity(entity);
            }
        }

        for (int i = 0; i < entities.Count; i++)
        {
            mController.AddSceneEntity(entities[i]);
        }
    }

    public void AddEntity(STSceneEntity entity)
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
