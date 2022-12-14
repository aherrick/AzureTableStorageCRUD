using AzureTableStorageCRUD;
using Bogus;
using System.Diagnostics;

var customerFaker = new Faker<CustomerEntity>()
        .RuleFor(c => c.FirstName, f => f.Person.FirstName)
        .RuleFor(c => c.LastName, f => f.Person.LastName)
        .RuleFor(c => c.Email, f => f.Person.Email)
        .RuleFor(c => c.Bio, f => f.Lorem.Paragraph(1));

var customers = customerFaker.Generate(40000);

const string StorageConnStr = "";

var tableService = new TableStorageDataService<CustomerEntity>(StorageConnStr);

// delete all

var start = Stopwatch.GetTimestamp();

await tableService.DeleteAll();

Console.WriteLine("Delete All: " + Stopwatch.GetElapsedTime(start));

start = Stopwatch.GetTimestamp();

// add all

await tableService.Add(customers);

Console.WriteLine("Add All: " + Stopwatch.GetElapsedTime(start));

Console.ReadLine();