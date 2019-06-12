using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/*
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
using Firebase;
using Firebase.Database;
using Firebase.Storage;
using Firebase.Unity.Editor;
#endif
*/

public class PuzzleConsole : MonoBehaviour {
	public static GameObject instance;
	public Camera cam;
	public bool leftDrag;
	public Transform selectedPaint;
	public Vector3 mouseStart;	
	public Vector3 puzzleStart;	
	public float scaleSpeed;	
	public float zoomSpeed;	
	
	public GameObject[] paintings;
	public List<Transform> paintPuzzles;
	
	public int dirCount;
	public GameObject dirPrefab;	
	public List<string> filePathes;
	private const string paintDirectoryName = "/Painting";
	
	public float buttonWidth = 10f;
	public GameObject buttonPrefab;	
	public Transform contentTrasform;
	
	public GameObject layerPrefab;	
	public Transform layerTrasform;
	public float layerImgHeight = 10f;
	public float layerImgWidth = 10f;
	
	public GameObject puzzlePrefab;
	public float rotateSpeed;
	
	
	public Slider Rbar;
	public Slider Gbar;
	public Slider Bbar;
	public Slider Abar;
	public InputField colorCode;
	
	/*#if UNITY_STANDALONE_WIN || UNITY_EDITOR
	public DatabaseReference imageListRef;
	public StorageReference storage_ref;
	#endif*/
	
	public Text testLog;
	
	public Sprite[] paintImages;
	IEnumerator loadProcess;
	IEnumerator loadListProcess;
	
	public float btnScaling;
	public float btnRotate;
	
	// Use this for initialization
	void Start () {
		PuzzleConsole.instance = gameObject;
		
		
		try 
        {
            System.IO.Directory.CreateDirectory(string.Format("{0}/{1}", Application.dataPath, paintDirectoryName) );
        } 
        catch (Exception e) 
        {
            Debug.Log(e.ToString() );
        } 
		
		/*#if UNITY_STANDALONE_WIN || UNITY_EDITOR
		FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://paintingpuzzle-e2e3b.firebaseio.com/");
		imageListRef = FirebaseDatabase.DefaultInstance.GetReference("Images");
		FirebaseStorage storage = Firebase.Storage.FirebaseStorage.DefaultInstance;
		storage_ref = storage.GetReferenceFromUrl("gs://paintingpuzzle-e2e3b.appspot.com/");
				
		#elif UNITY_WEBGL
		
		#endif*/
		
		// LoadFileList();
		
		// StartCoroutine(LoadPainting() );
		// StartCoroutine(LoadPaintingOnline() );
		// LoadPaintingOnline();
	}
	
	public void ReloadImages()
	{
		if(loadProcess != null)
			StopCoroutine(loadProcess);
		
		filePathes.Clear();
		DestroyButtons();
	}
	
	public void DestroyButtons()
	{
         GameObject[] imageButtons = GameObject.FindGameObjectsWithTag("ImageButton");

        foreach (GameObject imageButton in imageButtons)
        {
			Destroy(imageButton);
        }
	}
	
	public void LoadFileListWeb()
	{
		
		// testLog.text += "\nLoadFileListWeb";
		
		loadListProcess = LoadFileListFromWeb();
		StartCoroutine(loadListProcess );
	}
	
		
	IEnumerator LoadFileListFromWeb()
    {
		
		// testLog.text += "\nLoadFileListFromWeb";
		
		string webFileList = "https://sherlore.github.io/fileList.txt";
			
		// Start a download of the given URL
		using (WWW www = new WWW(webFileList))
		{
			// Wait for download to complete
			yield return www;

			string rawList = www.text;
			
			// testLog.text += "\nData: " + rawList + "\nError: " + www.error;
			
			// yield return new WaitForSeconds(5f);
			
			// filePathes = rawList.Split(',');
			
			
			LoadPaintInstance();
		}
    }	
	
	public void LoadFileList()
	{
		
		/*#if UNITY_STANDALONE_WIN || UNITY_EDITOR
		LoadFileListFirebase();
		
		#elif UNITY_WEBGL
		
		// testLog.text = "LoadFileList";
		
		LoadFileListWeb();
		
		#endif*/
	}
	
	
	public void LoadFileListLocal()
	{
		Debug.Log("LoadFileListLocal");
		
		string rootPath = Application.dataPath + paintDirectoryName;
		
		dirCount = 0;
		DirSearch(rootPath);
		LoadPaintInstance();
	}
	
