using UnityEngine;
using System.Collections;
using System;
using AzureServicesForUnity;
using AzureServicesForUnity.AppService.QueryHelpers.Other;
using System.Linq;
using UnityEngine.UI;
using AzureServicesForUnity.AppService;
using AzureServicesForUnity.Storage;
using AzureServicesForUnity.Shared;

public class StorageUIScript : MonoBehaviour
{
    public Text StatusText;
    

    public void Start()
    {
        Globals.DebugFlag = true;

        if (Globals.DebugFlag)
            Debug.Log("instantiated Azure Services for Unity version " + Globals.LibraryVersion);

        TableStorage.Instance.AuthenticationToken ="";
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
        TableStorage.Instance.QueryTable<Customer>(query,"people2", queryTableResponse =>
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
        TableStorage.Instance.CreateTable("people3", createTableResponse =>
        {
            if (createTableResponse.Status == CallBackResult.Success)
            {
                string result = "CreateTable completed";
                Debug.Log(result);
                StatusText.text = result;
            }
            else
            {
                ShowError(createTableResponse.Exception.Message);
            }
        });
        StatusText.text = "Loading...";
    }

    public void DeleteTable()
    {
        TableStorage.Instance.DeleteTable("people2", deleteTableResponse =>
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
        cust.PartitionKey = "Gkanatsios2";
        cust.RowKey = "Dimitris2";
        cust.Age = 33;
        cust.City = "Athens2";

        TableStorage.Instance.InsertEntity<Customer>(cust, "people2", insertEntityResponse =>
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