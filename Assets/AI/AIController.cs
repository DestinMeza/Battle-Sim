using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameTools.Components;
public class AIController : MonoBehaviour
{
    public LayerMask enemyLayer;
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
    public float range = 15;
    public List <Transform> contacts = new List<Transform>();
    CharacterMotor motor;
    ProjectileLauncher gun;
    PathFindingManager pathFinding;
    TeamController team;
    HealthController health;
    List<Transform> pointsOfIntrest;
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
        pointChangeCal = Random.Range(1,10);
        pathFinding.SetPoints();
        pointsOfIntrest = pathFinding.pointsOfIntrest;
        closestPoint = pointsOfIntrest.Count -1;
        point = pathFinding.pointsOfIntrest[0];
        GetEnemies();
    }

    void GetEnemies(){

    }

    void Update(){
        if((point.position - transform.position).magnitude < pointChangeCal){
            closestPoint--;
            if(closestPoint < 1) closestPoint = pointsOfIntrest.Count;
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
                    motor.Turn(enemyDiff);
                    gun.Fire(transform.forward);
                }
                else if(enemyDiff.magnitude < 20){
                    movementState = AIState.Attacking;
                    motor.Turn(enemyDiff);
                    gun.Fire(transform.forward);
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
        RaycastHit[] contacts = Physics.SphereCastAll(transform.position, range, Vector3.one, Mathf.Infinity, enemyLayer);
        foreach(RaycastHit hit in contacts){
            if(!CheckIfExist(hit.transform) && hit.transform.gameObject.activeSelf){
                this.contacts.Add(hit.transform);
            }
            else if(CheckIfExist(hit.transform) && !hit.transform.gameObject.activeSelf){
                this.contacts.Remove(hit.transform);
            }
        }
        if(contacts.Length == 0) this.contacts.Clear();
        if(this.contacts.Count > 0) closestEnemy = this.contacts[0];
        else{
            closestEnemy = null;
        }
    }

    bool CheckIfExist(Transform t){
        foreach(Transform trans in this.contacts){
            if(t == trans) return true;
        }
        return false;
    }
}
