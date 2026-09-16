namespace VaultCore.Modules.Catalog.Models;

public class Miniature
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Universe { get; set; } = "Warhammer 40,000";
    public string Faction { get; set; } = string.Empty;
    public string Subfaction { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Material { get; set; } = "Citadel Plastic";
    public string Scale { get; set; } = "28-32mm";
    public int BaseSizeMm { get; set; } = 32;
    public string ImageUrl { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = [];
    public List<WargearOption> WargearOptions { get; set; } = [];
    public bool IsFeatured { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
