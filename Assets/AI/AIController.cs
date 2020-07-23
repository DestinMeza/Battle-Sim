using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameTools.Components;
public class AIController : MonoBehaviour
{

    public enum AIState{
        ToPoint,
        Attacking,
        Kite,
        Routing,
        Holding
    }
    public AIState movementState = AIState.ToPoint;
    public float retreatDuration = 3;
    public float kiteDuration = 2;
    CharacterMotor motor;
    ProjectileLauncher gun;
    PathFindingManager pathFinding;
    TeamController team;
    HealthController health;
    List<Transform> pointsOfIntrest;
    List<CharacterMotor> characters;
    List<Transform> enemies = new List<Transform>();
    Transform point;
    Transform closestEnemy;
    int closestPoint = 1;
    float lastRetreatTime = -1;
    float lastKiteTime = -1;
    float pointChangeCal = 1;
    void Awake()
    {
        pathFinding = GetComponent<PathFindingManager>();
        motor = GetComponent<CharacterMotor>();
        gun = GetComponentInChildren<ProjectileLauncher>();
        team = GetComponent<TeamController>();
        health = GetComponent<HealthController>();
    }

    void Start(){
        
        pathFinding.SetPoints();
        pointsOfIntrest = pathFinding.pointsOfIntrest;
        closestPoint = pointsOfIntrest.Count -1;
        point = pathFinding.pointsOfIntrest[pointsOfIntrest.Count - closestPoint];
        characters = new List<CharacterMotor>(FindObjectsOfType<CharacterMotor>());
        GetEnemies();
    }

    void GetEnemies(){
        foreach(CharacterMotor c in characters){
            if(c.GetComponent<TeamController>().layerID != team.layerID) enemies.Add(c.transform);
        }
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
                if(x.gameObject.activeSelf && y.gameObject.activeSelf){
                    if((x.position - transform.position).magnitude > (y.position - transform.position).magnitude){
                        return 1;
                    }
                    else{
                        return -1;
                    }
                }
                else if(x.gameObject.activeSelf && !y.gameObject.activeSelf){
                    return 1;
                }
                else if(!x.gameObject.activeSelf && y.gameObject.activeSelf){
                    return -1;
                }
                else{
                    return 0;
                }
            }
        }
    }

    void Update(){

        pointChangeCal = Random.Range(1,10);
        if((point.position - transform.position).magnitude < pointChangeCal){
            closestPoint--;
            if(closestPoint <= 1) closestPoint = pointsOfIntrest.Count - 1;
            point = pointsOfIntrest[pointsOfIntrest.Count - closestPoint];
        }

        if(Time.time - lastRetreatTime > retreatDuration){
            if(health.health < 2){
                movementState = AIState.Routing;
                lastRetreatTime = Time.time;
            }
            else if(closestEnemy != null){
                Vector3 enemyDiff = closestEnemy.position - transform.position;
                if(enemyDiff.magnitude < 10 && Time.time - lastKiteTime > kiteDuration){
                    movementState = AIState.Kite;
                    gun.Fire(transform.forward);
                    motor.Turn(enemyDiff);
                }
                else if(enemyDiff.magnitude < 20){
                    movementState = AIState.Attacking;
                    gun.Fire(transform.forward);
                    motor.Turn(enemyDiff);
                }
            }
            else if(movementState >= AIState.Attacking){
                movementState = AIState.ToPoint;
            }
        }
        
        switch(movementState){
            case AIState.ToPoint:
                if(point == null) break;
                Vector3 pointDir = point.position - transform.position;
                motor.Move(pointDir);
                break;
            case AIState.Attacking:
                if(closestEnemy != null){
                    lastKiteTime = Time.time;
                    Vector3 attackDir = closestEnemy.position - transform.position;
                    motor.Move(attackDir);
                }
                break;
            case AIState.Kite:
                if(closestEnemy != null){
                    Vector3 kiteDir = transform.position - closestEnemy.position;
                    motor.Move(kiteDir);
                }
                break;
            case AIState.Routing:
                Vector3 retreatDir = pointsOfIntrest[0].position - transform.position;
                motor.Move(retreatDir);
                break;
            case AIState.Holding:
                motor.Move(Vector3.zero);
                break;
        }
    }
    void LateUpdate(){
        enemies.Sort(CompareByDistance);
        if(enemies.Count > 0){
            Transform t = enemies[0];
            closestEnemy = t.gameObject.activeSelf ? t : null;
            if(closestEnemy == null) enemies.RemoveAt(0);
        }
    }
}
