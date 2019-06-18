using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.MSBuild;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.MSBuild.MSBuildTasks;
using static Nuke.Common.Tools.GitVersion.GitVersionTasks;

[CheckBuildProjectConfigurations]
[UnsetVisualStudioEnvironmentVariables]
class Build : NukeBuild
{
    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution] readonly Solution Solution;

    BuildMetadata Metadata;

    Project EntryProject => Solution.GetProject("SOTA.DeviceEmulator");

    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";
    AbsolutePath PackageDirectory => ArtifactsDirectory / "SOTA.DeviceEmulator";

    Target Clean => _ => _
                         .Before(Restore)
                         .Executes(() =>
                         {
                             EnsureCleanDirectory(ArtifactsDirectory);
                             DotNetClean(o => o.SetProject(Solution));
                         });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(o => o.SetProjectFile(Solution));
        });

    Target SetVersion => _ => _
        .Executes(() =>
        {
            var assemblyVersionFilePath = EntryProject.Directory / "Properties" / "AssemblyVersionInfo.cs";
            GitVersion(o =>
                o.SetEnsureAssemblyInfo(true)
                 .SetArgumentConfigurator(a => a.Add($"/updateassemblyinfo \"{assemblyVersionFilePath}\"")));
        });

    Target Package => _ => _
                           .DependsOn(Restore, SetVersion)
                           .Executes(() =>
                           {
                               MSBuild(o =>
                                   o.SetProjectFile(EntryProject)
                                    .SetConfiguration(Configuration)
                                    .SetTargets("Publish")
                                    .AddProperty("PublishDir", PackageDirectory + "\\")
                                    .AddProperty("ProductName", Metadata.ClickOnceProductName)
                                    .AddProperty("ApplicationVersion", Metadata.ClickOnceApplicationVersion)
                                    .AddProperty("EntryAssemblyName", Metadata.EntryAssemblyName));
                           });

    Target Test => _ => _
                        .DependsOn(Restore)
                        .Executes(() =>
                        {
                            var testProjects = Solution.GetProjects("*.Tests");
                            foreach (var testProject in testProjects)
                            {
                                DotNetTest(o => o.SetProjectFile(testProject).SetNoRestore(true));
                            }
                        });

    Target LocalBuild => _ => _
        .DependsOn(Test, Package);

    Target CiBuild => _ => _
        .DependsOn(Clean, Test, Package);

    /// Support plugins are available for:
    /// - JetBrains ReSharper        https://nuke.build/resharper
    /// - JetBrains Rider            https://nuke.build/rider
    /// - Microsoft VisualStudio     https://nuke.build/visualstudio
    /// - Microsoft VSCode           https://nuke.build/vscode
    public static int Main()
    {
        return Execute<Build>(x => x.LocalBuild);
    }

    protected override void OnBuildInitialized()
    {
        base.OnBuildInitialized();
        var gitVersion = GitVersion(o => o.SetLogOutput(false).SetOutput(GitVersionOutput.json)).Result;
        Metadata = new BuildMetadata(gitVersion);
    }
}