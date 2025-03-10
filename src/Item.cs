using System.ComponentModel.DataAnnotations;

class Item
{
    // fields
    public int Weight { get; }
    public string Description { get; }
    public string Name { get; }

    // constructor
    public Item(int weight, string description, string name)
    {
        Weight = weight;
        Description = description;
        Name = name;
    }

    // method to display item details
    public void Display()
    {
        Console.WriteLine($"Item: {Name}, Weight: {Weight}, Description: {Description}");
    }

}
