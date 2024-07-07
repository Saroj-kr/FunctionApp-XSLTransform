using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using XSLTransformationService;

namespace XSLTransform.InProcessFA;

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


    [FunctionName(nameof(Function))]
    public async Task Run(
            [BlobTrigger("%xsltransformcontainer%/%sourceFolder%/{name}.xml")] Stream strmSource,
            [Blob("%xsltransformcontainer%/%MapName%", FileAccess.Read)] Stream strmXSLT,
            [Blob("%xsltransformcontainer%/%destinationFolder%/{name}-output.xml", FileAccess.Write)] BlobClient blobDestination,
            string name
        )
    {
        _logger.LogInformation($"Processing Input File {name}.xml");

        // Load source xml and xslt
        _client.Load(strmSource, strmXSLT);

        // Transform Input stream
        Stream strmOutput = _client.Transform();

        // Write transformed body to blob
        await WriteTransformedOutput(blobDestination, strmOutput);
    }

    private async Task WriteTransformedOutput(BlobClient blob, Stream content)
    {
        await blob.DeleteIfExistsAsync();
        content.Seek(0, SeekOrigin.Begin);
        await blob.UploadAsync(content);
    }
}
