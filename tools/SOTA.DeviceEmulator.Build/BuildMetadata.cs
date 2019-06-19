using System.Globalization;
using System.Linq;
using Colorful;
using Nuke.Common.BuildServers;
using Nuke.Common.Tools.GitVersion;

class BuildMetadata
{
    public BuildMetadata(GitVersion gitVersion)
    {
        BuildVersion = gitVersion.FullSemVer;
        ReleaseGitTag = $"v{gitVersion.FullSemVer}";
        var universalSemver = new string(gitVersion.FullSemVer.ToLowerInvariant().TakeWhile(c => c != '+').ToArray());
        var universalSemverParts = universalSemver.Split('-');
        var index = 1;
        foreach (var part in universalSemverParts.Skip(index))
        {
            universalSemverParts[index] = part.Replace(".", string.Empty);
            index++;
        }
        UniversalPackageVersion = string.Join("-", universalSemverParts);
        ReleaseType = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(gitVersion.PreReleaseLabel ?? "stable");
    }

    public string BuildVersion { get; }
    public string ReleaseGitTag { get; }
    public string UniversalPackageVersion { get; }
    public string UniversalPackageDescription => $"SOTA Device Emulator ({ReleaseType} channel)";
    public string ReleaseType { get; }

    public void SetToCi()
    {
        if (TeamServices.Instance != null)
        {
            TeamServices.Instance.UpdateBuildNumber(BuildVersion);
            Set(nameof(UniversalPackageVersion), UniversalPackageVersion);
            Set(nameof(UniversalPackageDescription), UniversalPackageDescription);
            Set(nameof(ReleaseType), ReleaseType);
            Set(nameof(ReleaseGitTag), ReleaseGitTag);
        }
    }

    void Set(string name, string value)
    {
        Console.WriteLine($"##vso[task.setvariable variable={name}]{value}");
    }
}
