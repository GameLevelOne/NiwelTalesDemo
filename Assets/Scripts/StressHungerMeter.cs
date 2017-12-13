using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StressHungerMeter : MonoBehaviour {

	public Image barFill;
	public bool isHungerMeter;

	public delegate void NiwelDead();
	public event NiwelDead OnNiwelDead;

	float maxValue = 100;
	float startValue = 50;
	float currentValue = 0;

	void Start(){
		currentValue = startValue;
		UpdateDisplay ();
		if(isHungerMeter){
			StartCoroutine (TickHungerMeter ());
		}
	}

	public void ModMeter (float value){
		currentValue += value;
		if(currentValue == maxValue){
			currentValue = maxValue;
		} else if(currentValue <= 0){
			currentValue = 0;
		}
		UpdateDisplay ();
	}

	public float GetCurrentMeterValue(){
		return currentValue;
	}

	void UpdateDisplay(){
		barFill.fillAmount = currentValue / maxValue;
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
