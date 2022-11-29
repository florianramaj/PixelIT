using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using PixelIt.Contracts;

namespace PixelItApi.Services
{
    public class CosmosService : IDatabaseService
    {
        private readonly CosmosClient client;
        private readonly Container container;

        public CosmosService()
        {
            client = new CosmosClient(
                connectionString: "AccountEndpoint=https://pixelitcosmosdb.documents.azure.com:443/;AccountKey=0GdHNgIHY1Z5iJR1hFXyHaRfj0VnCgMnznNw58eyhsSGWrNcT2q47LTPACdYmABsFuBjDZs7xCOwACDbTd4rAw==;"
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
