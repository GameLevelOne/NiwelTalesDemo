using System.Collections;
using UnityEngine;

public class MonsterVisionTriggerCollider : MonoBehaviour {
	public Monster monster;

	void OnTriggerEnter2D(Collider2D other)
	{
		if(monster.monsterState != MonsterState.Eat){
			if(other.tag == Tags.MAINCHAR || other.tag == Tags.SOLDIER){
				monster.DetectObjects(other.transform.parent.gameObject);
			}
		}
	}
}
