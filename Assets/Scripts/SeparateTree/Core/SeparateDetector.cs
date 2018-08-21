using UnityEngine;
using System.Collections;

public abstract class SeparateDetector : MonoBehaviour, IDetector
{
    public Vector3 Position
    {
        get { return transform.position; }
    }
    

    public abstract bool IsDetected(Bounds bounds);
    
}
