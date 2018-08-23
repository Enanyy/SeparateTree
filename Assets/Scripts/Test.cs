using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Bounds b = new Bounds(Vector3.one * 7, Vector3.one * 5);
        string s = b.ToString("{0},{1},{2},{3}");
        Debug.Log(s);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
