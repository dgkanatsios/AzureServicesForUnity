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
        TableStorageClient.Instance.SetAccountName("gkanatsiostestsearch");

        //fill either one of the below
        //TableStorage.Instance.AuthenticationToken = "";
        TableStorageClient.Instance.SASToken = "?sv=2016-05-31&ss=bfqt&srt=sco&sp=rwdlacup&se=2017-05-02T18:37:09Z&st=2017-05-01T10:37:09Z&spr=https&sig=%2BcEz%2BqwTnMMillQxN9rdAIHinIAznuvJiSjt25oYx9A%3D";// "?sv=2015-12-11&ss=t&srt=sco&sp=rwdlacup&st=2017-01-22T11%3A27%3A00Z&se=2017-03-23T11%3A27%3A00Z&sig=t4YVEck1VU%2Fz046vscXiRuWrrkrtdG3Rn5Z8XVqvzOU%3D";
}

    private void ShowError(string error)
    {
        Debug.Log(error);
        StatusText.text = "Error: " + error;
    }

    public void QueryTable()
    {
        //Age >= 30 && Age <= 33
        TableQuery tq = new TableQuery();
        tq.filter = "Age ge 30 and Age le 33";
        tq.select = "PartitionKey,RowKey,Age,City";

        TableStorageClient.Instance.QueryTable<Customer>(tq,"people3", queryTableResponse =>
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
        Customer cust = new Customer();
        cust.PartitionKey = "Gkanatsios23";
        cust.RowKey = "Dimitris23";
        cust.Age = 333;
        cust.City = "Athens23";

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
        Customer cust = new Customer();
        cust.PartitionKey = "Gkanatsios23";
        cust.RowKey = "Dimitris23";
        cust.Age = 33;
        cust.City = "Athens25";

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