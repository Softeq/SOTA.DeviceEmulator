using System.Collections.Generic;
using System.Globalization;
using Nuke.Common.Tools.GitVersion;

class BuildMetadata
{
    public BuildMetadata(GitVersion gitVersion)
    {
        var nameParts = new List<string> {"SOTA Device Emulator"};
        if (!string.IsNullOrEmpty(gitVersion.PreReleaseLabel))
        {
            nameParts.Add(CultureInfo.InvariantCulture.TextInfo.ToTitleCase(gitVersion.PreReleaseLabel));
        }
        ClickOnceProductName = string.Join(" ", nameParts);
        ClickOnceApplicationVersion = $"{gitVersion.MajorMinorPatch}.{gitVersion.PreReleaseNumber}";
        EntryAssemblyName = $"SOTA.DeviceEmulator.{CultureInfo.InvariantCulture.TextInfo.ToTitleCase(gitVersion.PreReleaseLabel)}";
    }

    public string ClickOnceProductName { get; private set; }
    public string ClickOnceApplicationVersion { get; private set; }
    // We need a different assembly name to allow side-by-side installation of versions from different channels (alpha, beta, stable) in ClickOnce.
    public string EntryAssemblyName { get; private set; }


}