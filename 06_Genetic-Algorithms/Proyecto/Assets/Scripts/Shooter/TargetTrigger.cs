using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetTrigger : MonoBehaviour
{
    [Header("References")]
    public Transform Target;

    [Header("Test exports")]
    [SerializeField] float DistanceToTarget;

    bool _toDestroy = false;
    public delegate void SetResult(float result);
    public event SetResult OnHitCollider;

    void OnCollisionEnter(Collision col)
    {
        Debug.DrawRay(transform.position, Target.transform.position - transform.position,Color.red,10f); 
        DistanceToTarget =Vector3.Distance(transform.position, Target.transform.position);
        OnHitCollider(DistanceToTarget);
        _toDestroy = true;
    }

    void Update()
    {
        DistanceToTarget = Vector3.Distance(transform.position, Target.transform.position);
        if (_toDestroy) 
            DestroyImmediate(this.gameObject);
    }
}
