using System.Threading.Tasks;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CosmosController : ControllerBase
    {

        public CosmosController(IConfiguration config)
        {
            Config = config;
            Client = new CosmosClient(Config.GetValue<string>("CosmosConnection"));
        }

        [HttpGet]
        public async IAsyncEnumerable<Publication> Get(string key)
        {
            Database database = Client.GetDatabase("ToDoList");
            Container container = database.GetContainer("Items");
            var query = new QueryDefinition(
                query: "SELECT * FROM Items WHERE Items.section = @key"
                )
                .WithParameter("@key", key);
            FeedIterator<Publication> feed = container.GetItemQueryIterator<Publication>(queryDefinition: query);
            while (feed.HasMoreResults)
                foreach (var item in await feed.ReadNextAsync()) yield return item;

        }

        private CosmosClient Client { get; }
        public IConfiguration Config { get; set; }
    }
}
