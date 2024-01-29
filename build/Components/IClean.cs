using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;

interface IClean : IHazSlnFiles, IHazArtifacts
{
    Target CleanProjects =>
        _ =>
            _.Executes(
                () =>
                    Configuration.All.ForEach(config =>
                        DotNetTasks.DotNetClean(settings =>
                            settings.SetProject(Sln).SetConfiguration(config)
                        )
                    )
            );

    Target CleanArtifacts =>
        _ =>
            _.Executes(
                () => ArtifactDirectories.Packages.CreateOrCleanDirectory(),
                () => ArtifactDirectories.Libraries.CreateOrCleanDirectory(),
                () => ArtifactDirectories.Docs.CreateOrCleanDirectory(),
                () => ArtifactDirectories.TestResults.CreateOrCleanDirectory(),
                () => ArtifactDirectories.Coverage.CreateOrCleanDirectory()
            );
}