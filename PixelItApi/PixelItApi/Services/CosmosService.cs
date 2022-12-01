using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using PixelIt.Contracts;

namespace PixelItApi.Services
{
    public class CosmosService : IDatabaseService
    {
        private readonly CosmosClient client;
        private readonly Container container;

        public CosmosService(IConfiguration configuration)
        {
            client = new CosmosClient(
                connectionString: configuration.GetConnectionString("CosmosClient")
            );
            
            this.container = this.client.GetDatabase("PixelIt").GetContainer("Image");
        }

        public async Task SaveImage(Image image)
        {
            await this.container.CreateItemAsync(image);
        }

        public async Task<List<Image>> GetImages()
        {
            var queryable = this.container.GetItemLinqQueryable<Image>();
            using FeedIterator<Image> feed = queryable.ToFeedIterator();
            List<Image> results = new();

            while (feed.HasMoreResults)
            {
                var response = await feed.ReadNextAsync();
                foreach (Image item in response)
                {
                    results.Add(item);
                }
            }

            return results;
        }
    }
}
