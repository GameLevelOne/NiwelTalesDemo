using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rooms : MonoBehaviour {

	public bool isLocked = true;
	public GameObject overlay;
	public GameObject prefabMonster;
	public Transform[] DoorsAndWindows; //doorLeft,doorRight,window

	bool monsterIsSummoned = false;
	Monster monsterObj;

	void OnEnable(){
		StressMeter.OnStressMeterFull += OnStressMeterFull;
	}

	void OnDisable(){
		StressMeter.OnStressMeterFull -= OnStressMeterFull;
	}

	void OnStressMeterFull ()
	{
		if(DoorsAndWindows.Length!=0 && !monsterIsSummoned){
			//SummonMonster ();
			}
	}

	public void RevealRoom(bool isLocked){
		this.isLocked = isLocked;
		overlay.SetActive (isLocked);
	}

	void SummonMonster(){
		monsterIsSummoned = true;
		int temp = Random.Range (0, DoorsAndWindows.Length);
		Monster monsterObj = Instantiate (prefabMonster, this.transform, false).GetComponent<Monster> ();
		monsterObj.transform.localPosition = DoorsAndWindows [temp].localPosition;
		bool lookRight = false;
		if(temp == 0){
			lookRight = true;
		}
		InitMonster (monsterObj,lookRight);
	}

	void InitMonster(Monster monsterObj,bool lookRight){
		this.monsterObj = monsterObj;
		this.monsterObj.Init (lookRight);
		this.monsterObj.OnMonsterDestroyed += OnMonsterDestroyed;
	}

	void OnMonsterDestroyed ()
	{
		monsterIsSummoned = false;
		monsterObj.OnMonsterDestroyed -= OnMonsterDestroyed;
		Destroy (monsterObj.gameObject);
	}
}
