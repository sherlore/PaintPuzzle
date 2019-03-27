using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunctionPanel : MonoBehaviour {

	
	void OnEnable () 
	{
		PinToTop();
	}
	
	public void PinToTop()
	{
		transform.SetAsLastSibling();
	}
}
