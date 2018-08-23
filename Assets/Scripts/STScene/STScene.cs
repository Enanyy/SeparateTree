using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Xml;

public class STScene : MonoBehaviour,ISTAttribute
{
    #region
    public class STSceneAttribute:STAttribute
    {
        public string name;
        public Bounds bounds = new Bounds(Vector3.zero, new Vector3(200, 0, 200));
        public bool asyn;

        public List<STSceneEntity> entities = new List<STSceneEntity>();

        public override XmlElement ToXml(XmlNode parent)
        {
            Dictionary<string, string> attributes = new Dictionary<string, string>();
            attributes.Add("name", name);
            attributes.Add("bounds", Helper.BoundsToString(bounds));
            attributes.Add("asyn", asyn.ToString());

            XmlElement node = CreateXmlNode(parent, typeof(STScene).ToString(), attributes);
            for (int i = 0; i < entities.Count; ++i)
            {
                XmlElement child = entities[i].ToXml(node);
            }

            return node;
        }
    }

    public STSceneAttribute attribute = new STSceneAttribute();
    public void UpdateAttribute()
    {
       
    }

    public XmlElement ToXml(XmlNode parent)
    {
        UpdateAttribute();
        return attribute.ToXml(parent);
    }
#endregion

    public Bounds bounds
    {
        get
        {
            attribute.bounds.center = transform.position;
            return attribute.bounds;
        }
    }


    public SeparateDetector detector;

    private SeparateEntityController mController;

    public void Size(Vector3 size)
    {
        attribute.bounds.size = size;
    }

    void Start()
    {
        mController = gameObject.GetComponent<SeparateEntityController>();
        if (mController == null)
            mController = gameObject.AddComponent<SeparateEntityController>();
        mController.Init(bounds.center, bounds.size, attribute.asyn, SeparateTreeType.QuadTree);

        attribute.entities.Clear();
        for(int i = 0; i < transform.childCount; ++i)
        {
            var entity = transform.GetChild(i).GetComponent<STSceneEntity>();
            if(entity)
            {
                AddEntity(entity);
            }
        }

        for (int i = 0; i < attribute.entities.Count; i++)
        {
            mController.AddSceneEntity(attribute.entities[i]);
        }
    }

    public void AddEntity(STSceneEntity entity)
    {
        if (attribute.entities.Contains(entity) == false)
        {
            attribute.entities.Add(entity);
        }
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
