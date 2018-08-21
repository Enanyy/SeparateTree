using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    public Camera camera;

    private NavMeshAgent mAgent;

    private Vector3 mDeltaPos;
    
	void Start ()
	{
	    mAgent = gameObject.GetComponent<NavMeshAgent>();

	    mDeltaPos = camera.transform.position - transform.position;
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

	    Vector3 campos = transform.position + mDeltaPos;
	    camera.transform.position = Vector3.Lerp(camera.transform.position, campos, Time.deltaTime*10);
	}
}
