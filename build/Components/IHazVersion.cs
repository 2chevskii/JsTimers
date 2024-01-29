using Nuke.Common;
using Nuke.Common.Tools.GitVersion;

interface IHazVersion : INukeBuild
{
    [GitVersion, Required]
    GitVersion Version => TryGetValue(() => Version);
}