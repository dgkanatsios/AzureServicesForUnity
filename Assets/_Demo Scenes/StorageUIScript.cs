using UnityEngine;
using System.Collections;
using System;
using AzureServicesForUnity;
using System.Linq;
using UnityEngine.UI;
using AzureServicesForUnity.Shared;

public class StorageUIScript : MonoBehaviour
{
    public Text StatusText;


    public void Start()
    {
        Globals.DebugFlag = true;

        if (Globals.DebugFlag)
            Debug.Log("instantiated Azure Services for Unity, version " + Globals.LibraryVersion);

        //your storage account name
        TableStorageClient.Instance.SetAccountName("STORAGE ACCOUNT NAME");

        //fill either one of the below

        //TableStorage.Instance.AuthenticationToken = "";
        TableStorageClient.Instance.SASToken = "?sv=2015-12-11&ss=t&srt=sco&sp=rwdlacup&st=2017-01-22T11%3A27%3A00Z&se=2017-03-23T11%3A27%3A00Z&sig=t4YVEck1VU%2Fz04_I_AM_A_SAS_SAMPLE_6vscXiVqvzOU%3D";
    }

    private void ShowError(string error)
    {
        Debug.Log(error);
        StatusText.text = "Error: " + error;
    }

    public void QueryTable()
    {

        TableQuery tq = new TableQuery()
        {
            filter = "Age ge 30 and Age le 33", //Age >= 30 && Age <= 33
            select = "PartitionKey,RowKey,Age,City"
        };
        TableStorageClient.Instance.QueryTable<Customer>(tq, "people3", queryTableResponse =>
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
        TableStorageClient.Instance.CreateTableIfNotExists("people3", createTableResponse =>
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
        TableStorageClient.Instance.DeleteTable("people3", deleteTableResponse =>
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
        Customer cust = new Customer()
        {
            PartitionKey = "Gkanatsios23",
            RowKey = "Dimitris23",
            Age = 333,
            City = "Athens23"
        };
        TableStorageClient.Instance.InsertEntity<Customer>(cust, "people3", insertEntityResponse =>
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

    public void UpdateEntity()
    {
        Customer cust = new Customer()
        {
            PartitionKey = "Gkanatsios23",
            RowKey = "Dimitris23",
            Age = 33,
            City = "Athens25"
        };
        TableStorageClient.Instance.UpdateEntity<Customer>(cust, "people3", updateEntityResponse =>
        {
            if (updateEntityResponse.Status == CallBackResult.Success)
            {
                string result = "UpdateEntity completed";
                Debug.Log(result);
                StatusText.text = result;
            }
            else
            {
                ShowError(updateEntityResponse.Exception.Message);
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