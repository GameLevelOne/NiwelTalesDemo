using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NiwelFlag{
	PlatformUnder,
	GroundUnder,
	OnLadder,
	LadderUnder
}

public class NiwelFlags : MonoBehaviour {

	Dictionary<string,bool> flags;

	public void InitFlags(){
		flags = new Dictionary<string, bool> ();
		flags.Add (NiwelFlag.PlatformUnder.ToString (), false);
		flags.Add (NiwelFlag.GroundUnder.ToString (), false);
		flags.Add (NiwelFlag.OnLadder.ToString (), false);
		flags.Add (NiwelFlag.LadderUnder.ToString (), false);
	}

	public bool GetFlag(NiwelFlag flag){
		return flags [flag.ToString ()];
	}

	public void SetFlag(NiwelFlag flag,bool value){
		flags [flag.ToString ()] = value;
	}
}
