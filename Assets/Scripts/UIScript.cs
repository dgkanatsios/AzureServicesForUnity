using UnityEngine;
using System.Collections;
using System;
using AzureServicesForUnity;
using AzureServicesForUnity.QueryHelpers;
using System.Linq;

public class UIScript : MonoBehaviour
{
    public void Start()
    {
        UnityServices.Instance.AuthenticationToken = "";
    }


    public void CallAPI()
    {
        //custom message is response.send(200, "{\"message\": \"hello world\", \"data\": \"15\"}") in hello.js
        UnityServices.Instance.CallAPI<CustomAPIReturnObject>("hello", response =>
         {
             if(response.Status == CallBackResult.Success)
             {
                 CustomAPIReturnObject caro = response.Result;
                 Debug.Log(string.Format("message is {0} and data is {1}", caro.message, caro.data));
             }
         });
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
                Debug.Log("Insert completed");
            }
        });
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
                    Debug.Log(item.score);
                }
            }
            else
            {
                Debug.Log(x.Exception.Message);
            }
        });
    }

    public void SelectByID()
    {
        UnityServices.Instance.SelectByID<Highscore>("afdd7698-bba2-4a41-bb70-d9e202d91130", x =>
        {
            if (x.Status == CallBackResult.Success)
            {
                Highscore hs = x.Result;
                Debug.Log(hs.score);
            }
        });
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
                        Debug.Log("object with id " + updateResponse.Result.id + " was updated");
                    }
                });
            }
        });
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
                        Debug.Log("successfully deleted ID = " + hs.id);
                    }
                }
                );
            }
        });
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

