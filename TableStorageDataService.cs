using Azure.Data.Tables;

namespace AzureTableStorageCRUD;

public class TableStorageDataService<T> where T : class, ITableEntity, new()
{
    private TableClient TableClient { get; }

    public TableStorageDataService(string storageConnStr)
    {
        var serviceClient = new TableServiceClient(storageConnStr);
        TableClient = serviceClient.GetTableClient(typeof(T).Name);
        //await tableClient.CreateIfNotExistsAsync();
    }

    public async Task Add(List<T> entities)
    {
        await Parallel.ForEachAsync(entities.Chunk(100), async (entitiesBatch, token) =>
        {
            await BatchManipulateEntities(entitiesBatch, TableTransactionActionType.Add);
        });
    }

    public async Task DeleteAll()
    {
        // Only need to pull PartitionKey & RowKey fields are required for deletion
        var entities = TableClient.QueryAsync<TableEntity>(select: new string[] { nameof(TableEntity.PartitionKey), nameof(TableEntity.RowKey) }, maxPerPage: 1000);

        await Parallel.ForEachAsync(entities.AsPages(), async (page, token) =>
        {
            await BatchManipulateEntities(page.Values, TableTransactionActionType.Delete);
        });
    }

    private async Task BatchManipulateEntities(IEnumerable<ITableEntity> entities, TableTransactionActionType tableTransactionActionType)
    {
        // https://medium.com/medialesson/deleting-all-rows-from-azure-table-storage-as-fast-as-possible-79e03937c331

        var groups = entities.GroupBy(x => x.PartitionKey);

        foreach (var group in groups)
        {
            foreach (var itemsBatch in group.Chunk(100))
            {
                var actions = new List<TableTransactionAction>();
                actions.AddRange(itemsBatch.Select(e => new TableTransactionAction(tableTransactionActionType, e)));

                await TableClient.SubmitTransactionAsync(actions);
            }
        }
    }
}