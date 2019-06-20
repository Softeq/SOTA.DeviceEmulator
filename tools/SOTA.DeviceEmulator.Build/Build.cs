using System.IO;
using Newtonsoft.Json;
using Nuke.Common;
using Nuke.Common.BuildServers;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.MSBuild;
using static Nuke.Common.Logger;
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
    AbsolutePath NuGetPackagesDirectory => RootDirectory / "packages";

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
            DotNetRestore(o => o.SetProjectFile(Solution));
            MSBuild(o => o.SetProjectFile(EntryProject).SetTargets("Restore"));
        });

    Target SetAssemblyVersion => _ => _
        .Executes(() =>
        {
            Info($"Build Version: {Metadata.BuildVersion}");
            var assemblyVersionFilePath = EntryProject.Directory / "Properties" / "AssemblyVersionInfo.cs";
            GitVersion(o =>
                o.SetEnsureAssemblyInfo(true)
                 .SetArgumentConfigurator(a => a.Add($"/updateassemblyinfo \"{assemblyVersionFilePath}\"")));
        });

    Target CiSetBuildMetadata => _ => _
        .Executes(() =>
        {
            Metadata.SetToCi();
        });

    Target Package => _ => _
                           .DependsOn(Restore, SetAssemblyVersion)
                           .Executes(() =>
                           {
                               MSBuild(o =>
                                   o.SetProjectFile(EntryProject)
                                    .SetConfiguration(Configuration)
                                    .SetTargets("Build")
                                    .AddProperty("OutDir", ArtifactsDirectory));
                               var metadataJson = JsonConvert.SerializeObject(Metadata);
                               var metadataFilePath = ArtifactsDirectory / "app-package.json";
                               File.WriteAllText(metadataFilePath, metadataJson);
                               var metadataScriptFilePath =
                                   // ReSharper disable once PossibleNullReferenceException
                                   Solution.GetProject("SOTA.DeviceEmulator.Build").Directory / "set-metadata.ps1";
                               CopyFileToDirectory(metadataScriptFilePath, ArtifactsDirectory, FileExistsPolicy.Overwrite);
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

                                    var testResultsFile =
                                        Solution.Directory / "TestResults" / $"{testProject.Name}.TestResults.trx";
                                    config = config.SetLogger($"trx;LogFileName={testResultsFile}");
                                    return config;
                                });
                            }
                        });

    Target LocalBuild => _ => _
        .DependsOn(Test, Package);

    Target CiBuild => _ => _
        .DependsOn(CiSetBuildMetadata, Clean, Test, Package);

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