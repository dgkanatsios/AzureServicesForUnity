using UnityEngine;
using System.Collections;
using System;
using AzureServicesForUnity;
using AzureServicesForUnity.QueryHelpers.Other;
using System.Linq;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{
    public Text StatusText;

    public void Start()
    {
        //get the authentication token somehow...
        //e.g. for facebook, check the Unity Facebook SDK at https://developers.facebook.com/docs/unity
        AzureUnityServices.Instance.AuthenticationToken = "";
    }


    public void CallAPI()
    {
        //custom message is: response.send(200, "{\"message\": \"hello world\", \"data\": \"15\"}") 
        //in hello.js and method is POST
        AzureUnityServices.Instance.CallAPI<CustomAPIReturnObject>("Hello", HttpMethod.Post, response =>
         {
             if (response.Status == CallBackResult.Success)
             {
                 CustomAPIReturnObject obj = response.Result;
                 string result = string.Format("message is {0} and data is {1}", obj.message, obj.data);
                 Debug.Log(result);
                 StatusText.text = result;
             }
             else
             {
                 ShowError(response.Exception.Message);
             }
         });
        StatusText.text = "Loading...";
    }

    public void Insert()
    {
        Highscore score = new Highscore();
        score.playername = "dimitris";
        score.score = UnityEngine.Random.Range(10,100);
        AzureUnityServices.Instance.Insert(score, insertResponse =>
        {
            if (insertResponse.Status == CallBackResult.Success)
            {
                string result = "Insert completed";
                Debug.Log(result);
                StatusText.text = result;
            }
            else
            {
                ShowError(insertResponse.Exception.Message);
            }
        });
        StatusText.text = "Loading...";
    }

    public void SelectFiltered()
    {
        EasyTableQueryHelper<Highscore> queryHelper = new EasyTableQueryHelper<Highscore>();
        

        string pn = "d";
        var query = queryHelper.Where(x => x.score > 500 || x.playername.StartsWith(pn)).OrderBy(x => x.score);

        AzureUnityServices.Instance.SelectFiltered<Highscore>(query, x =>
        {
            if (x.Status == CallBackResult.Success)
            {
                foreach (var item in x.Result)
                {
                    Debug.Log(string.Format("ID is {0},score is {1}", item.id, item.score ));
                }
                StatusText.text = "success, found " + x.Result.Count() + " results";
            }
            else
            {
                ShowError(x.Exception.Message);
            }
        });
        StatusText.text = "Loading...";
    }

    private void ShowError(string error)
    {
        Debug.Log(error);
        StatusText.text = "Error: " + error;
    }

    public void SelectByID()
    {
        AzureUnityServices.Instance.SelectByID<Highscore>("ecca86cb-8e35-47ac-8eef-74dc2ef87faa", x =>
        {
            if (x.Status == CallBackResult.Success)
            {
                Highscore hs = x.Result;
                Debug.Log(hs.score);
                StatusText.text = "score of selected Highscore entry is " + hs.score;
            }
            else
            {
                ShowError(x.Exception.Message);
            }
        });
        StatusText.text = "Loading...";
    }

    public void UpdateSingle()
    {
        AzureUnityServices.Instance.SelectByID<Highscore>("bbd01bc4-52db-407d-83a4-d8b5422e300f", selectResponse =>
        {
            if (selectResponse.Status == CallBackResult.Success)
            {
                Highscore hs = selectResponse.Result;
                hs.score += 1;
                AzureUnityServices.Instance.UpdateObject(hs, updateResponse =>
                {
                    if (updateResponse.Status == CallBackResult.Success)
                    {
                        string msg = "object with id " + updateResponse.Result.id + " was updated";
                        Debug.Log(msg);
                        StatusText.text = msg;
                    }
                    else
                    {
                        ShowError(updateResponse.Exception.Message);
                    }
                });
            }
            else
            {
                ShowError(selectResponse.Exception.Message);
            }
        });
        StatusText.text = "Loading...";
    }

    public void DeleteByID()
    {
        AzureUnityServices.Instance.SelectByID<Highscore>("bbd01bc4-52db-407d-83a4-d8b5422e300f", selectResponse =>
        {
            if (selectResponse.Status == CallBackResult.Success)
            {
                Highscore hs = selectResponse.Result;
                AzureUnityServices.Instance.DeleteByID<Highscore>(hs.id, deleteResponse =>
                {
                    if (deleteResponse.Status == CallBackResult.Success)
                    {
                        string msg = "successfully deleted ID = " + hs.id;
                        Debug.Log(msg);
                        StatusText.text = msg;
                    }
                    else
                    {
                        ShowError(deleteResponse.Exception.Message);
                    }
                });
            }
            else
            {
                ShowError(selectResponse.Exception.Message);
            }
        });
        StatusText.text = "Loading...";
    }
}


[Serializable()]
public class Highscore : AzureObjectBase
{
    public int score;
    public string playername;
}

[Serializable()]
public class CustomAPIReturnObject
{
    public string message;
    public int data;
}

