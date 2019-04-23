using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Layer : MonoBehaviour {

	public int layerIndex;
	public Image myImg;
	public Image myBg;
	public Transform puzzle;
	public Color colorSelected;
	public Color colorNormal;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
		layerIndex = transform.GetSiblingIndex();
	}
	
	public void SetShow(bool val)
	{
		puzzle.gameObject.SetActive(val);
	}
	
	public void SetPuzzle(Transform val)
	{
		puzzle = val;
	}
	
	public void SetImage(Sprite val)
	{
		myImg.sprite = val;
	}
	
	public void Resize()
	{
		RectTransform imgRect = myImg.rectTransform;
		if(myImg.sprite.rect.width/myImg.sprite.rect.height > imgRect.rect.width/imgRect.rect.height)
		{
			//Fit width
			myImg.rectTransform.sizeDelta = new Vector2(imgRect.rect.width, imgRect.rect.width/myImg.sprite.rect.width * myImg.sprite.rect.height);
		}
		else
		{
			//Fir height
			myImg.rectTransform.sizeDelta = new Vector2(imgRect.rect.height / myImg.sprite.rect.height * myImg.sprite.rect.width , imgRect.rect.height);
		}
	}
	
	public void DetectMouseOn()
	{
		LayerConsole.nowPointerIndex = layerIndex;
	}
	
	public void DetectDrag()
	{
		if(layerIndex != LayerConsole.nowPointerIndex)
		{
			transform.SetSiblingIndex(LayerConsole.nowPointerIndex);
			PuzzleConsole.instance.SendMessage("SortSpriteOrder");
		}
	}
	
	public void SetLayerUp(bool isUp)
	{
		/*Debug.Log("SetLayerUp: " + isUp);
		Debug.Log("SiblingIndex():" + transform.GetSiblingIndex());
		Debug.Log("childCount: " + transform.parent.childCount);*/
		layerIndex = transform.GetSiblingIndex();
		
		if(isUp)
		{
			if(transform.GetSiblingIndex() > 0)
				transform.SetSiblingIndex(layerIndex-1);
		}
		else
		{
			if(transform.GetSiblingIndex() < transform.parent.childCount-1)
				transform.SetSiblingIndex(layerIndex+1);
		}
		
		PuzzleConsole.instance.SendMessage("SortSpriteOrder");
		
	}
	
	public void Select()
	{
		PuzzleConsole.instance.SendMessage("SetSelect", puzzle);
	}
	
	public void IsFocused(bool val)
	{
		if(val)
			// myBg.color = Color.cyan;
			myBg.color = colorSelected;
		else
			// myBg.color = Color.white;
			myBg.color = colorNormal;
	}
	
}
