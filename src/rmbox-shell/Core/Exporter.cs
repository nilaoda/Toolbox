using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using Ruminoid.Toolbox.Shell.ViewModels.Project;

// ReSharper disable MemberCanBePrivate.Global

namespace Ruminoid.Toolbox.Shell.Core
{
    public static class Exporter
    {
        public static object ExportProjectToObject(
            ProjectViewModel project) =>
            project switch
            {
                SingleProjectViewModel singleProject => new
                {
                    version = 1,
                    type = "project",
                    operation = singleProject.OperationModel.Id,
                    sections =
                        singleProject.ConfigSections
                            .Select(x =>
                                new
                                {
                                    type = x.ConfigSectionAttribute.Id,
                                    data = x.ConfigSection
                                })
                            .ToArray()
                },
                BatchProjectViewModel batchProject => new
                {
                    version = 1,
                    type = "batch",
                    inputs = batchProject.BatchConfig["inputs"]!.ToObject<List<string>>(),
                    subtitle_format = batchProject.BatchConfig["subtitle_format"]!.ToString(),
                    output_format = batchProject.BatchConfig["output_format"]!.ToString(),
                    operation = batchProject.OperationModel.Id,
                    sections =
                        batchProject.ConfigSections
                            .Select(x =>
                                new
                                {
                                    type = x.ConfigSectionAttribute.Id,
                                    data = x.ConfigSection
                                })
                            .ToArray()
                },
                _ => throw new ArgumentOutOfRangeException(nameof(project), project, null)
            };

        public static JObject ExportProjectToJObject(
            ProjectViewModel project) =>
            JObject.FromObject(ExportProjectToObject(project));

        public static string ExportProjectToJsonString(
            ProjectViewModel project) =>
            ExportProjectToJObject(project).ToString();

        public static void ExportProjectToFile(
            ProjectViewModel project,
            string path) =>
            File.WriteAllText(path, ExportProjectToJsonString(project));
    }
}
