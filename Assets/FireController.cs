using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameTools.Components;
public class FireController : MonoBehaviour
{   
    ProjectileLauncher gun;
    void Awake(){
        gun = GetComponentInChildren<ProjectileLauncher>();
    }
    void Update()
    {
        gun.Fire(transform.forward);
    }
}
