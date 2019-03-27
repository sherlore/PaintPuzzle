using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;

public class ChatConsole : MonoBehaviour {

	public DatabaseReference chatRef;
	public InputField chatContent;
	public GameObject chatPrefab;
	public Transform chatParent;
	public ScrollRect chatScrollRect;
	public bool autoScroll;
	
	
	public string chatID;
	
	// Use this for initialization
	void Start () {
		
		FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://paintingpuzzle-e2e3b.firebaseio.com/");
		
		chatRef = FirebaseDatabase.DefaultInstance.GetReference("Chat");
		// chatRef.ChildAdded += HandleChatChanged;
		chatRef.ChildChanged += HandleChatChanged;
		
	}
	
	public void SetScrollMode(bool isAuto)
	{
		autoScroll = isAuto;
	}
	
	public void SetID(string val)
	{
		if(val == "" ) val = "Anonymous";
		
		chatID = val;
	}
	
	public void Chat() 
	{
		chatContent.Select();
		chatContent.ActivateInputField();
		
		if(chatContent.text == "") return;
		
		string myChatMsg = chatID + ": \n" + chatContent.text;
		
		string ticks = "" + DateTime.Now.Ticks;
		ChatPack pack = new ChatPack(myChatMsg, ticks);
		string json = JsonUtility.ToJson(pack);
		// chatRef.Child(ticks).SetRawJsonValueAsync(json);
		chatRef.Child("Cache").SetRawJsonValueAsync(json);
		
		chatContent.text = String.Empty;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void HandleChatChanged(object sender, ChildChangedEventArgs args) 
	{
		if (args.DatabaseError != null) 
		{
			Debug.LogError(args.DatabaseError.Message);
			return;
		}
		else
		{			
			var values = args.Snapshot.Value as Dictionary<string, object>;
			// string channel = args.Snapshot.Key;						
			string contentStr = values["content"].ToString();
			
			GameObject dialog = (GameObject)Instantiate(chatPrefab, chatParent);
			dialog.SendMessage("SetContent", contentStr);	
			
			if(autoScroll)
				Invoke("AutoChatScroll", 0.2f);
		}
    }
	
	void AutoChatScroll()
	{
		chatScrollRect.verticalNormalizedPosition = 0f;	
	}
}
