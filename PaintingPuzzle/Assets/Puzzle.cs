using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puzzle : MonoBehaviour 
{
	public SpriteRenderer sprite;
	// public GameObject layerObject;
	public Layer layerComponent;

	/*public void SetOrderUp(bool val)
	{
		Debug.Log("SetOrderUp: " + val);
		
		if(val)
			transform.Translate(Vector3.forward * -1);
		else
			transform.Translate(Vector3.forward);
	}*/
	
	void OnDestroy()
	{
		// Destroy(layerObject);
		if(layerComponent != null)
			Destroy(layerComponent.gameObject);
	}
	
	/*public void SetLayerObject(GameObject val)
	{
		layerObject = val;
	}*/
	
	public void SetLayerComponent(Layer val)
	{
		layerComponent = val;
	}
	
	public void SetLayerUp(bool isUp)
	{
		// layerObject.SendMessage("SetLayerUp", isUp);
		layerComponent.SetLayerUp(isUp);
	}
	
	public void SetFlip()
	{
		/*Vector3 tempScale = transform.localScale;
		tempScale.x *= -1f;
		transform.localScale = tempScale;*/
		
		
		transform.Rotate(Vector3.up * 180f, Space.World);
		
		// sprite.flipX = !sprite.flipX;
	}
	
	public void ShowOutline()
	{
		sprite.material.SetFloat("_OutlineWidth", 12f);
		layerComponent.IsFocused(true);
	}
	
	public void HideOutline()
	{
		sprite.material.SetFloat("_OutlineWidth", 0f);
		layerComponent.IsFocused(false);
	}
}
