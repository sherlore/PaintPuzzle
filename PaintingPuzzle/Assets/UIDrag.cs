using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDrag : MonoBehaviour 
{
	public Vector3 offset;
	public Transform rootTransform;
	
	public void BeginDrag()
	{
		offset = rootTransform.position - Input.mousePosition;
	}
	
	public void onDrag()
	{
		rootTransform.position = Input.mousePosition + offset;
	}
}
