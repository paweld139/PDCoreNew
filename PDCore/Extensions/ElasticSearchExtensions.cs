using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace PDCore.Extensions
{
    public static class ElasticsearchExtensions
    {
        private static Func<CreateIndexDescriptor, ICreateIndexRequest> Selector;

        public static void AddElasticsearch(this IServiceCollection services, IConfiguration configuration,
            Func<CreateIndexDescriptor, ICreateIndexRequest> selector, Action<ConnectionSettings> settingsModifier)
        {
            Selector = selector;

            string url = GetUrl(configuration);

            string indexName = GetIndexName(configuration);

            var settings = new ConnectionSettings(new Uri(url))
                            .DefaultIndex(indexName);

            settingsModifier(settings);

            var client = new ElasticClient(settings);

            services.AddSingleton(client);

            CreateIndex(client, indexName, selector);
        }

        private static string GetIndexName(IConfiguration configuration) => configuration["ElasticSearch:Index"];

        private static string GetUrl(IConfiguration configuration) => configuration["ElasticSearch:Url"];

        public static void CreateIndex(this IElasticClient client, IConfiguration configuration)
        {
            string indexName = GetIndexName(configuration);

            CreateIndex(client, indexName, Selector);
        }

        public static void Deleteindex(this IElasticClient client, IConfiguration configuration)
        {
            string indexName = GetIndexName(configuration);

            if (client.Indices.Exists(indexName).Exists)
                client.Indices.Delete(indexName);
        }

        private static void CreateIndex(IElasticClient client, string indexName, Func<CreateIndexDescriptor, ICreateIndexRequest> selector)
        {
            if (!client.Indices.Exists(indexName).Exists)
            {
                var result = client.Indices.Create(indexName, selector);

                if (!result.IsValid)
                {
                    ThrowException(result);
                }
            }
        }

        private static void ThrowException(CreateIndexResponse result)
        {
            string error = result.ServerError?.Error?.RootCause?.FirstOrDefault()?.Reason ??
                result.ServerError?.Error?.CausedBy?.FailedShards?.FirstOrDefault()?.Reason?.Reason ??
                result.ServerError?.Error?.Reason ?? JsonConvert.SerializeObject(result.ServerError?.Error) ??
                result.OriginalException?.ToString() ??
                result.DebugInformation ??
                (result.ServerError != null ?
                JsonConvert.SerializeObject(result.ServerError) :
                JsonConvert.SerializeObject(result));

            if (error != null)
                throw new Exception(error);
        }
    }
}
