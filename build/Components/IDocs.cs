using System;
using System.Collections.Generic;
using NuGet.Versioning;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Npm;
using Nuke.Common.Utilities;
using Serilog;

interface IDocs : INukeBuild, IHazArtifacts, IHazVersion
{
    AbsolutePath DocsDirectory => RootDirectory / "docs";
    AbsolutePath DocsPackageJson => DocsDirectory / "package.json";

    [LatestGitHubRelease("2chevskii/JsTimers", TrimPrefix = true)]
    string GitHubLatestReleaseVersion => TryGetValue(() => GitHubLatestReleaseVersion);

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
            _.DependsOn(DocsRestore, PatchPackageJsonVersionProps)
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
            _.DependsOn(DocsRestore, PatchPackageJsonVersionProps)
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

    Target PatchPackageJsonVersionProps =>
        _ =>
            _.After(DocsRestore)
                .Executes(() =>
                {
                    var pkg = DocsPackageJson.ReadJson<Dictionary<string, object>>();
                    pkg["version"] = Version.SemVer;
                    Log.Debug("Setting package.json 'version' to {Version}", Version.SemVer);
                    pkg["latestReleaseVersion"] = GitHubLatestReleaseVersion;
                    Log.Debug(
                        "Setting package.json 'latestReleaseVersion' to {Version}",
                        GitHubLatestReleaseVersion
                    );
                    DocsPackageJson.WriteJson(pkg);
                });
}
