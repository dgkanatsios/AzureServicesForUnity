using AzureServicesForUnity.EventHub;
using AzureServicesForUnity.Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventHubUIScript : MonoBehaviour {

    public Text StatusText;

    // Use this for initialization
    void Start () {
        Globals.DebugFlag = true;

        if (Globals.DebugFlag)
            Debug.Log("instantiated Azure Services for Unity, version " + Globals.LibraryVersion);
    }
	
	
	public void PostData () {
        CustomData cd = new CustomData() { test = "test", x = 5 };
        EventHubClient.Instance.PostData(cd, postDataResponse =>
        {
            if (postDataResponse.Status == CallBackResult.Success)
            {
                string result = "PostData completed";
                if(Globals.DebugFlag) Debug.Log(result);
                StatusText.text = result;
            }
            else
            {
                ShowError(postDataResponse.Exception.Message);
            }
        });
        StatusText.text = "Loading...";
    }

    private void ShowError(string error)
    {
        Debug.Log(error);
        StatusText.text = "Error: " + error;
    }


}

[Serializable()]
public class CustomData
{
    public int x;
    public string test;
}
