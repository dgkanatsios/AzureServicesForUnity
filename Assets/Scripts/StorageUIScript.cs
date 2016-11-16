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
            Debug.Log("instantiated Azure Services for Unity version " + Constants.LibraryVersion);
    }

    private void ShowError(string error)
    {
        Debug.Log(error);
        StatusText.text = "Error: " + error;
    }

    public void CreateTable()
    {
        TableStorage.Instance.CreateTable("people2", createTableResponse =>
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
        cust.PartitionKey = "Gkanatsios";
        cust.RowKey = "Dimitris";
        cust.Age = 32;
        cust.City = "Athens";

        TableStorage.Instance.InsertEntity<Customer>(cust, "people4", insertEntityResponse =>
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