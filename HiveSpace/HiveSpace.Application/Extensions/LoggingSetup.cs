using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Serilog.Sinks;
using Elastic.Transport;
using Serilog;

namespace HiveSpace.Application.Extensions
{
    public static class LoggingSetup
    {
        public static void ConfigureLogging(IHostEnvironment environment, IConfiguration configuration)
        {
            string environmentName = environment.EnvironmentName.ToLower();

            if (configuration == null)
            {
                return;
            }


            string elasticUrl = configuration["Elasticsearch:Url"];
            string username = configuration["Elasticsearch:Username"];
            string password = configuration["Elasticsearch:Password"];

            if (elasticUrl == null || username == null || password == null)
            {
                return;
            }

            Log.Logger = new LoggerConfiguration()
                 .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.Elasticsearch(new[] { new Uri(elasticUrl) }, opts =>
                {
                    opts.DataStream = new DataStreamName("logs", "hivespace", environmentName);
                }, transport =>
                {
                    transport.Authentication(new BasicAuthentication(username, password));
                    transport.ServerCertificateValidationCallback((sender, certificate, chain, sslPolicyErrors) => true);
                })
                .CreateLogger();
        }
    }
}
