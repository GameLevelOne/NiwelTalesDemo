using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fader : MonoBehaviour {
	public delegate void FadeInFinished();
	public delegate void FadeOutFinished();
	public event FadeInFinished OnFadeInFinished;
	public event FadeOutFinished OnFadeOutFinished;

	Image panelFader;

	void Awake(){
		panelFader = GetComponent<Image>();
		panelFader.enabled = true;
	}

	void Start(){
		FadeIn();
	}

	public void FadeIn(){
		panelFader.gameObject.SetActive(true);
		StartCoroutine(StartFade(true,Color.clear));
	}

	public void FadeOut(){
		panelFader.gameObject.SetActive(true);
		StartCoroutine(StartFade(false,Color.black));
	}

	public void FadeOutGameOver(){
		panelFader.gameObject.SetActive (true);
		StartCoroutine (StartFade (false, new Color (0, 0, 0, 0.5f)));
	}

	IEnumerator StartFade(bool fadeIn,Color targetColor){
		float elapsedTime = 0f;

		while(elapsedTime < 1f){
			if(fadeIn) panelFader.color = Color.Lerp(Color.black,Color.clear,elapsedTime);
			else  panelFader.color = Color.Lerp(Color.clear,targetColor,elapsedTime);
			
			elapsedTime += Time.deltaTime;

			yield return null;
		}

		if(fadeIn){
			if(OnFadeInFinished !=null){
				OnFadeInFinished();
			}
			gameObject.SetActive(false);
		} else{
			if(OnFadeOutFinished != null){
				OnFadeOutFinished();
			}
		}
	}
}