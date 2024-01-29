using Nuke.Common;
using Nuke.Common.Tools.DotNet;

interface IRestore : IHazSlnFiles
{
    Target Restore =>
        _ => _.Executes(() => DotNetTasks.DotNetRestore(settings => settings.SetProjectFile(Sln)));
}