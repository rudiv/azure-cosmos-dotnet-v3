// See https://aka.ms/new-console-template for more information
using AotTest;
using Azure.Identity;
using Microsoft.Azure.Cosmos;
using NanoidDotNet;

var cred = new DefaultAzureCredential();
var cosmos = new CosmosClient(Environment.GetEnvironmentVariable("COSMOS_EP"), cred, new CosmosClientOptions
{
    Serializer = CosmosSystemTextJsonSerializer.Default
});

var cont = cosmos.GetContainer(Environment.GetEnvironmentVariable("COSMOS_DB"), Environment.GetEnvironmentVariable("COSMOS_CNT"));

var newDoc = new TestDocument() { Id = Nanoid.Generate(), DocType = "test", FirstName = "Hello, I am test" };

var create = await cont.CreateItemAsync(newDoc);
Console.WriteLine("Created: " + create.Resource.Id);

var iterator = cont.GetItemQueryIterator<TestDocument>(new QueryDefinition("SELECT * FROM c WHERE c.docType = @test")
    .WithParameter("@test", "test"));
while (iterator.HasMoreResults)
{
    var result = await iterator.ReadNextAsync();
    foreach (var item in result)
    {
        Console.WriteLine(item.FirstName);
    }
}
Console.WriteLine("Read it back with params?");

newDoc.FirstName = "Hello, I am second test";
await cont.ReplaceItemAsync(newDoc, newDoc.Id);

Console.WriteLine("Replacing it with test 2");

var read = await cont.ReadItemAsync<TestDocument>(newDoc.Id, new PartitionKeyBuilder().Add(newDoc.Id).Add(newDoc.DocType).Build());

Console.WriteLine("Read: " + read.Resource.FirstName);

await cont.DeleteItemAsync<TestDocument>(newDoc.Id, new PartitionKeyBuilder().Add(newDoc.Id).Add(newDoc.DocType).Build());

Console.WriteLine("Deleted?");
iterator = cont.GetItemQueryIterator<TestDocument>("SELECT * FROM c WHERE c.docType = 'test'");
while (iterator.HasMoreResults)
{
    var result = await iterator.ReadNextAsync();
    foreach (var item in result)
    {
        Console.WriteLine(item.FirstName);
    }
}

Console.WriteLine("^ Results would be here (query without params)");