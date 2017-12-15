using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
	
	private Vector3 playerPos;
	private Vector3 targetPos;
	private float smoothTime = 0.3f;
	private Vector3 velocity = Vector3.zero;
	private string playerTag = "MainChar";

	float screenSize = 11.5f;

	public Transform startPos;
	public Transform endPos;
	public GameObject playerObj;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
	{

		playerPos = playerObj.transform.localPosition;

		float newX;
		if (playerPos.x < startPos.position.x + screenSize)
			newX = startPos.position.x + screenSize;
		else if (playerPos.x > endPos.position.x - screenSize-2f)
			newX = endPos.position.x - screenSize - 2f;
		else
			newX = playerPos.x;

		targetPos = new Vector3(newX,6.4f,-10);
		transform.position = Vector3.SmoothDamp(transform.position,targetPos,ref velocity,smoothTime);
	}
}
