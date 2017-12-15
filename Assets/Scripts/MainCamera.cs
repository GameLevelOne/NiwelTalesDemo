using System.Collections;
using UnityEngine;

public class MainCamera : MonoBehaviour {
	public Transform target;
	public Vector2 offset;
	public Monster monster;
	void LateUpdate()
	{
		if(target != null) transform.position = new Vector3(target.position.x+offset.x,target.position.y+offset.y,-10f) ;
	}
	void Start()
	{
//		monster.Init(false);
	}
}
