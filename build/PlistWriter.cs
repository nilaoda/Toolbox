using System;
using System.IO;
using System.Xml;
using Nuke.Common;

public class Plist
{
    #region Data

    public string CFBundleName { get; init; }

    public string CFBundleDisplayName { get; init; }

    public string CFBundleSpokenName { get; init; }

    public string LSApplicationCategoryType { get; init; }

    public string CFBundleIdentifier { get; init; }

    public string CFBundleVersion { get; init; }

    public string CFBundleExecutable { get; init; }

    public string CFBundleIconFileName { get; init; }

    public string CFBundleShortVersionString { get; init; }

    #endregion

    #region Writer

    public void Write(string contentsDirectory)
    {
        var settings = new XmlWriterSettings
        {
            Indent = true,
            NewLineOnAttributes = false
        };

        var path = Path.Combine(contentsDirectory, "Info.plist");

        Logger.Info($"Writing property list file: {path}");
        using var xmlWriter = XmlWriter.Create(path, settings);

        xmlWriter.WriteStartDocument();

        xmlWriter.WriteRaw(Environment.NewLine);
        xmlWriter.WriteRaw(
            "<!DOCTYPE plist PUBLIC \"-//Apple//DTD PLIST 1.0//EN\" \"http://www.apple.com/DTDs/PropertyList-1.0.dtd\">");
        xmlWriter.WriteRaw(Environment.NewLine);

        xmlWriter.WriteStartElement("plist");
        xmlWriter.WriteAttributeString("version", "1.0");
        xmlWriter.WriteStartElement("dict");

        WriteProperty(xmlWriter, nameof(CFBundleName), CFBundleName);
        WriteProperty(xmlWriter, nameof(CFBundleDisplayName), CFBundleDisplayName);
        WriteProperty(xmlWriter, nameof(CFBundleSpokenName), CFBundleSpokenName);
        WriteProperty(xmlWriter, nameof(CFBundleIdentifier), CFBundleIdentifier);
        WriteProperty(xmlWriter, nameof(CFBundleVersion), CFBundleVersion);
        WriteProperty(xmlWriter, "CFBundlePackageType", "APPL");
        //WriteProperty(xmlWriter, nameof(CFBundleSignature), CFBundleSignature);
        WriteProperty(xmlWriter, nameof(CFBundleExecutable), CFBundleExecutable);
        WriteProperty(xmlWriter, nameof(CFBundleIconFileName), CFBundleIconFileName);
        WriteProperty(xmlWriter, nameof(CFBundleShortVersionString), CFBundleShortVersionString);
        WriteProperty(xmlWriter, nameof(LSApplicationCategoryType), LSApplicationCategoryType);
        WriteProperty(xmlWriter, "NSPrincipalClass", "NSApplication");
        WriteProperty(xmlWriter, "NSHumanReadableCopyright", "2021 Il Harper");
        WriteProperty(xmlWriter, "NSHighResolutionCapable", true);

        xmlWriter.WriteEndElement();
        xmlWriter.WriteEndElement();
    }

    static void WriteProperty(XmlWriter xmlWriter, string name, string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return;
        xmlWriter.WriteStartElement("key");
        xmlWriter.WriteString(name);
        xmlWriter.WriteEndElement();
        xmlWriter.WriteStartElement("string");
        xmlWriter.WriteString(value);
        xmlWriter.WriteEndElement();
    }

    static void WriteProperty(XmlWriter xmlWriter, string name, bool value)
    {
        xmlWriter.WriteStartElement("key");
        xmlWriter.WriteString(name);
        xmlWriter.WriteEndElement();
        xmlWriter.WriteStartElement(value ? "true" : "false");
        xmlWriter.WriteEndElement();
    }

    #endregion
}
