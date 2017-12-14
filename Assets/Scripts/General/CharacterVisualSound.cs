using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterVisualSound : MonoBehaviour {
	float timer = 0f;
	Vector3 originalScale;

	void Start(){
		originalScale = transform.localScale;
		gameObject.SetActive (false);
	}

	public void ShowVisualSound(){
		gameObject.SetActive (true);
		transform.localScale = originalScale;
		timer = 0f;
		StartCoroutine (Animate (5f, 8f, 0.2f));
	}

	IEnumerator Animate(float startSize,float endSize,float duration){
		while(timer < duration){
			float size = Mathf.Lerp (startSize, endSize, timer);
			transform.localScale = new Vector3 (size, size, size);
			timer += Time.deltaTime/duration;
			yield return null;
		}
		gameObject.SetActive (false);
	}
}
