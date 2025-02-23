using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Serilog.Sinks;
using Elastic.Transport;
using Serilog;

namespace HiveSpace.Application.Extensions
{
    public static class LoggingSetup
    {
        public static void ConfigureLogging(IHostEnvironment environment)
        {
            string environmentName = environment.EnvironmentName.ToLower();

            Log.Logger = new LoggerConfiguration()
                 .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.Elasticsearch(new[] { new Uri("https://localhost:9200") }, opts =>
                {
                    opts.DataStream = new DataStreamName("logs", "hivespace", environmentName);
                }, transport =>
                {
                    transport.Authentication(new BasicAuthentication("elastic", "Xim6c9h0Xn10dK7X1Z2x9Q9x"));
                    transport.ServerCertificateValidationCallback((sender, certificate, chain, sslPolicyErrors) => true); 
                })
                .CreateLogger();
        }
    }
}
