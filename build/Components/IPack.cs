using Nuke.Common;
using Nuke.Common.Tools.DotNet;

interface IPack : ICompile, IHazArtifacts
{
    Target Pack =>
        _ =>
            _.DependsOn(CompileSrc)
                .Executes(
                    () =>
                        DotNetTasks.DotNetPack(settings =>
                            settings
                                .SetProject(SrcProject)
                                .EnableNoBuild()
                                .SetConfiguration(Configuration)
                                .SetVersion(Version.SemVer)
                                .SetOutputDirectory(ArtifactDirectories.Packages)
                        )
                );
}
