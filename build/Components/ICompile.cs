using Nuke.Common;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;

interface ICompile : IRestore, IHazVersion, IHazConfiguration
{
    [Parameter, Required]
    bool CopyLibsOutput => TryGetValue<bool?>(() => CopyLibsOutput).GetValueOrDefault();

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

    Target CopyLibrariesOutput =>
        _ => _.DependsOn(CompileSrc).TriggeredBy(CompileSrc).OnlyWhenStatic(() => CopyLibsOutput);

    Configure<DotNetBuildSettings> CompileSettingsBase =>
        settings =>
            settings
                .EnableNoRestore()
                .EnableNoDependencies()
                .SetConfiguration(Configuration)
                .SetVersion(Version.SemVer);
}