	public void DirSearch(string path)
	{
		Debug.Log("Dir:" + path);
		
		try
		{				
			bool hasFile = false;
			
			foreach(string file in Directory.GetFiles(path) )
			{
				if(file.EndsWith(".png") )
				{
					if(!hasFile)
					{
						dirCount++;
						filePathes.Add(path + ".dir");
						hasFile = true;
					}
					
					Debug.Log("File: " + file);
					filePathes.Add(file);
				}
			}	
			
			foreach(string dir in Directory.GetDirectories(path) )
			{				
				DirSearch(dir);
			}
		}
		catch(System.Exception e)
		{
			Debug.Log(e.ToString() );
		}
	}
	
	/*#if UNITY_STANDALONE_WIN || UNITY_EDITOR
	public void LoadFileListFirebase()
	{
		imageListRef.GetValueAsync().ContinueWith(task => {
        if (task.IsFaulted) {
          // Handle the error...
        }
        else if (task.IsCompleted) 
		{
			DataSnapshot snapshot = task.Result;
			// Do something with snapshot...
			if (snapshot == null) 
			{
				Debug.Log("null!");
			} 
			else if (!snapshot.HasChildren) 
			{
				Debug.Log("no children!");
			}
			else 
			{
				filePathes = new string[snapshot.ChildrenCount];
				
				int index = 0;
				foreach (var child in snapshot.Children)
				{
					//Debug.Log(child.ToString());
					var values = child.Value as Dictionary<string, object>;
					string id = child.Key;
					
					// string user = values["name"].ToString();
					// string cord = values["cord"].ToString();
					
					string fileName = id + ".png";
					Debug.Log("Loading: " + fileName);				
					LoadPaintingOnline(fileName, index);
					index++;
				}
				LoadPaintInstance();
			}
        }
      });
	}
	#endif
	
	#if UNITY_STANDALONE_WIN || UNITY_EDITOR
	// IEnumerator LoadPaintingOnline()
	public void LoadPaintingOnline(string imageName, int index)
    {
		StorageReference images_ref = storage_ref.Child(imageName);
		
		images_ref.GetDownloadUrlAsync().ContinueWith((System.Threading.Tasks.Task<Uri> task) => {
		// images_ref.GetDownloadUrlAsync().ContinueWith(task => {
		  if (!task.IsFaulted && !task.IsCanceled) {
			Debug.Log("Download URL: " + task.Result);
			// ... now download the file via WWW or UnityWebRequest.
			filePathes[index] = task.Result.ToString();
			// string url = task.Result;
			
		  }
		  else
		  {
			Debug.Log(task.Exception.ToString());
		  }
		});
		
    }
	#endif	*/
	
	public void CreateListFile()
	{
		string listPath = Application.dataPath + "/fileList.txt";
		Debug.Log(listPath);
		
		using (StreamWriter sw = new StreamWriter(listPath))    
        {
			for(int i=0; i<filePathes.Count; i++)
			{
				sw.Write(filePathes[i]);
				if(i != filePathes.Count-1)
					sw.Write(",");
			}
        }
		
		
		Debug.Log("Create List Success");
	}
	
	public void LoadPaintInstance()
	{
		// testLog.text += "\nLoadPaintInstance";
		
		
		loadProcess = LoadPainting();
		StartCoroutine(loadProcess );
	}
		
