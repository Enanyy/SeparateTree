using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
#if UNITY_EDITOR
using System.Xml;
#endif
using System.Security;
using Mono.Xml;

public class STScene : STComponent
{
    #region
    [Serializable]
    public class STSceneAttribute:STAttribute
    {
        [HideInInspector]
        public string name;
        [HideInInspector]
        public Bounds bounds = new Bounds(Vector3.zero, new Vector3(200, 0, 200));
        [HideInInspector]
        public bool asyn;

        public List<STComponent> components = new List<STComponent>();

        
#if UNITY_EDITOR
        public override XmlElement ToXml(XmlNode parent)
        {
            Dictionary<string, string> attributes = new Dictionary<string, string>();
            attributes.Add("name", name);
            attributes.Add("bounds", bounds.ToStringEx());
            attributes.Add("asyn", asyn.ToString());

            XmlElement node = CreateXmlNode(parent, typeof(STScene).ToString(), attributes);
            for (int i = 0; i < components.Count; ++i)
            {
                XmlElement child = components[i].ToXml(node);
            }

            return node;
        }
#endif
    }
    [SerializeField]
    public STSceneAttribute attribute = new STSceneAttribute();
    protected override STAttribute Attribute
    {
        get
        {
            return attribute;
        }
    }

    public override void UpdateAttribute()
    {
       
    }

    public override void SetAttribute()
    {

    }
    public override void ParseXml(SecurityElement node)
    {
        if(node == null)
        {
            return;
        }

       
        if(IsType(node.Tag))
        {
            attribute.name = node.Attribute("name");
            attribute.asyn = node.Attribute("asyn").ToBoolEx();
            attribute.bounds = node.Attribute("bounds").ToBoundsEx();

            if (node.Children != null)
            {

                for (int i = 0; i < node.Children.Count; ++i)
                {
                    SecurityElement child = node.Children[i] as SecurityElement;

                    GameObject go = new GameObject(child.Tag);

                    go.transform.SetParent(transform);
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localRotation = Quaternion.identity;
                    go.transform.localScale = Vector3.one;

                    Type component = Type.GetType(child.Tag);
                    if(component == null)
                    {
                        DestroyImmediate(go);continue;
                    }
                    STComponent entity = go.AddComponent(component) as STComponent;

                    if(entity == null)
                    {
                        DestroyImmediate(go);continue;
                    }

                    attribute.components.Add(entity);

                    entity.ParseXml(child);

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

        List<STSceneEntity> entities = new List<STSceneEntity>();
        for (int i = 0; i < transform.childCount; ++i)
        {
            var entity = transform.GetChild(i).GetComponent<STSceneEntity>();
            if (entity)
            {
                entities.Add(entity);
            }
        }


        for (int i = 0; i < entities.Count; i++)
        {
            mController.AddSceneEntity(entities[i]);
        }
    }

    public void AddSTComponent(STComponent component)
    {
        if (attribute.components.Contains(component) == false)
        {
            attribute.components.Add(component);
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
