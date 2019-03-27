using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PolygonTest : MonoBehaviour {
	// public PolygonCollider2D myCol;
	public Sprite[] sprites;
	public GameObject emptyPrefab;
	public string[] filePathes;
	public Texture2D rawTexture;
	
	private const string paintDirectoryName = "Painting";
	public float buttonWidth = 10f;
	
	public Image myImg;
	
	public GameObject buttonPrefab;
	public Transform contentTrasform;
	
	// Use this for initialization
	void Start () 
	{
		string dirPath = Application.persistentDataPath + "/" + paintDirectoryName;
		filePathes = Directory.GetFiles(dirPath);
		
		
		StartCoroutine(LoadPainting() );
	}
	

    IEnumerator LoadPainting()
    {
		for(int i=0; i<filePathes.Length; i++)
		{
			string url = "file:///" + filePathes[i];
			
			// Start a download of the given URL
			using (WWW www = new WWW(url))
			{
				// Wait for download to complete
				yield return www;

				
				// assign texture
				Texture2D myTexture = new Texture2D(4, 4, TextureFormat.DXT1, false);
 				www.LoadImageIntoTexture(myTexture);
				
				float width = myTexture.width;
				float height = myTexture.height;
				
				Sprite s = Sprite.Create (myTexture, new Rect (0, 0, width, height), new Vector2(myTexture.width/2, myTexture.height/2), 100.0f, 1, SpriteMeshType.Tight);
				
				GameObject button = (GameObject)Instantiate(buttonPrefab, contentTrasform);
				button.SendMessage("SetIndex", i);
				
				Image buttonImg = button.GetComponent<Image>();
				buttonImg.sprite = s;				
				buttonImg.rectTransform.sizeDelta = new Vector2(buttonWidth, buttonWidth/width * height);
				
			}
			yield return new WaitForSeconds(3f);
		}
    }
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public void SetTiling(int val)
	{
		GameObject temp = (GameObject)Instantiate(emptyPrefab, new Vector3(0,0, 100) ,Quaternion.identity);
		SpriteRenderer mySpriteRenderer = temp.GetComponent<SpriteRenderer>();
		mySpriteRenderer.sprite = sprites[val];
		// paintPuzzles.Add(temp.GetComponent<Transform>() );
		temp.AddComponent<PolygonCollider2D>();
	}
}
