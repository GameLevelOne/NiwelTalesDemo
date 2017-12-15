using System.Collections;
using UnityEngine;

public class MonsterWallDetectionTriggerCollider : MonoBehaviour {
	public Monster monster;

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.tag == Tags.WALL){
			monster.FaceWall();
		}
	}
}
