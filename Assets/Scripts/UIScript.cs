using UnityEngine;
using System.Collections;
using System;
using AzureServicesForUnity;
using AzureServicesForUnity.QueryHelpers;
using System.Linq;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{
    public Text StatusText;

    public void Start()
    {
        //get the authentication token somehow...
        //e.g. for facebook, check the Unity Facebook SDK at https://developers.facebook.com/docs/unity
        UnityServices.Instance.AuthenticationToken = "";
    }


    public void CallAPI()
    {
        //custom message is: response.send(200, "{\"message\": \"hello world\", \"data\": \"15\"}") 
        //in hello.js and method is POST
        UnityServices.Instance.CallAPI<CustomAPIReturnObject>("hello", HttpMethod.Post, response =>
         {
             if (response.Status == CallBackResult.Success)
             {
                 CustomAPIReturnObject obj = response.Result;
                 string result = string.Format("message is {0} and data is {1}", obj.message, obj.data);
                 Debug.Log(result);
                 StatusText.text = result;
             }
         });
        StatusText.text = "Loading...";
    }

    public void Insert()
    {
        Highscore score = new Highscore();
        score.playername = "Zookee";
        score.score = 54;
        UnityServices.Instance.Insert(score, insertResponse =>
        {
            if (insertResponse.Status == CallBackResult.Success)
            {
                string result = "Insert completed";
                Debug.Log(result);
                StatusText.text = result;
            }
        });
        StatusText.text = "Loading...";
    }

    public void SelectFiltered()
    {
        ODataQueryProvider odqp = new ODataQueryProvider();
        ODataQuery<Highscore> q = new ODataQuery<Highscore>(odqp);

        string pn = "Zookee";
        var query = q.Where(x => x.score > 500 || x.playername == pn).OrderBy(x => x.score);

        UnityServices.Instance.SelectFiltered<Highscore>(query, x =>
        {
            if (x.Status == CallBackResult.Success)
            {
                foreach (var item in x.Result)
                {
                    Debug.Log("new score is " + item.score);
                }
                StatusText.text = "success, found " + x.Result.Count() + " results";
            }
            else
            {
                Debug.Log(x.Exception.Message);
            }
        });
        StatusText.text = "Loading...";
    }

    public void SelectByID()
    {
        UnityServices.Instance.SelectByID<Highscore>("afdd7698-bba2-4a41-bb70-d9e202d91130", x =>
        {
            if (x.Status == CallBackResult.Success)
            {
                Highscore hs = x.Result;
                Debug.Log(hs.score);
                StatusText.text = "score of selected Highscore entry is " + hs.score;
            }
        });
        StatusText.text = "Loading...";
    }

    public void UpdateSingle()
    {
        UnityServices.Instance.SelectByID<Highscore>("afdd7698-bba2-4a41-bb70-d9e202d91130", selectResponse =>
        {
            if (selectResponse.Status == CallBackResult.Success)
            {
                Highscore hs = selectResponse.Result;
                hs.score += 1;
                UnityServices.Instance.UpdateObject(hs, updateResponse =>
                {
                    if (updateResponse.Status == CallBackResult.Success)
                    {
                        string msg = "object with id " + updateResponse.Result.id + " was updated";
                        Debug.Log(msg);
                        StatusText.text = msg;
                    }
                });
            }
        });
        StatusText.text = "Loading...";
    }

    public void DeleteByID()
    {
        UnityServices.Instance.SelectByID<Highscore>("b199d724-6f15-4e44-abbe-d1e9d1f751da", selectResponse =>
        {
            if (selectResponse.Status == CallBackResult.Success)
            {
                Highscore hs = selectResponse.Result;
                UnityServices.Instance.DeleteByID<Highscore>(hs.id, deleteResponse =>
                {
                    if (deleteResponse.Status == CallBackResult.Success)
                    {
                        string msg = "successfully deleted ID = " + hs.id;
                        Debug.Log(msg);
                        StatusText.text = msg;
                    }
                }
                );
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

