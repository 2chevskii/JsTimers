using System.ComponentModel;
using Nuke.Common.Tooling;

[TypeConverter(typeof(TypeConverter<Configuration>))]
class Configuration : Enumeration
{
    public static readonly Configuration Debug = new Configuration { Value = nameof(Debug) };
    public static readonly Configuration Release = new Configuration { Value = nameof(Release) };
    public static readonly Configuration[] All = { Debug, Release };

    public static implicit operator string(Configuration configuration) => configuration.Value;
}
