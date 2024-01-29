using System;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Npm;

interface IDocs : INukeBuild
{
    AbsolutePath DocsDirectory => RootDirectory / "docs";

    Target DocsRestore =>
        _ =>
            _.Executes(
                () =>
                    NpmTasks.NpmInstall(settings =>
                        settings.SetProcessWorkingDirectory(DocsDirectory)
                    )
            );

    Target DocsDev =>
        _ =>
            _.DependsOn(DocsRestore)
                .Executes(
                    () =>
                        NpmTasks.NpmRun(settings =>
                            settings
                                .SetCommand("docs:dev")
                                .SetProcessWorkingDirectory(DocsDirectory)
                        )
                );

    Target DocsCompile =>
        _ =>
            _.DependsOn(DocsRestore)
                .Executes(
                    () =>
                        NpmTasks.NpmRun(settings =>
                            settings
                                .SetCommand("docs:build")
                                .SetProcessWorkingDirectory(DocsDirectory)
                        )
                );

    Target DocsPreview =>
        _ =>
            _.DependsOn(DocsCompile)
                .Executes(
                    () =>
                        NpmTasks.NpmRun(settings =>
                            settings
                                .SetCommand("docs:preview")
                                .SetProcessWorkingDirectory(DocsDirectory)
                        )
                );

    Target DocsGzip =>
        _ => _.DependsOn(DocsCompile).Executes(() => throw new NotImplementedException());
}
