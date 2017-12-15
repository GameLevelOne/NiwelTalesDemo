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
		
		print("GameObject: "+other.gameObject.name);
		if(other.gameObject.tag == Tags.MONSTER){
			other.transform.parent.GetComponent<Monster>().DetectObjects(soldierObj);
			go = false;
		}
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
