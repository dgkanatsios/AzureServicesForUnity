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
    }

    public void Insert()
    {
        Highscore score = new Highscore();
        score.playername = "Zookee";
        score.score = 54;
        UnityServices.Instance.Insert(score);
    }

    public void GetAll()
    {
        ODataQueryProvider odqp = new ODataQueryProvider();
        ODataQuery<Highscore> q = new ODataQuery<Highscore>(odqp);

        string pn = "Zookee";
        var lala = q.Where(x => x.score > 500 || x.playername == pn ).OrderBy(x=>x.score);

        UnityServices.Instance.Select<Highscore>(lala, x =>
        {
            foreach (var item in x)
            {
                Debug.Log(item.score);
            }
        });
    }

    public void GetSingle()
    {
        UnityServices.Instance.SelectSingle<Highscore>("afdd7698-bba2-4a41-bb70-d9e202d91130", x =>
        {
            Debug.Log(x.score);
        });
    }

    public void UpdateSingle()
    {
        UnityServices.Instance.SelectSingle<Highscore>("afdd7698-bba2-4a41-bb70-d9e202d91130", x =>
        {
            x.score += 1;
            UnityServices.Instance.UpdateObject(x, y =>
            {
                UnityServices.Instance.SelectSingle<Highscore>("afdd7698-bba2-4a41-bb70-d9e202d91130", z =>
                {
                    Debug.Log(z.score);
                });
            });
        });
    }

    public void Delete()
    {
        UnityServices.Instance.SelectSingle<Highscore>("b199d724-6f15-4e44-abbe-d1e9d1f751da", x =>
        {
            UnityServices.Instance.Delete<Highscore>(x.id, () =>
            Debug.Log("successfully deleted ID = " + x.id));
        });
    }
}


[Serializable()]
public class Highscore : AzureObjectBase
{

    public int score;
    public string playername;
}

