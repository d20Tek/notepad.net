using System.Text.Json;

namespace D20Tek.Notepad.Tui;

public sealed class SettingsService
{
    private const string SettingsFileName = "editor-settings.json";
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly string _settingsFilePath;

    public SettingsService()
    {
        var exeDirectory = AppContext.BaseDirectory;
        _settingsFilePath = Path.Combine(exeDirectory, SettingsFileName);
    }

    public SettingsService(string settingsFilePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(settingsFilePath);
        _settingsFilePath = settingsFilePath;
    }

    public EditorSettings Load()
    {
        try
        {
            if (!File.Exists(_settingsFilePath))
            {
                return EditorSettings.Default;
            }

            var json = File.ReadAllText(_settingsFilePath);
            var settings = JsonSerializer.Deserialize<EditorSettings>(json, _jsonOptions);
            return settings ?? EditorSettings.Default;
        }
        catch (Exception)
        {
            return EditorSettings.Default;
        }
    }

    public bool Save(EditorSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        try
        {
            var json = JsonSerializer.Serialize(settings, _jsonOptions);
            File.WriteAllText(_settingsFilePath, json);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public string SettingsFilePath => _settingsFilePath;
}