	IEnumerator LoadPainting()
    {
		paintImages = new Sprite[filePathes.Count-dirCount];
		
		int imgIndex = 0;
		// int fileCount = filePathes.Count-dirCount;
		
		GameObject nowLoadingDir = null;
		
		for(int i=0; i<filePathes.Count; i++)
		{
			
			#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
				string url = "file://" + filePathes[i];
			#else 
				string url = filePathes[i];
			#endif
				
			if(url.EndsWith(".png") )
			{
				//is img
				// Start a download of the given URL
				using (WWW www = new WWW(url))
				{
					// Wait for download to complete
					yield return www;

					// testLog.text += "\nError: " + www.error;
					
					// assign texture
					Texture2D myTexture = new Texture2D(4, 4, TextureFormat.DXT1, false);
					www.LoadImageIntoTexture(myTexture);
					
					float width = myTexture.width;
					float height = myTexture.height;
					
					Sprite s = Sprite.Create (myTexture, new Rect (0, 0, width, height), Vector2.one * 0.5f, 100.0f, 1, SpriteMeshType.Tight);
					paintImages[imgIndex] = s;
					
					GameObject button = (GameObject)Instantiate(buttonPrefab, contentTrasform);
					button.SendMessage("SetIndex", imgIndex);
					
					if(nowLoadingDir)
					{
						nowLoadingDir.SendMessage("AddFile", button);
					}
					
					Image buttonImg = button.GetComponent<Image>();
					buttonImg.sprite = s;				
					buttonImg.rectTransform.sizeDelta = new Vector2(buttonWidth, buttonWidth/width * height);
					
				}
				imgIndex++;				
			}
			else if(url.EndsWith(".dir") )
			{
				//is dir
				
				//E:/Dropbox/RealUnreal/PaintingPuzzle/Assets/Painting\谿山夢旅.dir
				
				url = url.Remove(url.Length-4);
				
				int lastSlash = url.LastIndexOf("/"); 
				
				url = url.Substring(lastSlash+1);
				
				int lastInverseSlash = url.LastIndexOf("\\"); 
				
				string dirName = url.Substring(lastInverseSlash+1);
				
				GameObject dirTitle = (GameObject)Instantiate(dirPrefab, contentTrasform);
				dirTitle.SendMessage("SetTitle", dirName);
				
				nowLoadingDir = dirTitle;
			}
		}
		
		/*#if UNITY_STANDALONE_WIN || UNITY_EDITOR
			CreateListFile();
		#endif	*/
    }	
		
	/*public void SortSpriteOrder()
	{
		for(int i=0; i<paintPuzzles.Count; i++)
		{
			Vector3 tempPos = paintPuzzles[i].position;
			tempPos.z = paintPuzzles.Count-i;
			paintPuzzles[i].position = tempPos;
		}
	}*/
		
	public void SortSpriteOrder()
	{
		int puzzleCount = layerTrasform.childCount;
		
		for(int i=0; i<puzzleCount; i++)
		{
			Transform child = layerTrasform.GetChild(i);
			Transform puzzle = child.GetComponent<Layer>().puzzle;
			Vector3 tempPos = puzzle.position;
			tempPos.z = i;
			puzzle.position = tempPos;
		}
		
	}
	
	
	public void SwapListItem(int a, int b)
	{
		if(a < 0 || b < 0 || a >= paintPuzzles.Count || b >= paintPuzzles.Count) return;
		
		Transform temp = paintPuzzles[a];
		paintPuzzles[a] = paintPuzzles[b];
		Vector3 tempPosA = paintPuzzles[a].position;
		tempPosA.z = paintPuzzles.Count-a;
		paintPuzzles[a].position = tempPosA;
		
		paintPuzzles[b] = temp;
		Vector3 tempPosB = paintPuzzles[b].position;
		tempPosB.z = paintPuzzles.Count-b;
		paintPuzzles[b].position = tempPosB;
	}
	
	public void SetColorCode(string val)
	{
		if(!val.StartsWith("#") || val.Length != 7) return;
		
		for (int i = 1; i < 7; i ++)
            if(!Uri.IsHexDigit(val[i]))
			{
				Debug.Log("Invalid code");
				return;
			}
		
				// Debug.Log(val.Substring(1, 2));
				// Debug.Log(Int32.Parse(val.Substring(1, 2), System.Globalization.NumberStyles.HexNumber ) );
				
		Rbar.value = Int32.Parse(val.Substring(1, 2), System.Globalization.NumberStyles.HexNumber );
		Gbar.value = Int32.Parse(val.Substring(3, 2), System.Globalization.NumberStyles.HexNumber );
		Bbar.value = Int32.Parse(val.Substring(5, 2), System.Globalization.NumberStyles.HexNumber );
		
		// Debug.Log(String.Format("#{0:X2}{1:X2}{2:X2}", (int)(Rbar.value), (int)(Gbar.value), (int)(Bbar.value) ));
		Debug.Log(String.Format("#{0:X2}{1:X2}{2:X2}", (int)(Rbar.value), (int)(Gbar.value), (int)(Bbar.value) ));
	}
	
	public void DisableColor()
	{		
		colorCode.interactable = false;
		Rbar.interactable = false;
		Gbar.interactable = false;
		Bbar.interactable = false;
		Abar.interactable = false;
	}
	
