using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    public new Camera camera;

    private NavMeshAgent mAgent;



    void Start()
    {
        mAgent = gameObject.GetComponent<NavMeshAgent>();
        mAgent.speed = 6;

        if (camera)
        {
            SmoothFollow follow = camera.gameObject.AddComponent<SmoothFollow>();
            follow.target = transform;
            follow.height = 15;
            follow.distance = 8;
        }
    }
	
	void Update () {
	    if (Input.GetMouseButtonDown(0))
	    {
	        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
	        RaycastHit hit;
	        if (Physics.Raycast(ray, out hit))
	        {
	            mAgent.SetDestination(hit.point);
	        }
	    }
	}
}
