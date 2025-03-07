using System.Collections.Generic;

class Player
{

    // field
    private Inventory backpack;

    // auto property
    public Room CurrentRoom { get; set; }

    // player's health
    private int health;

    public int Health => health;


    // player loses some health...
    public void Damage(int amount)
    {
       //health -= amount;
       health -= amount;
        if (health < 0)
       {
           health = 0;
       }
    }
    // player's health restores...
    public void Heal(int amount)
    {
        health += amount;
        if (health > 100)
        {
            health = 100;
        }
    }
    // checks whether the player is alive or not
    public bool IsAlive()
    {
        return health > 0;
    }

    //constructor
    public Player()
    {
        CurrentRoom = null;
        health = 100;
        // 25kg is pretty heavy to carry around all day
        backpack = new Inventory(25); 
    }

    public bool TakeFromChest(string itemName)
    {
        // Find the item in the room
        Item itemToTake = CurrentRoom.GetItems().FirstOrDefault(item => item.Name == itemName);
        if (itemToTake == null)
        {
            Console.WriteLine($"Item '{itemName}' not found in the room.");
            return false;
        }

        // Remove the item from the room
        bool removedFromRoom = CurrentRoom.GetItems().Remove(itemToTake);
        if (!removedFromRoom)
        {
            Console.WriteLine($"Failed to remove item '{itemName}' from the room.");
            return false;
        }

        // Try to add the item to the backpack
        if (backpack.Put(itemName, itemToTake))
        {
            Console.WriteLine($"Item '{itemName}' taken from the chest and added to the backpack.");
            return true;
        }
        else
        {
            // If the item doesn't fit in the backpack, put it back in the room
            CurrentRoom.GetItems().Add(itemToTake);
            Console.WriteLine($"Item '{itemName}' doesn't fit in the backpack and was put back in the chest.");
            return false;
        }
    }

    public bool DropToChest(string itemName)
    {
        Item itemToDrop = backpack.Get(itemName);
        if (itemToDrop == null)
        {
            Console.WriteLine($"Item '{itemName}' not found in the backpack.");
            return false;
        }

        // Try to add the item to the room
        CurrentRoom.AddItem(itemToDrop);
        Console.WriteLine($"Item '{itemName}' dropped to the chest.");

        return false;
     
    }
    public string ShowInventory()
    {
        return backpack.Show();


    }
}