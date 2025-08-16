﻿using System;
using System.Collections.Generic;
public interface IInventoryItem
{
    int Id { get; }
    string Name { get; }
    int Quantity { get; set; }
}

public class ElectronicItem : IInventoryItem
{
    public int Id { get; }
    public string Name { get; }
    public int Quantity { get; set; }
    public string Brand { get; }
    public int WarrantyMonths { get; }

    public ElectronicItem(int id, string name, int quantity, string brand, int warrantyMonths)
    {
        Id = id;
        Name = name;
        Quantity = quantity;
        Brand = brand;
        WarrantyMonths = warrantyMonths;
    }
}

public class GroceryItem : IInventoryItem
{
    public int Id { get; }
    public string Name { get; }
    public int Quantity { get; set; }
    public DateTime ExpiryDate { get; }

    public GroceryItem(int id, string name, int quantity, DateTime expiryDate)
    {
        Id = id;
        Name = name;
        Quantity = quantity;
        ExpiryDate = expiryDate;
    }
}

public class DuplicateItemException : Exception
{
    public DuplicateItemException(string message) : base(message) { }
}

public class ItemNotFoundException : Exception
{
    public ItemNotFoundException(string message) : base(message) { }
}

public class InvalidQuantityException : Exception
{
    public InvalidQuantityException(string message) : base(message) { }
}

public class InventoryRepository<T> where T : IInventoryItem
{
    private readonly Dictionary<int, T> _items = new();

    public void AddItem(T item)
    {
        if (_items.ContainsKey(item.Id))
            throw new DuplicateItemException($"Item with ID {item.Id} already exists.");
        _items[item.Id] = item;
    }

    public T GetItemById(int id)
    {
        if (!_items.TryGetValue(id, out var item))
            throw new ItemNotFoundException($"Item with ID {id} not found.");
        return item;
    }

    public void RemoveItem(int id)
    {
        if (!_items.Remove(id))
            throw new ItemNotFoundException($"Item with ID {id} not found.");
    }

    public List<T> GetAllItems() => new List<T>(_items.Values);

    public void UpdateQuantity(int id, int newQuantity)
    {
        if (newQuantity < 0)
            throw new InvalidQuantityException("Quantity cannot be negative.");

        var item = GetItemById(id);
        item.Quantity = newQuantity;
    }
}

public class WareHouseManager
{
    private readonly InventoryRepository<ElectronicItem> _electronics = new();
    private readonly InventoryRepository<GroceryItem> _groceries = new();

    public void SeedData()
    {
        try
        {
            _electronics.AddItem(new ElectronicItem(1, "Laptop", 10, "Dell", 24));
            _electronics.AddItem(new ElectronicItem(2, "Smartphone", 20, "Samsung", 12));
            _electronics.AddItem(new ElectronicItem(3, "Tablet", 15, "Apple", 12));

            _groceries.AddItem(new GroceryItem(1, "Milk", 50, DateTime.Now.AddDays(7)));
            _groceries.AddItem(new GroceryItem(2, "Bread", 30, DateTime.Now.AddDays(3)));
            _groceries.AddItem(new GroceryItem(3, "Eggs", 100, DateTime.Now.AddDays(10)));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during seeding: {ex.Message}");
        }
    }

    public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
    {
        var items = repo.GetAllItems();
        Console.WriteLine($"\nListing all {typeof(T).Name}s:");
        foreach (var item in items)
        {
            if (item is ElectronicItem e)
                Console.WriteLine($"ID: {e.Id}, Name: {e.Name}, Qty: {e.Quantity}, Brand: {e.Brand}, Warranty: {e.WarrantyMonths} months");
            else if (item is GroceryItem g)
                Console.WriteLine($"ID: {g.Id}, Name: {g.Name}, Qty: {g.Quantity}, Expiry: {g.ExpiryDate.ToShortDateString()}");
        }
    }

    public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : IInventoryItem
    {
        try
        {
            var item = repo.GetItemById(id);
            repo.UpdateQuantity(id, item.Quantity + quantity);
            Console.WriteLine($"Increased stock of {item.Name} to {item.Quantity}.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error increasing stock: {ex.Message}");
        }
    }

    public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
    {
        try
        {
            repo.RemoveItem(id);
            Console.WriteLine($"Item with ID {id} removed successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error removing item: {ex.Message}");
        }
    }

    public InventoryRepository<ElectronicItem> ElectronicsRepo => _electronics;
    public InventoryRepository<GroceryItem> GroceriesRepo => _groceries;
}

class Program
{
    static void Main()
    {
        var manager = new WareHouseManager();

        manager.SeedData();

        manager.PrintAllItems(manager.GroceriesRepo);
        manager.PrintAllItems(manager.ElectronicsRepo);

        Console.WriteLine("\nAttempting to add duplicate item:");
        try
        {
            manager.ElectronicsRepo.AddItem(new ElectronicItem(1, "Laptop", 5, "HP", 12));
        }
        catch (DuplicateItemException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        Console.WriteLine("\nAttempting to remove non-existent item:");
        manager.RemoveItemById(manager.GroceriesRepo, 999);

        Console.WriteLine("\nAttempting to update invalid quantity:");
        try
        {
            manager.ElectronicsRepo.UpdateQuantity(2, -10);
        }
        catch (InvalidQuantityException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        Console.WriteLine("\nIncreasing stock of Grocery item (Milk):");
        manager.IncreaseStock(manager.GroceriesRepo, 1, 20);
    }
}