using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public enum PlayerState{
        Camera,
        Player
    }
    PlayerState playerState = PlayerState.Camera;
    public LayerMask layer;
    public KeyCode keyToRotate = KeyCode.LeftControl;
    public float panSpeed = 10;
    public float rotateSpeed = 4;
    Camera cam;
    void Awake(){
        cam = Camera.main;
    }

    //Vector3 origin, Vector3 direction, float maxDistance = Mathf.Infinity, int layerMask = DefaultRaycastLayers, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal);
    void Update()
    {
        switch(playerState){
            case PlayerState.Camera:
                
                if(Input.GetMouseButtonDown(0)){
                Ray ray = cam.ViewportPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, layer))
                    Debug.Log("I'm looking at " + hit.transform.name);
                else
                    Debug.Log("I'm looking at nothing!");
                }
                if(Input.GetMouseButton(1)){
                    Vector3 mousePos = cam.ScreenToViewportPoint(Input.mousePosition);
                    float xMouse = mousePos.x < 0.5f ? -1 : 1;
                    float yMouse = mousePos.y > 0.5f ? -1 : 1;
                    if(mousePos.x < 0.4f || mousePos.x > 0.6f || mousePos.y < 0.4f || mousePos.y > 0.6f){
                        Vector3 rotateDir = new Vector3(yMouse,xMouse,0);
                        transform.eulerAngles += rotateDir * rotateSpeed * Time.deltaTime;
                    }
                }
                if(!Input.GetKey(keyToRotate)){
                    float x = Input.GetAxis("Horizontal");
                    float z = Input.GetAxis("Vertical");
                    Vector3 inputDir = new Vector3(x,0,z);
                    Vector3 movementDir = inputDir.x * transform.right + inputDir.y * transform.up + inputDir.z * transform.forward;
                    transform.position += movementDir * panSpeed * Time.deltaTime;
                }
                else{
                    float xKey = Input.GetAxis("Horizontal");
                    float yKey = Input.GetAxis("Vertical");
                    Vector3 rotateDir = new Vector3(yKey,xKey,0);
                    transform.eulerAngles += rotateDir * rotateSpeed * Time.deltaTime;
                }
                break;
            case PlayerState.Player:
                break;
        }
        
    }
}
