using System.Linq;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Coverlet;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.ReportGenerator;

interface ITest : ICompile
{
    [Parameter]
    bool HtmlTestResults => TryGetValue<bool?>(() => HtmlTestResults).GetValueOrDefault();

    Target Test =>
        _ =>
            _.DependsOn(CompileTests)
                .Executes(
                    () =>
                        DotNetTasks.DotNetTest(settings =>
                            settings
                                .Apply(TestSettingsBase)
                                .Apply(TestSettingsLoggers)
                                .SetProjectFile(TestProject)
                        )
                );

    Target Coverage => _ => _.DependsOn(CoverageCollect, CoverageCreateReport);

    Target CoverageCollect =>
        _ =>
            _.DependsOn(CompileTests)
                .Executes(
                    () =>
                        CoverletTasks.Coverlet(settings =>
                            settings
                                .SetFormat("cobertura")
                                .SetAssembly(
                                    TestProject.Directory
                                        / $"bin/{Configuration}/{TestProject.GetTargetFrameworks()!.First()}/JsTimers.Tests.dll"
                                )
                                .SetTarget("dotnet")
                                .SetTargetArgs($"test {TestProject} --no-build --configuration {Configuration}")
                                .SetInclude("[JsTimers]*")
                                .SetOutput(ArtifactDirectories.Coverage / "coverage.JsTimers.xml")
                        )
                );

    Target CoverageCreateReport =>
        _ =>
            _.DependsOn(CoverageCollect)
                .Executes(
                    () =>
                        ReportGeneratorTasks.ReportGenerator(settings =>
                            settings
                                .SetReports(ArtifactDirectories.Coverage / "coverage.JsTimers.xml")
                                .SetTargetDirectory(
                                    ArtifactDirectories.Coverage / "report.JsTimers"
                                )
                        )
                );

    Configure<DotNetTestSettings> TestSettingsBase =>
        settings => settings.EnableNoBuild().SetConfiguration(Configuration);

    Configure<DotNetTestSettings> TestSettingsLoggers =>
        settings =>
            settings
                .AddLoggers("console;verbosity=detailed")
                .When(
                    HtmlTestResults,
                    settings =>
                        settings.AddLoggers("html;logfilename=test-results.JsTimers.Tests.html")
                )
                .When(
                    Host is GitHubActions,
                    settings =>
                        settings.AddLoggers(
                            "GitHubActions;summary.includePassedTests=true;summary.includeSkippedTests=true"
                        )
                );
}
