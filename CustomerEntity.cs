using Azure.Data.Tables;

namespace AzureTableStorageCRUD;

public class CustomerEntity : ITableEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Bio { get; set; }

    // table storage requirements
    public string PartitionKey { get; set; } = nameof(CustomerEntity);

    public string RowKey { get; set; } = Guid.NewGuid().ToString();
    public DateTimeOffset? Timestamp { get; set; }

    public Azure.ETag ETag { get; set; }
}