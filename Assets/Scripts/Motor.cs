using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Motor : MonoBehaviour
{
    public float maxSpeedChange = 2;
    public float speed = 10;
    public float maxTurnSpeed = 15;
    public float duration = 0.5f;
    Rigidbody rb;
    Vector3 targetVel;
    void Awake(){
        rb = GetComponent<Rigidbody>();
    }
    
    public void Move(Vector3 dir){
        dir.y = 0;
        targetVel = dir;
    }
    public void Turn(Vector3 targetDir){
        transform.forward = targetDir;
    }
    void FixedUpdate(){
        targetVel = Vector3.ClampMagnitude(targetVel, maxSpeedChange) * speed;
        rb.velocity = targetVel;
    }

}
