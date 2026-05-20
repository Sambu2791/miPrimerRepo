using System.IO;
using System.Text.Json;

const string configFileName = "appsettings.json";

Console.WriteLine("Simulación de cadena de conexión");

if (!File.Exists(configFileName))
{
    Console.WriteLine($"No se encontró el archivo de configuración '{configFileName}'.");
    return;
}

try
{
    var json = await File.ReadAllTextAsync(configFileName);
    using var document = JsonDocument.Parse(json);

    if (document.RootElement.TryGetProperty("ConnectionStrings", out var connectionStrings) &&
        connectionStrings.TryGetProperty("DefaultConnection", out var defaultConnection) &&
        defaultConnection.ValueKind == JsonValueKind.String)
    {
        var connectionString = defaultConnection.GetString();
        Console.WriteLine("Cadena de conexión simulada leída desde configuración:");
        Console.WriteLine(connectionString);
        Console.WriteLine();
        Console.WriteLine("Simulando conexión a la base de datos...");
        Console.WriteLine($"Conectando a: {ExtractDatabaseName(connectionString)}");
        Console.WriteLine("Estado: Conexión simulada establecida correctamente.");
    }
    else
    {
        Console.WriteLine("No se encontró la cadena de conexión 'DefaultConnection' en appsettings.json.");
    }
}
catch (JsonException ex)
{
    Console.WriteLine($"Error al parsear appsettings.json: {ex.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"Error inesperado: {ex.Message}");
}

static string ExtractDatabaseName(string? connectionString)
{
    if (string.IsNullOrEmpty(connectionString))
        return "(desconocido)";

    var parts = connectionString.Split(';', StringSplitOptions.RemoveEmptyEntries);
    foreach (var part in parts)
    {
        var kv = part.Split('=', 2);
        if (kv.Length == 2 && string.Equals(kv[0].Trim(), "Database", StringComparison.OrdinalIgnoreCase))
            return kv[1].Trim();
    }

    return "(desconocido)";
}

