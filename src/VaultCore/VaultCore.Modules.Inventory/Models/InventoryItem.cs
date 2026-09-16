namespace VaultCore.Modules.Inventory.Models;

public class InventoryItem
{
    public Guid Id { get; set; } // MiniatureId
    public string MiniatureName { get; set; } = string.Empty;
    public int AvailableStock { get; set; }
    public int ReservedStock { get; set; }
    public Guid Version { get; set; }
    public DateTimeOffset LastUpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}
