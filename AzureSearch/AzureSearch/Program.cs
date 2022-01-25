using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using AzureSearch.Methods;
using AzureSearch.Models;

var azureSearchSettings = Demo.GetAzureClientConfiguration();

Uri serviceEndpoint = new Uri($"https://{azureSearchSettings.ServiceName}.search.windows.net/");
AzureKeyCredential credential = new AzureKeyCredential(azureSearchSettings.ApiKey);
SearchIndexClient adminClient = new SearchIndexClient(serviceEndpoint, credential);

Console.WriteLine("{0}", "Deleting index...\n");
Demo.DeleteIndexIfExists(azureSearchSettings.IndexName, adminClient);

Console.WriteLine("{0}", "Creating index...\n");
Demo.CreateIndex(azureSearchSettings.IndexName, adminClient);

SearchClient srchclient = new SearchClient(serviceEndpoint, azureSearchSettings.IndexName , credential);

FieldBuilder fieldBuilder = new FieldBuilder();
var searchFields = fieldBuilder.Build(typeof(Hotel));

var definition = new SearchIndex(azureSearchSettings.IndexName, searchFields);

var suggester = new SearchSuggester("sg", new[] { "HotelName", "Category", "Address/City", "Address/StateProvince" });
definition.Suggesters.Add(suggester);

adminClient.CreateOrUpdateIndex(definition);

var adminRulesForUpdate = adminClient.GetSearchClient(azureSearchSettings.IndexName);

Demo.UploadDocuments(adminRulesForUpdate);
Console.WriteLine("Starting queries...\n");
Demo.RunQueries(srchclient);

Console.WriteLine("{0}", "Complete. Press any key to end this program...\n");
Console.ReadKey();