using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleButton : MonoBehaviour {
	// public GameObject console;
	public int puzzleIndex;
	
	/*public void SetConsole(GameObject val)
	{
		
	}*/
	
	public void SetIndex(int val)
	{
		puzzleIndex = val;
	}
	
	public void CreatePuzzle()
	{
		PuzzleConsole.instance.SendMessage("Create", puzzleIndex);
	}
}
