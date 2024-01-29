using Nuke.Common;

interface IHazConfiguration : INukeBuild
{
    [Parameter, Required]
    Configuration Configuration => TryGetValue(() => Configuration) ?? Configuration.Debug;
}