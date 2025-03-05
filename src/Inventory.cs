class Inventory{
    // fields
    private int maxWeight;
    private Dictionary<string, Item> items;

    // constructor
    public Inventory(int maxWeight)
    {
        this.maxWeight = maxWeight;
        this.items = new Dictionary<string, Item>();
    }
    // methods
    public int TotalWeight()
    {
        int total = 0;
        foreach (var item in items.Values)
        {
            total += item.Weight;
        }
        return total;
    }

    public int FreeWeight()
    {
        return maxWeight - TotalWeight();
    }
    public bool Put(string itemName, Item item)
    {
        if (item.Weight + TotalWeight() <= maxWeight)
        {
            items[itemName] = item;
            return true;
        }
        return false;
    }
    public Item Get(string itemName){
        if (items.TryGetValue(itemName, out Item item))
        {
            items.Remove(itemName);
            return item;
        }
        return null;
    }
}