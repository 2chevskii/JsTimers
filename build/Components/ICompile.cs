using System.IO;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Octokit;

interface ICompile : IRestore, IHazVersion, IHazConfiguration, IHazArtifacts
{
    [Parameter, Required]
    bool CopyLibs => TryGetValue<bool?>(() => CopyLibs).GetValueOrDefault();

    Target CompileSrc =>
        _ =>
            _.DependsOn(Restore)
                .Executes(
                    () =>
                        DotNetTasks.DotNetBuild(settings =>
                            settings.Apply(CompileSettingsBase).SetProjectFile(SrcProject)
                        )
                );

    Target CompileTests =>
        _ =>
            _.DependsOn(Restore, CompileSrc)
                .Executes(
                    () =>
                        DotNetTasks.DotNetBuild(settings =>
                            settings.Apply(CompileSettingsBase).SetProjectFile(TestProject)
                        )
                );

    Target Compile => _ => _.DependsOn(CompileSrc, CompileTests);

    Target CopyLibsOutput =>
        _ =>
            _.Unlisted()
                .DependsOn(CompileSrc)
                .TriggeredBy(CompileSrc)
                .OnlyWhenStatic(() => CopyLibs)
                .Executes(
                    () =>
                        FileSystemTasks.CopyDirectoryRecursively(
                            SrcProject.Directory / "bin" / Configuration / "netstandard2.0",
                            ArtifactDirectories.Libraries / "netstandard2.0",
                            DirectoryExistsPolicy.Merge,
                            FileExistsPolicy.Overwrite
                        )
                );

    Configure<DotNetBuildSettings> CompileSettingsBase =>
        settings =>
            settings
                .EnableNoRestore()
                .EnableNoDependencies()
                .SetConfiguration(Configuration)
                .SetVersion(Version.SemVer);
}
