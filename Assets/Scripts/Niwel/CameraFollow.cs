using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {
    
    public GameObject objectToFollow;
    
    public float speed = 1f;
	public float smoothTime = 0.3f;

    Vector3 velocity = Vector3.zero;

    
    void Update () {
        float interpolation = speed * Time.deltaTime;
        
        Vector3 position = this.transform.position;
        position.y = Mathf.Lerp(this.transform.position.y, objectToFollow.transform.position.y + 4.5f, interpolation);
        position.x = Mathf.Lerp(this.transform.position.x, objectToFollow.transform.position.x, interpolation);
        
//        this.transform.position = position;

		transform.position = Vector3.SmoothDamp(transform.position,position,ref velocity,smoothTime);
    }
}