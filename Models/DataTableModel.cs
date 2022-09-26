using Microsoft.WindowsAzure.Storage.Table;

namespace API_01.Models;

public class DataTableModel : TableEntity
{
    public DataTableModel(string apiName, string log)
    {
        PartitionKey = log;
        RowKey = apiName;
    }
}