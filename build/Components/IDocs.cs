using System;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Npm;

interface IDocs : INukeBuild, IHazArtifacts
{
    AbsolutePath DocsDirectory => RootDirectory / "docs";

    Target Docs => _ => _.DependsOn(DocsCompile, DocsCopy);

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

    Target DocsCopy =>
        _ =>
            _.DependsOn(DocsCompile)
                .Executes(
                    () =>
                        FileSystemTasks.CopyDirectoryRecursively(
                            DocsDirectory / ".vitepress/dist",
                            ArtifactDirectories.Docs / "dist",
                            DirectoryExistsPolicy.Merge,
                            FileExistsPolicy.Overwrite
                        )
                )
                .Unlisted();

    Target DocsClean =>
        _ =>
            _.Executes(
                () => (DocsDirectory / ".vitepress/dist").DeleteDirectory(),
                () => (ArtifactDirectories.Docs / "dist").DeleteDirectory()
            );
}
