using System.Collections.Generic;

class Player
{
    // auto property
    public Room CurrentRoom { get; set; }

    // player's health
    private int health;


    // player loses some health...
    public void Damage(int amount)
    {
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
    }
}