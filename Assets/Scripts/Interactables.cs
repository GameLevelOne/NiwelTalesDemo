using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InteractableType{
	Door,
	Food,
	Openable
}

public class Interactables : MonoBehaviour {
	public InteractableType type;
	
}
