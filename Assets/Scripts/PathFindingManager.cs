using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFindingManager : MonoBehaviour
{
    public List<Transform> pointsOfIntrest; 
    void Awake()
    {
        GameObject pointsParent = GameObject.Find("PointsOfInterest");
        MeshRenderer[] points = pointsParent.GetComponentsInChildren<MeshRenderer>();
        foreach(MeshRenderer p in points){
            pointsOfIntrest.Add(p.GetComponent<Transform>());
        }
        pointsOfIntrest.Sort(CompareByDistance);
    }
    public void SetPoints(){
        pointsOfIntrest.Sort(CompareByDistance);
    }
    private int CompareByDistance(Transform x, Transform y){
        if (x == null){
            if (y == null){
                // If x is null and y is null, they're
                // equal.
                return 0;
            }
            else{
                // If x is null and y is not null, y
                // is greater.
                return -1;
            }
        }
        else{
            // If x is not null...
            //
            if (y == null){
                // ...and y is null, x is greater.
                return 1;
            }
            else{
                if((x.position - transform.position).magnitude > (y.position - transform.position).magnitude){
                    return 1;
                }
                else{
                    return -1;
                }
            }
        }
    }
}
