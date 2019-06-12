using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BrowserTool : MonoBehaviour 
{
	public bool fullScreenMode;
	public Camera canvasCam;
	public Camera screenShotCam;
	public Rect fullScreenRect;
	public Rect originRect;
	public GameObject ui;
	public int ratioWidth = 2550; 
	public int resWidth = 2550; 
    public int ratioHeight = 3300;
    public int resHeight = 3300;
	public int scale = 1;
	public Text resText;
	public float scrollRatio;
	public GameObject hint;
	
	public void SetScale(string val)
	{
		SetScale(int.Parse(val) );
	}
	public void SetScale(int val)
	{
		scale = val;
		resWidth = ratioWidth * scale;
		resHeight = ratioHeight * scale;
		FormatCam();
	}
	
	public void SetResolutionHeight(string val)
	{
		SetResolutionHeight(int.Parse(val));
	}
	
	public void SetResolutionHeight(int val)
	{
		ratioHeight = val;
		resHeight = ratioHeight * scale;
		FormatCam();
	}
	
	public void SetResolutionWidth(string val)
	{
		SetResolutionWidth(int.Parse(val));
	}
		
	public void SetResolutionWidth(int val)
	{
		ratioWidth = val;
		resWidth = ratioWidth * scale;
		FormatCam();
	}
	
	public void FormatCam()
	{
		float ratio = (float)resWidth / (float)resHeight;
		float screenRatio = (float)Screen.width / (float)Screen.height;
		
		
		if(ratio > screenRatio)
		{
			fullScreenRect.width = 1f;
			fullScreenRect.x = 0;
			
			fullScreenRect.height = 1f/ratio * screenRatio;
			fullScreenRect.y = (1f-fullScreenRect.height)/2f;
			
			originRect.width = fullScreenRect.width * 0.7f;
			originRect.x = (1f-originRect.width)/2f;
			
			originRect.height = fullScreenRect.height * 0.7f;
			originRect.y = (1f-originRect.height)/2f;
		}
		else
		{
			fullScreenRect.height = 1f;
			fullScreenRect.y = 0;
			
			fullScreenRect.width = ratio / screenRatio;
			fullScreenRect.x = (1f-fullScreenRect.width)/2f;
			
			originRect.width = fullScreenRect.width * 0.7f;
			originRect.x = (1f-originRect.width)/2f;
			
			originRect.height = fullScreenRect.height * 0.7f;
			originRect.y = (1f-originRect.height)/2f;
		}
				
		canvasCam.rect = originRect;
		
		resText.text = "" + resWidth + "*" + resHeight;
	}
	
	public void FormatScrollRatio()
	{
		originRect.width = fullScreenRect.width * 0.7f * scrollRatio;
		originRect.x = (1f-originRect.width)/2f;
		
		originRect.height = fullScreenRect.height * 0.7f * scrollRatio;
		originRect.y = (1f-originRect.height)/2f;
		canvasCam.rect = originRect;
	}
	
	// Use this for initialization
	void Start () {
		SetResolutionHeight(Screen.height);
		SetResolutionWidth(Screen.width);
		hint.SetActive(false);
		
		try 
        {
            System.IO.Directory.CreateDirectory(string.Format("{0}/Screenshots", Application.dataPath) );
        } 
        catch (Exception e) 
        {
            Debug.Log(e.ToString() );
        } 
		
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("FullScreen") )
		{
			if(fullScreenMode)
			{
				canvasCam.rect = originRect;
			}
			else
			{
				originRect = canvasCam.rect;
				canvasCam.rect = fullScreenRect;
			}
			fullScreenMode = !fullScreenMode;
			ui.SetActive(!fullScreenMode);
			hint.SetActive(fullScreenMode);
		}
		
		if(fullScreenMode && Input.GetButtonDown("ScreenShot") )
		{
			ScreenShot();
		}
		
		if(Input.GetButton("Ctrl") && Input.GetAxis("Mouse ScrollWheel") != 0f ) 
		{
			scrollRatio += Input.GetAxis("Mouse ScrollWheel");
			
			scrollRatio = Mathf.Max(scrollRatio, 0.1f);
			FormatScrollRatio();
		}
	}
	
	public static string ScreenShotName(int width, int height) 
	{
         return string.Format("{0}/Screenshots/screen_{1}x{2}_{3}.png", 
                              Application.dataPath, 
                              width, height, 
                              System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
    }
	
	void ScreenShot()
	{		
	
		hint.SetActive(false);
	
		screenShotCam.CopyFrom(canvasCam);
		screenShotCam.rect = new Rect(0, 0, 1, 1);
	
        RenderTexture currentRT = RenderTexture.active;
		
		RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
		screenShotCam.targetTexture = rt;
		screenShotCam.Render();
		RenderTexture.active = rt;
		
		/*float ratio = (float)resWidth / (float)resHeight;
		float screenRatio = (float)Screen.width / (float)Screen.height;		
		
		if(resWidth > resHeight)
		{
			
		}
		else
		{
			
		}*/
		
		
		Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
		screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
		screenShot.Apply();
		
		screenShotCam.targetTexture = null;		
        RenderTexture.active = currentRT;
		
		// RenderTexture.active = null; 
		// Destroy(rt);
		
		
		byte[] bytes = screenShot.EncodeToPNG();
		string filename = ScreenShotName(resWidth, resHeight);
		System.IO.File.WriteAllBytes(filename, bytes);
		Debug.Log(string.Format("Took screenshot to: {0}", filename));
		
		
		hint.SetActive(true);
	}
	
}
