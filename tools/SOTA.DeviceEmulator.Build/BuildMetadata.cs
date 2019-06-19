using System.Globalization;
using System.Linq;
using Nuke.Common.Tools.GitVersion;

class BuildMetadata
{
    public BuildMetadata(GitVersion gitVersion)
    {
        BuildVersion = gitVersion.FullSemVer;
        var lowerCaseSemVer = gitVersion.FullSemVer.ToLowerInvariant();
        var semverParts = lowerCaseSemVer.Split('-');
        var index = 1;
        foreach (var part in semverParts.Skip(index))
        {
            semverParts[index] = part.Replace(".", string.Empty);
            index++;
        }
        UniversalPackageVersion = string.Join("-", semverParts);
        ReleaseType = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(gitVersion.PreReleaseLabel ?? "stable");
    }

    public string BuildVersion { get; }
    public string UniversalPackageVersion { get; }
    public string ReleaseType { get; }
}
