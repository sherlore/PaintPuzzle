using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dialog : MonoBehaviour {
	public Text contentText;
	
	public void SetContent(string val)
	{
		contentText.text = val;
	}
}
