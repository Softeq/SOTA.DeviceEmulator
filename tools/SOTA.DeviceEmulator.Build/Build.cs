using System.Linq;
using Nuke.Common;
using Nuke.Common.BuildServers;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.MSBuild;
using Nuke.Common.Tools.NuGet;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.MSBuild.MSBuildTasks;
using static Nuke.Common.Tools.GitVersion.GitVersionTasks;
using static Nuke.Common.Tools.NuGet.NuGetTasks;

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
    AbsolutePath NuGetPackagesDirectory => RootDirectory / "packages";
    AbsolutePath PackageDirectory => ArtifactsDirectory / "SOTA.DeviceEmulator";

    Target Clean => _ => _
                         .Before(Restore)
                         .Executes(() =>
                         {
                             EnsureCleanDirectory(ArtifactsDirectory);
                             EnsureCleanDirectory(NuGetPackagesDirectory);
                             DotNetClean(o => o.SetProject(Solution));
                         });

    Target Restore => _ => _
        .Executes(() =>
        {
            NuGetRestore(o => o.SetSolutionDirectory(Solution.Directory));
        });

    Target SetVersion => _ => _
        .Executes(() =>
        {
            var assemblyVersionFilePath = EntryProject.Directory / "Properties" / "AssemblyVersionInfo.cs";
            var gitVersion = GitVersion(o =>
                    o.SetEnsureAssemblyInfo(true)
                     .SetArgumentConfigurator(a => a.Add($"/updateassemblyinfo \"{assemblyVersionFilePath}\"")))
                .Result;
            if (TeamServices.Instance != null)
            {
                TeamServices.Instance.UpdateBuildNumber(gitVersion.FullSemVer);
            }
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
                               // Application manifest file (.application) is not generated when publish package is generated
                               // using MSBuild. But we need it for an ability to install older versions
                               // So we apply a hack to copy it manually similarly to this answer in StackOverflow
                               // https://stackoverflow.com/questions/23221089/missing-manifest-file-in-applicationfiles-folder-with-msbuild-in-nant-task
                               var applicationManifestFile = GlobFiles(PackageDirectory, "*.application").First();
                               var clickOnceUnderscoreVersion = Metadata.ClickOnceApplicationVersion.Replace(".", "_");
                               var clickOnceVersionFolderName =
                                   $"{Metadata.EntryAssemblyName}_{clickOnceUnderscoreVersion}";
                               var versionedApplicationDirectory =
                                   PackageDirectory / "Application Files" / clickOnceVersionFolderName;
                               CopyFileToDirectory(applicationManifestFile, versionedApplicationDirectory, FileExistsPolicy.Overwrite);
                           });

    Target Test => _ => _
                        .DependsOn(Restore)
                        .Executes(() =>
                        {
                            var testProjects = Solution.GetProjects("*.Tests");
                            foreach (var testProject in testProjects)
                            {
                                DotNetTest(o =>
                                {
                                    var config = o.SetProjectFile(testProject).SetNoRestore(true);
                                    if (TeamServices.Instance == null)
                                    {
                                        return config;
                                    }
                                    var testResultsFile = Solution.Directory / "TestResults" / $"{testProject.Name}.TestResults.trx";
                                    config = config.SetLogger($"trx;LogFileName={testResultsFile}");
                                    return config;
                                });
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