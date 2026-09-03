namespace GameLogBack.Mappers;

public static class ExtensionsMapper
{
    public static string ContentTypeToExtensionFile( string contentType)
    {
        return contentType.Split('/')[1];
    }
}