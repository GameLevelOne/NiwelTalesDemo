using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NiwelColliders : MonoBehaviour {
	[SerializeField]
	NiwelFlags niwelFlags;

	public string[] triggersToCheck;
	public NiwelFlag[] flagsToCheck;

	List<string> triggerList = new List<string>();

	void OnTriggerEnter2D(Collider2D other){
		for(int i=0;i<triggersToCheck.Length;i++){
			if(other.tag == triggersToCheck[i]){
				niwelFlags.SetFlag (flagsToCheck [i], true);
				triggerList.Add (triggersToCheck [i]);
				break;
			}
		}
	}

	void OnTriggerExit2D(Collider2D other){
		for(int i=0;i<triggersToCheck.Length;i++){
			if(other.tag == triggersToCheck[i]){
				triggerList.Remove (triggersToCheck [i]);
				if(!triggerList.Contains(triggersToCheck[i])){
					niwelFlags.SetFlag (flagsToCheck [i], false);
				}
				break;
			}
		}
	}

}
