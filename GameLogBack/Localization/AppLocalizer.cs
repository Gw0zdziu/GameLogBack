using System.Reflection;
using Microsoft.Extensions.Localization;

namespace GameLogBack.Localization;

public class AppLocalizer : IAppLocalizer
{
    private readonly IStringLocalizer _localizer;

    public AppLocalizer(IStringLocalizerFactory localizerFactory)
    {
        var type = typeof(AppLocalizer);
        var assemblyName = new AssemblyName(type.GetTypeInfo().Assembly.FullName);
        _localizer = localizerFactory.Create("SharedResource", assemblyName.Name);
    }

    public string Localize(string key) => _localizer[key];
}