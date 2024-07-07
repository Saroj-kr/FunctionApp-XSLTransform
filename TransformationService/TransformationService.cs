using System.Xml;
using System.Xml.Xsl;
using Microsoft.Extensions.Logging;

namespace XSLTransformationService;

public class TransformationService : ITransformationService
{
    private ILogger _log;
    private readonly XslCompiledTransform _xslTransform;
    XmlReader readerSource;
    XmlReader readerXSL;
    public bool _disposed;

    public TransformationService(ILogger<TransformationService> log)
    {
        _log = log;
        _xslTransform = new();
    }

    #region ITransformationClient members

    public void Load(Stream source, Stream xsl)
    {
        readerSource = XmlReader.Create(source);
        _log.LogDebug("Loaded source XML Document");

        readerXSL = XmlReader.Create(xsl);
        _xslTransform.Load(readerXSL);
        _log.LogDebug("Loaded XSLT mapping");
    }

    public Stream Transform()
    {
        MemoryStream strmOutput = new();
        _xslTransform.Transform(readerSource, null, strmOutput);
        _log.LogDebug("Transformation Completed");

        return strmOutput;
    }
    #endregion

    #region  IDisposable members
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            readerXSL?.Dispose();
            readerSource?.Dispose();
        }

        _disposed = true;
    }
    #endregion
}