using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SLerpFollow : MonoBehaviour
{
    public GameObject Target;

    [Space(10)]

    public bool SLerpPosition;
    public bool SLerpRotation;
    public bool SLerpScale;

    [Space(10)]

    public float SpeedPosition = 1;
    public float SpeedRotation = 1;
    public float SpeedScale    = 1;
    

    void FixedUpdate()
    {
        if(SLerpPosition) transform.position   = Vector3.Slerp    (transform.position,   Target.transform.position,   SpeedPosition * Time.deltaTime);
        if(SLerpRotation) transform.rotation   = Quaternion.Slerp (transform.rotation,   Target.transform.rotation,   SpeedRotation * Time.deltaTime);
        if(SLerpScale)    transform.localScale = Vector3.Slerp    (transform.localScale, Target.transform.localScale, SpeedScale * Time.deltaTime);
    }
}
