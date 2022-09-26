using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API_01.DataAccess;
using API_01.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Refit;

namespace API_01;

public class RandomData
{
    private readonly IConfiguration _config;

    public RandomData(IConfiguration config)
    {
        _config = config;
    }
    
    [FunctionName("RandomData")]
    public async Task Run([TimerTrigger("0 */1 * * * *")] TimerInfo myTimer, ILogger log)
    {
        log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        
        var apiService = RestService.For<IRandomData>("https://api.publicapis.org");
        var response = await apiService.GetRandomData();
        var name = response.Content?.Entries.FirstOrDefault(x => x.Api.Length > 0)?.Api;
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(response.Content);

        var connectionString = _config.GetConnectionString("azureStorage");
        const string containerName = "data-container";
        const string tableName = "logtable";
        
        BlobContainerClient container = new BlobContainerClient(connectionString, containerName);
        BlobClient blob = container.GetBlobClient(name);
        
        using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
        {
            await blob.UploadAsync(ms);
        }

        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
        CloudTableClient client = storageAccount.CreateCloudTableClient();
        CloudTable table = client.GetTableReference(tableName);
        await table.CreateIfNotExistsAsync();
        var attemptLog = response.IsSuccessStatusCode ? "Succeeded" : "Failed";
        
        var record = new DataTableModel(name, attemptLog);
        TableOperation insertOperation = TableOperation.Insert(record);
        await table.ExecuteAsync(insertOperation);
    }
}