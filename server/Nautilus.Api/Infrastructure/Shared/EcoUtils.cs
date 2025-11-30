namespace Nautilus.Api.Infrastructure.Shared;

public static class EcoUtils
{
    /// <summary>
    /// Normalizes scientific names to "genus species" format.
    /// </summary>
    public static string NormalizeScientificName(string scientificName)
    {
        var parts = scientificName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length >= 2)
        {
            return $"{parts[0]} {parts[1]}";
        }
        return scientificName;
    }
}