using System.Collections;
using UnityEngine;

public class SoldierBodyTriggerCollider : SoldierBaseTriggerCollider {
	protected override void Trigger ()
	{
		soldier.Stay(false,1.5f);
	}
}
