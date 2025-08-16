using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

public interface IInventoryEntity
{
    int Id { get; }
}

public class InventoryLogger<T> where T : IInventoryEntity
{
    private readonly List<T> _log = new();
    private readonly string _filePath;

    public InventoryLogger(string filePath)
    {
        _filePath = filePath;
    }

    public void Add(T item)
    {
        _log.Add(item);
    }

    public List<T> GetAll()
    {
        return new List<T>(_log);
    }

    public void SaveToFile()
    {
        try
        {
            using var stream = new FileStream(_filePath, FileMode.Create, FileAccess.Write);
            JsonSerializer.Serialize(stream, _log, new JsonSerializerOptions { WriteIndented = true });
            Console.WriteLine($"Data successfully saved to {_filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving to file: {ex.Message}");
        }
    }

    public void LoadFromFile()
    {
        try
        {
            if (!File.Exists(_filePath))
            {
                Console.WriteLine("File does not exist. No data loaded.");
                return;
            }

            using var stream = new FileStream(_filePath, FileMode.Open, FileAccess.Read);
            var items = JsonSerializer.Deserialize<List<T>>(stream);
            if (items != null)
            {
                _log.Clear();
                _log.AddRange(items);
                Console.WriteLine($"Data successfully loaded from {_filePath}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading from file: {ex.Message}");
        }
    }
}

public class InventoryApp
{
    private readonly InventoryLogger<InventoryItem> _logger = new("inventory.json");

    public void SeedSampleData()
    {
        _logger.Add(new InventoryItem(1, "Laptop", 10, DateTime.Now.AddDays(-10)));
        _logger.Add(new InventoryItem(2, "Mouse", 50, DateTime.Now.AddDays(-5)));
        _logger.Add(new InventoryItem(3, "Keyboard", 30, DateTime.Now.AddDays(-7)));
        _logger.Add(new InventoryItem(4, "Monitor", 15, DateTime.Now.AddDays(-3)));
        _logger.Add(new InventoryItem(5, "USB Drive", 100, DateTime.Now));
    }

    public void SaveData()
    {
        _logger.SaveToFile();
    }

    public void LoadData()
    {
        _logger.LoadFromFile();
    }

    public void PrintAllItems()
    {
        var items = _logger.GetAll();
        Console.WriteLine("\nInventory Items:");
        foreach (var item in items)
        {
            Console.WriteLine($"ID: {item.Id}, Name: {item.Name}, Quantity: {item.Quantity}, Date Added: {item.DateAdded}");
        }
    }
}

class Program
{
    static void Main()
    {
        var app = new InventoryApp();

        app.SeedSampleData();
        app.SaveData();

        var newAppSession = new InventoryApp();

        newAppSession.LoadData();
        newAppSession.PrintAllItems();
    }
}