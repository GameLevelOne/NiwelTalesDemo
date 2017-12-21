using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour {
	public GameObject soldierObj;
	public float bulletForce;
	public bool go = true;

	public void Init(GameObject soldierObj)
	{
		this.soldierObj = soldierObj;
		StartCoroutine(Translate());
		StartCoroutine(Destroy());
	}

	void OnCollisionEnter2D(Collision2D other){
		
//		print("GameObject: "+other.gameObject.name);
		if(other.gameObject.tag == Tags.MONSTER){
			if(other.transform.parent != null)	other.transform.parent.GetComponent<Monster>().InitChase(soldierObj);
			go = false;
		}
	}

	IEnumerator Translate()
	{
		while(go)
		{
			float direction = transform.localScale.x < 0f ? 1f : -1f;
			transform.Translate(bulletForce * direction,0,0);
			yield return null;
		}
		Destroy(gameObject);
	}

	IEnumerator Destroy()
	{
//		print("DESTROYED CALLED");
		yield return new WaitForSeconds(5f);
		Destroy(gameObject);
	}
}
