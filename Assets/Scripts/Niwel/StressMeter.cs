using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StressMeter : MonoBehaviour {

	public Image barFill;

	protected float maxValue = 100;
	protected float startValue = 50;
	protected float currentValue = 0;

	void Start(){
		Init ();
	}

	protected void Init(){
		currentValue = startValue;
		UpdateDisplay ();
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
}
