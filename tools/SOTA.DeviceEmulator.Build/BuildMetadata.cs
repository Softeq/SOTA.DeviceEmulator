using System.Collections.Generic;
using System.Globalization;
using Nuke.Common.Tools.GitVersion;

class BuildMetadata
{
    public BuildMetadata(GitVersion gitVersion)
    {
        BuildVersion = gitVersion.FullSemVer.ToLowerInvariant();
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

    public string BuildVersion { get; private set; }
    public string ClickOnceProductName { get; private set; }
    public string ClickOnceApplicationVersion { get; private set; }
    // We need a different assembly name to allow side-by-side installation of versions from different channels (alpha, beta, stable) in ClickOnce.
    public string EntryAssemblyName { get; private set; }
    public string ReleaseType { get; private set; }
}
