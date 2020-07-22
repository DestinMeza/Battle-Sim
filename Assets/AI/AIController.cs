using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{

    public enum AIState{
        ToPoint,
        Attacking,
        Routing,
        Holding
    }
    public AIState movementState = AIState.ToPoint;
    CharacterMotor motor;
    PathFindingManager pathFinding;
    List<Transform> pointsOfIntrest; 
    Transform point;
    int closestPoint = 1;
    void Awake()
    {
        pathFinding = GetComponent<PathFindingManager>();
        motor = GetComponent<CharacterMotor>();
    }

    void Start(){
        pathFinding.SetPoints();
        pointsOfIntrest = pathFinding.pointsOfIntrest;
        point = pathFinding.pointsOfIntrest[pointsOfIntrest.Count - closestPoint];
    }

    void Update()
    {
        if((point.position - transform.position).magnitude < 3){
            closestPoint++;
            if(closestPoint > pointsOfIntrest.Count) closestPoint = 1;
            point = pointsOfIntrest[pointsOfIntrest.Count - closestPoint];
        }

        switch(movementState){
            case AIState.ToPoint:
                if(point == null) break;
                Vector3 diff = point.position - transform.position;
                motor.Move(diff);
                break;
            case AIState.Attacking:
                break;
            case AIState.Routing:
                break;
            case AIState.Holding:
                break;
        }
    }
}
