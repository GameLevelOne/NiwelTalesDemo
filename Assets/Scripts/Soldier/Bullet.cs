using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour {
	public float bulletForce;
	public bool go = true;
	public void Init()
	{
		StartCoroutine(Translate());
		StartCoroutine(Destroy());
	}

	void OnCollisionEnter2D(Collision2D other){
		go = false;
	}

	IEnumerator Translate()
	{
		while(go)
		{
			transform.Translate(bulletForce * transform.localScale.x,0,0);
			yield return null;
		}
		Destroy(gameObject);
	}

	IEnumerator Destroy()
	{
		print("DESTROYED CALLED");
		yield return new WaitForSeconds(5f);
		Destroy(gameObject);
	}
}