	public void GetColor()
	{
		
		SpriteRenderer paintRenderer = selectedPaint.GetComponent<SpriteRenderer>();
		Rbar.value = paintRenderer.color.r*256f;
		Gbar.value = paintRenderer.color.g*256f;
		Bbar.value = paintRenderer.color.b*256f;
		Abar.value = paintRenderer.color.a;
		// colorCode.text = String.Format("#{0:X2}{1:X2}{2:X2}", (int)(Rbar.value*255), (int)(Gbar.value*255), (int)(Bbar.value*255) );
		colorCode.text = String.Format("#{0:X2}{1:X2}{2:X2}", (int)(Rbar.value), (int)(Gbar.value), (int)(Bbar.value) );
		
		colorCode.interactable = true;
		Rbar.interactable = true;
		Gbar.interactable = true;
		Bbar.interactable = true;
		Abar.interactable = true;
	}
	
	public void SetColorR(float val)
	{
		
		
		// colorCode.text = String.Format("#{0:X2}{1:X2}{2:X2}", (int)(Rbar.value*255), (int)(Gbar.value*255), (int)(Bbar.value*255) );
		colorCode.text = String.Format("#{0:X2}{1:X2}{2:X2}", (int)(Rbar.value), (int)(Gbar.value), (int)(Bbar.value) );
		
		SpriteRenderer paintRenderer = selectedPaint.GetComponent<SpriteRenderer>();
		Color tempColor = paintRenderer.color;
		tempColor.r = val/256f;
		paintRenderer.color = tempColor;
		
		
		Debug.Log("SetColorR");
	}
	
	public void SetColorG(float val)
	{
		// colorCode.text = String.Format("#{0:X2}{1:X2}{2:X2}", (int)(Rbar.value*255), (int)(Gbar.value*255), (int)(Bbar.value*255) );
		colorCode.text = String.Format("#{0:X2}{1:X2}{2:X2}", (int)(Rbar.value), (int)(Gbar.value), (int)(Bbar.value) );
		
		SpriteRenderer paintRenderer = selectedPaint.GetComponent<SpriteRenderer>();
		Color tempColor = paintRenderer.color;
		tempColor.g = val/256f;
		paintRenderer.color = tempColor;
	}
	
	public void SetColorB(float val)
	{
		// colorCode.text = String.Format("#{0:X2}{1:X2}{2:X2}", (int)(Rbar.value*255), (int)(Gbar.value*255), (int)(Bbar.value*255) );
		colorCode.text = String.Format("#{0:X2}{1:X2}{2:X2}", (int)(Rbar.value), (int)(Gbar.value), (int)(Bbar.value) );
		
		SpriteRenderer paintRenderer = selectedPaint.GetComponent<SpriteRenderer>();
		Color tempColor = paintRenderer.color;
		tempColor.b = val/256f;
		paintRenderer.color = tempColor;
	}
	
	public void SetColorA(float val)
	{
		SpriteRenderer paintRenderer = selectedPaint.GetComponent<SpriteRenderer>();
		Color tempColor = paintRenderer.color;
		tempColor.a = val;
		paintRenderer.color = tempColor;
	}
	
	public void SetSelect(Transform val)
	{
		SetFocus(false);
		selectedPaint = val;
		SetFocus(true);
	}
	
	public void SetBtnScaling(float val)
	{
		btnScaling = val;
	}
	
	public void SetBtnRotate(float val)
	{
		btnRotate = val;
	}
	
	// Update is called once per frame
	void Update () {
		
		//Key input, no need to be block by mouse over UI
		if(selectedPaint != null)
		{
			#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
			Vector3 temp = Vector3.one * (Input.GetAxis("Vertical") + btnScaling) * Time.deltaTime * scaleSpeed;
			if(selectedPaint.localScale.y + temp.y > 0.1f)
			{ 
				selectedPaint.localScale += temp;
			}
			
			selectedPaint.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime * (Input.GetAxis("Horizontal") + btnRotate), Space.World);
			
			if(Input.GetButtonDown("Layer") )
			{
				float layerVal = Input.GetAxis("Layer");
				if(layerVal > 0 )
				{
					SetOrderUp(true);
				}
				else if(layerVal < 0 )
				{
					SetOrderUp(false);
				}
			}
			
			if(Input.GetButtonDown("Delete") )
			{
				Delete();
			}
			
			if(Input.GetButtonDown("Flip") )
			{
				SetFlip();
			}
			
			// Debug.Log("Rotate: " + rotateSpeed);
			// selectedPaint.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);
			// selectedPaint.localScale += Vector3.one * scaleSpeed * Time.deltaTime;
			
			#endif
			
			/*#if UNITY_ANDROID || UNITY_IOS || UNITY_WEBGL
			if (Input.touchCount == 2)
			{
				// Store both touches.
				Touch touchZero = Input.GetTouch(0);
				Touch touchOne = Input.GetTouch(1);

				// Find the position in the previous frame of each touch.
				Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
				Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

				// Find the magnitude of the vector (the distance) between the touches in each frame.
				float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
				float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

				// Find the difference in the distances between each frame.
				// float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
				float deltaMagnitudeDiff = touchDeltaMag - prevTouchDeltaMag;
				
				if(deltaMagnitudeDiff > 0 || selectedPaint.localScale.x > 0.1f)
					selectedPaint.localScale += Vector3.one * deltaMagnitudeDiff * zoomSpeed;
				
				Vector2 prevDir = touchZeroPrevPos - touchOnePrevPos;
				Vector2 nowDir = touchZero.position - touchOne.position;
				float deltaAngle = Vector2.SignedAngle(prevDir, nowDir);
				
				if(deltaMagnitudeDiff > 0)
					selectedPaint.Rotate(Vector3.forward * deltaAngle);
				
			}
			#endif*/
		}
		
		
		
