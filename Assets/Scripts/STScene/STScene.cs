using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
#if UNITY_EDITOR
using System.Xml;
#endif
using System.Security;
using Mono.Xml;

public class STScene : MonoBehaviour,ISTAttribute
{
    #region
    [System.Serializable]
    public class STSceneAttribute:STAttribute
    {
        public string name;
        public Bounds bounds = new Bounds(Vector3.zero, new Vector3(200, 0, 200));
        public bool asyn;

        public List<STSceneEntity> entities = new List<STSceneEntity>();
#if UNITY_EDITOR
        public override XmlElement ToXml(XmlNode parent)
        {
            Dictionary<string, string> attributes = new Dictionary<string, string>();
            attributes.Add("name", name);
            attributes.Add("bounds", bounds.ToStringEx());
            attributes.Add("asyn", asyn.ToString());

            XmlElement node = CreateXmlNode(parent, typeof(STScene).ToString(), attributes);
            for (int i = 0; i < entities.Count; ++i)
            {
                XmlElement child = entities[i].ToXml(node);
            }

            return node;
        }
#endif
    }
    [SerializeField][HideInInspector]
    public STSceneAttribute attribute = new STSceneAttribute();

    public void UpdateAttribute()
    {
       
    }
#if UNITY_EDITOR
    public XmlElement ToXml(XmlNode parent)
    {
        UpdateAttribute();
        return attribute.ToXml(parent);
    }

#endif
    public void SetAttribute()
    {

    }
    public void ParseXml(SecurityElement node)
    {
        if(node == null)
        {
            return;
        }

       
        if(node.Tag == typeof(STScene).ToString())
        {
            attribute.name = node.Attribute("name");
            attribute.asyn = node.Attribute("asyn").ToBoolEx();
            attribute.bounds = node.Attribute("bounds").ToBoundsEx();

            if (node.Children != null)
            {
                string entityName = typeof(STSceneEntity).ToString();
                for (int i = 0; i < node.Children.Count; ++i)
                {
                    SecurityElement child = node.Children[i] as SecurityElement;
                    if (child.Tag == entityName)
                    {
                        GameObject go = new GameObject("entity-" + (attribute.entities.Count + 1));

                        go.transform.SetParent(transform);
                        go.transform.localPosition = Vector3.zero;
                        go.transform.localRotation = Quaternion.identity;
                        go.transform.localScale = Vector3.one;

                        STSceneEntity entity = go.AddComponent<STSceneEntity>();

                        attribute.entities.Add(entity);

                        entity.ParseXml(child);

                    }
                }
            }
        }

        SetAttribute();

       
    }

    public void LoadXml(string text)
    {
        if(string.IsNullOrEmpty(text))
        {
            return;

        }

        SecurityParser sp = new SecurityParser();

        sp.LoadXml(text);


        SecurityElement se = sp.ToXml();

        ParseXml(se);
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
            bounds.DrawEx(Color.green);
        }
    }
}
