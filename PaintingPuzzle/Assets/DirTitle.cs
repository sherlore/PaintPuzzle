using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DirTitle : MonoBehaviour {

	public Text myTitle;
	public List<GameObject> myFiles;

	public void SetTitle(string val)
	{
		myTitle.text = val;
	}
	
	public void AddFile(GameObject file)
	{
		myFiles.Add(file);
	}
	
	public void SetDirExpand(bool isExpand)
	{
		for(int i=0; i<myFiles.Count; i++)
		{
			myFiles[i].SetActive(isExpand);
		}
	}
}
