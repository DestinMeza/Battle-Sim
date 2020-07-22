using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamController : MonoBehaviour
{
    public MeshRenderer mesh;
    public Material mat;
    public int layerID;
    void Start()
    {
        Transform[] trans = GetComponentsInChildren<Transform>();
        foreach(Transform t in trans){
            t.gameObject.layer = layerID;
        }
        mesh.material = mat;
    }
}
