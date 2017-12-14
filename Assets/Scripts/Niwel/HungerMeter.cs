using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HungerMeter : StressMeter {
	public delegate void NiwelDead();
	public event NiwelDead OnNiwelDead;

	void Start(){
		base.Init ();
		StartCoroutine (TickHungerMeter ());
	}

	IEnumerator TickHungerMeter(){
		while(true){
			ModMeter (-1);
			yield return new WaitForSeconds (1);
		}

		if(currentValue <= 0){
			if(OnNiwelDead != null){
				OnNiwelDead ();
			}
		}
	}
}
