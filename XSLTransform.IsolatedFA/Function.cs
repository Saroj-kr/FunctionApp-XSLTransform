using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using XSLTransformationService;

namespace XSLTransform.IsolatedFA;

public class Function
{
    private readonly ILogger _logger;
    private readonly IConfiguration _config;
    private readonly BlobContainerClient _blobContainerClient;
    private readonly ITransformationService _client;

    public Function(BlobServiceClient blobService, ITransformationService client, IConfiguration config, ILogger<Function> logger)
    {
        _logger = logger;
        _config = config;
        _client = client;

        _blobContainerClient = blobService.GetBlobContainerClient(_config.GetValue<string>("xsltransformcontainer"));
        _blobContainerClient.CreateIfNotExists();
    }

    [Function(nameof(Function))]
    public async Task Run(
            [BlobTrigger("%xsltransformcontainer%/%sourceFolder%/{name}.xml")] Stream strmSource,
            [BlobInput("%xsltransformcontainer%/%MapName%")] Stream strmXSLT,
            string name
        )
    {
        try
        {
            _logger.LogInformation($"Processing Input File {name}.xml");

            // Load source xml and xslt
            _client.Load(strmSource, strmXSLT);

            // Transform Input stream
            Stream strmOutput = _client.Transform();

            // Write transformed body to blob
            await WriteTransformedOutput($"{name}-output.xml", strmOutput);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw;
        }
    }

    private async Task WriteTransformedOutput(string filename, Stream content)
    {
        BlobClient blobClient = _blobContainerClient.GetBlobClient($"{_config.GetValue<string>("destinationFolder")}/{filename}");
        await blobClient.DeleteIfExistsAsync();
        content.Seek(0, SeekOrigin.Begin);
        await blobClient.UploadAsync(content);
    }
}
