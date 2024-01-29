using Nuke.Common;
using Nuke.Common.IO;

interface IHazArtifacts : INukeBuild
{
    ArtifactPathCollection ArtifactDirectories =>
        new ArtifactPathCollection { Artifacts = RootDirectory / "artifacts" };

    void InitializeArtifactDirectories()
    {
        ArtifactDirectories.Artifacts.CreateDirectory();
        ArtifactDirectories.Packages.CreateDirectory();
        ArtifactDirectories.Libraries.CreateDirectory();
        ArtifactDirectories.Docs.CreateDirectory();
        ArtifactDirectories.TestResults.CreateDirectory();
        ArtifactDirectories.Coverage.CreateDirectory();
    }

    public readonly struct ArtifactPathCollection
    {
        public AbsolutePath Artifacts { get; init; }
        public AbsolutePath Packages => Artifacts / "pkg";
        public AbsolutePath Libraries => Artifacts / "lib";
        public AbsolutePath Docs => Artifacts / "docs";
        public AbsolutePath TestResults => Artifacts / "test_results";
        public AbsolutePath Coverage => TestResults / "coverage";
    }
}