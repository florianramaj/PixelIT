namespace PixelItApi.Services
{
    public class CosmosService : IDatabaseService
    {
        private readonly CosmosClient client;
        private readonly Container container;

        public CosmosService()
        {
            client = new CosmosClient(
                connectionString: "ToDo "
            );

            this.container = this.client.GetDatabase("theteameditordatabase").GetContainer("editors");
        }

        public async Task SaveImage(Image image) => await this.container.CreateItemAsync(editor);
    }
}
