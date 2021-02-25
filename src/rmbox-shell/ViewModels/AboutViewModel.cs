﻿using System.Reflection;
using ReactiveUI;

namespace Ruminoid.Toolbox.Shell.ViewModels
{
    public class AboutViewModel : ReactiveObject
    {
        public string VersionSummary { get; } = $"版本 v{Assembly.GetExecutingAssembly().GetName().Version}";

        public string VersionDetail { get; } = Assembly.GetExecutingAssembly()
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion;
    }
}