		#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
				
		if(EventSystem.current.IsPointerOverGameObject()) return;
		
		if(Input.GetButtonDown("Fire1"))
		{
			Ray ray = cam.ScreenPointToRay(Input.mousePosition);
						
			// RaycastHit hit;
			RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
			if (hit)
			{
				/*if(hit.transform.tag == "Finish")
				{
					SetFocus(false);
					selectedPaint = null;
					
					Rbar.interactable = false;
					Gbar.interactable = false;
					Bbar.interactable = false;
					Abar.interactable = false;
				}*/
				// else
				// {
					Debug.Log(hit.transform.name);
					leftDrag = true;
					
					
					SetFocus(false);
					selectedPaint = hit.transform;
					SetFocus(true);
					
					puzzleStart = selectedPaint.position;
					mouseStart = cam.ScreenToWorldPoint(Input.mousePosition);
					
				// }
			}
			else
			{
				SetFocus(false);
				selectedPaint = null;
				
				Rbar.interactable = false;
				Gbar.interactable = false;
				Bbar.interactable = false;
				Abar.interactable = false;
			}
		}
		
		if(leftDrag)
		{
			
			selectedPaint.position = puzzleStart + (cam.ScreenToWorldPoint(Input.mousePosition) - mouseStart);
			
			if(Input.GetButtonUp("Fire1"))
			{
				leftDrag = false;
			}
		}
		#endif
		
		
		/*#if UNITY_ANDROID || UNITY_IOS || UNITY_WEBGL
		if(Input.touchCount == 1)
		{
			Touch touchZero = Input.GetTouch(0);
			if(EventSystem.current.IsPointerOverGameObject(touchZero.fingerId)) return;
			
			switch (touchZero.phase)
            {
                case TouchPhase.Began:
					Ray ray = cam.ScreenPointToRay(touchZero.position);
					
					// RaycastHit hit;
					RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
					if (hit)
					{
						if(hit.transform.tag == "Finish")
						{
							selectedPaint = null;
						}
						else
						{
							Debug.Log(hit.transform.name);
							leftDrag = true;
							selectedPaint = hit.transform;
							puzzleStart = selectedPaint.position;
							mouseStart = cam.ScreenToWorldPoint(touchZero.position);
						}
					}
                    break;

                case TouchPhase.Moved:
					if(leftDrag)
						selectedPaint.position = puzzleStart + (cam.ScreenToWorldPoint(touchZero.position) - mouseStart);
                    break;

                case TouchPhase.Ended:
					if(leftDrag)
						leftDrag = false;
                    break;
            }
			
		}
		#endif*/
		
		
		
		
		
	}
	
	public void SetScale(float val)
	{
		scaleSpeed = val;
	}
	
	public void SetRotate(float val)
	{
		rotateSpeed = val;
	}
	
	public void Create(int val)
	{
		// StartCoroutine(CreatePuzzle(val) );
		CreatePuzzle(val);
	}
	
	public void CreatePuzzle(int val)
    {
		/*string url = "file:///" + filePathes[val];
		
		using (WWW www = new WWW(url))
		{
			// Wait for download to complete
			yield return www;
			
			// assign texture
			Texture2D myTexture = new Texture2D(4, 4, TextureFormat.DXT1, false);
			www.LoadImageIntoTexture(myTexture);
			
			float width = myTexture.width;
			float height = myTexture.height;
			
			Sprite s = Sprite.Create (myTexture, new Rect (0, 0, width, height), Vector2.one * 0.5f, 100.0f, 1, SpriteMeshType.Tight);
			
			GameObject puzzle = (GameObject)Instantiate(puzzlePrefab, new Vector3(-1, 1, 100) ,Quaternion.identity);
			SpriteRenderer puzzleRenderer = puzzle.GetComponent<SpriteRenderer>();
			puzzleRenderer.sprite = s;
			puzzle.AddComponent<PolygonCollider2D>();
					
			paintPuzzles.Add(puzzle.transform );
			SortSpriteOrder();
			
			selectedPaint = puzzle.transform;
			
		}*/
		GameObject puzzle = (GameObject)Instantiate(puzzlePrefab, new Vector3(-1, 1, 100) ,Quaternion.identity);
		SpriteRenderer puzzleRenderer = puzzle.GetComponent<SpriteRenderer>();
		puzzleRenderer.sprite = paintImages[val];
		puzzle.AddComponent<PolygonCollider2D>();
				
		paintPuzzles.Add(puzzle.transform );
		
		
		
		GameObject layerObject = (GameObject)Instantiate(layerPrefab, layerTrasform);
		// Image layerImg = layerObject.GetComponent<Image>();
		// layerImg.sprite = paintImages[val];
		layerObject.transform.SetAsFirstSibling();
		
		Layer layerComponent = layerObject.GetComponent<Layer>();
		// layerObject.SendMessage("SetImage", paintImages[val]);
		layerComponent.SetImage(paintImages[val]);
		// layerObject.SendMessage("Resize");
		layerComponent.Resize();
		// layerObject.SendMessage("SetPuzzle", puzzle.transform);
		layerComponent.SetPuzzle(puzzle.transform);
		
		
		// puzzle.SendMessage("SetLayerObject", layerObject);
		puzzle.SendMessage("SetLayerComponent", layerComponent);
		
		SortSpriteOrder();
		
		
		SetFocus(false);
		selectedPaint = puzzle.transform;
		SetFocus(true);
		
		/*if(layerImg.sprite.rect.width/layerImg.sprite.rect.height > layerImgWidth/layerImgHeight)
		{
			//Fit width
			layerImg.rectTransform.sizeDelta = new Vector2(layerImgWidth, layerImgWidth/layerImg.sprite.rect.width * layerImg.sprite.rect.height);
		}
		else
		{
			//Fir height
			layerImg.rectTransform.sizeDelta = new Vector2(layerImgHeight / layerImg.sprite.rect.height * layerImg.sprite.rect.width , layerImgHeight);
		}*/
    }
	
	public void SetFocus(bool isFocus)
	{
		if(selectedPaint == null) return;
		
		if(isFocus)
		{
			// selectedPaint.SendMessage("ShowOutline");
			Puzzle selectPuzzle = selectedPaint.GetComponent<Puzzle>();
			selectPuzzle.ShowOutline();
			
			GetColor();
		}
		else
		{
			// selectedPaint.SendMessage("HideOutline");
			Puzzle selectPuzzle = selectedPaint.GetComponent<Puzzle>();
			selectPuzzle.HideOutline();
			
			DisableColor();
		}
	}
	
	public void Delete()
	{
		if(selectedPaint != null)
		{
			// paintPuzzles.Remove(selectedPaint.GetComponent<Transform>() );
			Destroy(selectedPaint.gameObject);
			SortSpriteOrder();
			selectedPaint = null;
		}
	}
	
	public void SetOrderUp(bool val)
	{
		if(selectedPaint != null)
		{
			/*int index = paintPuzzles.IndexOf(selectedPaint.GetComponent<Transform>() );
			if(val)
				SwapListItem(index, index+1);
			else
				SwapListItem(index, index-1);*/
			
			// selectedPaint.SendMessage("SetLayerUp", val);
			Puzzle selectPuzzle = selectedPaint.GetComponent<Puzzle>();
			selectPuzzle.SetLayerUp(val);
		}
	}
	
	
	
	public void SetFlip()
	{
		if(selectedPaint != null)
		{
			// selectedPaint.SendMessage("SetFlip");
			Puzzle selectPuzzle = selectedPaint.GetComponent<Puzzle>();
			selectPuzzle.SetFlip();
		}
	}
}
