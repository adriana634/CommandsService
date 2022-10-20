using AutoMapper;
using Grpc.Net.Client;
using PlatformService.Protos;
using static PlatformService.Protos.PlatformService;

namespace CommandsService.SyncDataServices.Grpc
{
    public class PlatformDataClient : IPlatformDataClient
    {
        private readonly IConfiguration configuration;
        private readonly IMapper mapper;
        private readonly ILogger<PlatformDataClient> logger;

        public PlatformDataClient(IConfiguration configuration, IMapper mapper, ILogger<PlatformDataClient> logger)
        {
            this.configuration = configuration;
            this.mapper = mapper;
            this.logger = logger;
        }

        public IEnumerable<Models.Platform> GetAllPlatforms()
        {
            logger.LogInformation("Calling GRPC Service: {GrpcPlatform}", configuration["GrpcPlatform"]);
            var channel = GrpcChannel.ForAddress(configuration["GrpcPlatform"]);
            var client = new PlatformServiceClient(channel);
            var request = new GetAllPlatformsRequest();

            try
            {
                var response = client.GetAllPlatforms(request);
                return mapper.Map<IEnumerable<Models.Platform>>(response.Platform);
            }
            catch (Exception ex)
            {
                logger.LogError("Could not call GRPC Server: {ExceptionMessage}", ex.Message);
                throw;
            }
        }
    }
}
