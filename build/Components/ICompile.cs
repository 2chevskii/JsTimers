using System.IO;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;

interface ICompile : IRestore, IHazVersion, IHazConfiguration, IHazArtifacts
{
    [Parameter, Required]
    bool ZipLibs => TryGetValue<bool?>(() => ZipLibs).GetValueOrDefault();

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

    Target ZipLibsOutput =>
        _ =>
            _.Unlisted()
                .DependsOn(CompileSrc)
                .TriggeredBy(CompileSrc)
                .OnlyWhenStatic(() => ZipLibs)
                .Executes(
                    () =>
                        (SrcProject.Directory / "bin" / Configuration / "netstandard2.0").ZipTo(
                            ArtifactDirectories.Libraries / "netstandard2.0.zip",
                            fileMode: FileMode.Create
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
