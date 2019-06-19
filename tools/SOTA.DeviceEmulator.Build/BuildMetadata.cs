using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Nuke.Common.Tools.GitVersion;

class BuildMetadata
{
    public BuildMetadata(GitVersion gitVersion)
    {
        var lowerCaseSemVer = gitVersion.FullSemVer.ToLowerInvariant();
        var semverParts = lowerCaseSemVer.Split('-');
        var index = 1;
        foreach (var part in semverParts.Skip(index))
        {
            semverParts[index] = part.Replace(".", string.Empty);
            index++;
        }
        BuildVersion = string.Join("-", semverParts);
        var nameParts = new List<string> {"SOTA Device Emulator"};
        ReleaseType = gitVersion.PreReleaseLabel ?? "stable";
        var releaseTypeTitle = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(ReleaseType);
        if (!string.IsNullOrEmpty(gitVersion.PreReleaseLabel))
        {
            nameParts.Add(releaseTypeTitle);
        }
        ClickOnceProductName = string.Join(" ", nameParts);
        ClickOnceApplicationVersion = $"{gitVersion.MajorMinorPatch}.{gitVersion.PreReleaseNumber}";
        EntryAssemblyName = $"SOTA.DeviceEmulator.{releaseTypeTitle}";
    }

    public string BuildVersion { get; }
    public string ClickOnceProductName { get; }
    public string ClickOnceApplicationVersion { get; }
    // We need a different assembly name to allow side-by-side installation of versions from different channels (alpha, beta, stable) in ClickOnce.
    public string EntryAssemblyName { get; }
    public string ReleaseType { get; }
}
