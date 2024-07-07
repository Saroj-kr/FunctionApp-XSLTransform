namespace XSLTransformationService;

public interface ITransformationService : IDisposable
{
    void Load(Stream source, Stream xsl);
    Stream Transform();
}
