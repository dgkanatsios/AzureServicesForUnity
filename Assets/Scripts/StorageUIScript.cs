using UnityEngine;
using System.Collections;
using System;
using AzureServicesForUnity;
using AzureServicesForUnity.Shared.QueryHelpers.Other;
using System.Linq;
using UnityEngine.UI;
using AzureServicesForUnity.Shared;
using AzureServicesForUnity.Storage;

public class StorageUIScript : MonoBehaviour
{
    public Text StatusText;
    

    public void Start()
    {
        Globals.DebugFlag = true;

        if (Globals.DebugFlag)
            Debug.Log("instantiated Azure Services for Unity version " + Globals.LibraryVersion);

        //TableStorage.Instance.AuthenticationToken = "CxKQjfU47L22tk8GL7fnFVX9qGunZQkNXaW/OmrFXfIGnifoNxWxsd9HaZ6a5ZnDs4UQngqFRsGH0qEIwBve1w==";
        TableStorage.Instance.SASToken = "?sv=2015-12-11&ss=bfqt&srt=sco&sp=rwdlacup&se=2017-03-20T19%3A11%3A43Z&st=2017-02-18T11%3A11%3A43Z&spr=https&sig=dohu1Yr7xxPZxx1cte1pZyzt8LfLCwd1TtRRyoVts44%3D&api-version=2015-12-11";
}

    private void ShowError(string error)
    {
        Debug.Log(error);
        StatusText.text = "Error: " + error;
    }

    public void QueryTable()
    {
        //Age >= 30 && Age <= 33
        string query = "$filter=(Age%20ge%2030)%20and%20(Age%20le%2033)&$select=PartitionKey,RowKey,Age,City";
        TableStorage.Instance.QueryTable<Customer>(query,"people3", queryTableResponse =>
        {
            if (queryTableResponse.Status == CallBackResult.Success)
            {
                string result = "QueryTable completed";
                Debug.Log(result);
                StatusText.text = result;
            }
            else
            {
                ShowError(queryTableResponse.Exception.Message);
            }
        });
        StatusText.text = "Loading...";
    }

    public void CreateTable()
    {
        TableStorage.Instance.CreateTableIfNotExists("people3", createTableResponse =>
        {
            if (createTableResponse.Status == CallBackResult.Success)
            {
                string result = "CreateTable completed";
                Debug.Log(result);
                StatusText.text = result;
            }
            //you could also check if CallBackResult.ResourceExists
            else
            {
                ShowError(createTableResponse.Exception.Message);
            }
        });
        StatusText.text = "Loading...";
    }

    public void DeleteTable()
    {
        TableStorage.Instance.DeleteTable("people3", deleteTableResponse =>
        {
            if (deleteTableResponse.Status == CallBackResult.Success)
            {
                string result = "DeleteTable completed";
                Debug.Log(result);
                StatusText.text = result;
            }
            else
            {
                ShowError(deleteTableResponse.Exception.Message);
            }
        });
        StatusText.text = "Loading...";
    }

    public void InsertEntity()
    {
        Customer cust = new Customer();
        cust.PartitionKey = "Gkanatsios23";
        cust.RowKey = "Dimitris23";
        cust.Age = 33;
        cust.City = "Athens2";

        TableStorage.Instance.InsertEntity<Customer>(cust, "people3", insertEntityResponse =>
        {
            if (insertEntityResponse.Status == CallBackResult.Success)
            {
                string result = "InsertEntity completed";
                Debug.Log(result);
                StatusText.text = result;
            }
            else
            {
                ShowError(insertEntityResponse.Exception.Message);
            }
        });
        StatusText.text = "Loading...";
    }



    [Serializable()]
    public class Customer : TableEntity
    {
        public Customer(string partitionKey, string rowKey)
            : base(partitionKey, rowKey) { }

        public Customer() : base() { }

        public int Age;
        public string City;
    }


}