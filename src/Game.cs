using System.Collections.Generic;
using System.Reflection.Metadata;

class Game
{
	// Private fields
	private Parser parser;
	private Player player;
	private Room winnigRoom;

	// Constructor
	public Game()
	{
		parser = new Parser();
		player = new Player();
		CreateRooms();
	}

	// Initialise the Rooms (and the Items)
	private void CreateRooms()
	{
		// Create the rooms
		Room outside = new Room("outside the main entrance of the university");
		Room theatre = new Room("in a lecture theatre");
		Room pub = new Room("in the campus pub");
		Room lab = new Room("in a computing lab");
		Room office = new Room("in the computing admin office");
		Room up = new Room("upstairs");
		Room down = new Room("downstairs");

		// Initialise room exits
		outside.AddExit("east", theatre);
		outside.AddExit("south", lab);
		outside.AddExit("west", pub);
		outside.AddExit("up", up);
		outside.AddExit("down", down);

		up.AddExit("down", outside);
		down.AddExit("up", outside);

		theatre.AddExit("west", outside);

		pub.AddExit("east", outside);

		lab.AddExit("north", outside);
		lab.AddExit("east", office);

		office.AddExit("west", lab);

		// Create your Items here
		Item healer = new Item(3, "A healer gives you the heal that's you losen when you are visiting the rooms", "healer");
		// And add them to the Rooms
		lab.AddItem(healer);

		// Start game outside

		player.CurrentRoom = outside;
		winnigRoom = office;
	}

	//  Main play routine. Loops until end of play.
	public void Play()
	{
		PrintWelcome();

		// Enter the main command loop. Here we repeatedly read commands and
		// execute them until the player wants to quit.
		bool finished = false;
		while (!finished)
		{
			if (!player.IsAlive())
			{
				Console.WriteLine("You have died. game Over");
				finished = true;
				continue;
			}

			if (player.CurrentRoom == winnigRoom)
			{
                Console.WriteLine("Congratulations! You have reached the office and won the game!");
				//Environment.Exit(0); // End the game
				finished = true;
				continue;
			}
			Command command = parser.GetCommand();
			finished = ProcessCommand(command);
		}
		Console.WriteLine("Thank you for playing.");
		Console.WriteLine("Press [Enter] to continue.");
		Console.ReadLine();
	}

	// Print out the opening message for the player.
	private void PrintWelcome()
	{
		Console.WriteLine();
		Console.WriteLine("Welcome to Zuul!");
		Console.WriteLine("Zuul is a new, incredibly boring adventure game.");
		Console.WriteLine("Type 'help' if you need help.");
		Console.WriteLine();
		Console.WriteLine(player.CurrentRoom.GetLongDescription());
	}

	// Given a command, process (that is: execute) the command.
	// If this command ends the game, it returns true.
	// Otherwise false is returned.
	private bool ProcessCommand(Command command)
	{
		bool wantToQuit = false;

		if (command.IsUnknown())
		{
			Console.WriteLine("I don't know what you mean...");
			return wantToQuit; // false
		}

		switch (command.CommandWord)
		{
			case "help":
				PrintHelp();
				break;
			case "go":
				GoRoom(command);
				break;
			case "look":
				PrintLook(command);
				break;
			case "status":
				PrintStatus();
				break;
			case "take":
				Take(command);
				break;
			case "drop":
				Drop(command);
				break;
			case "use":
				Use(command);
				break;
			case "quit":
				wantToQuit = true;
				break;
		}

		return wantToQuit;
	}

	// ######################################
	// implementations of user commands:
	// ######################################

	// Print out some help information.
	// Here we print the mission and a list of the command words.
	private void PrintHelp()
	{
		Console.WriteLine("You are lost. You are alone.");
		Console.WriteLine("You wander around at the university.");
		Console.WriteLine();
		// let the parser print the commands
		parser.PrintValidCommands();
	}

	// Try to go to one direction. If there is an exit, enter the new
	// room, otherwise print an error message.
	private void GoRoom(Command command)
	{
		if (!command.HasSecondWord())
		{
			// if there is no second word, we don't know where to go...
			Console.WriteLine("Go where?");
			return;
		}

		string direction = command.SecondWord;

		// Try to go to the next room.
		Room nextRoom = player.CurrentRoom.GetExit(direction);
		if (nextRoom == null)
		{
			Console.WriteLine("There is no door to " + direction + "!");
			return;
		}
        player.Damage(10);
        player.CurrentRoom = nextRoom;
		Console.WriteLine(player.CurrentRoom.GetLongDescription());

		if (player.CurrentRoom == winnigRoom)
		{
			//Console.WriteLine("Congratulations! You have reached the office and won the game!");
			//Environment.Exit(0); // End the game
		}
	}


	private void PrintLook(Command command)
	{
		List<Item> itemsInRoom = player.CurrentRoom.GetItems();
		if (itemsInRoom.Count > 0)
		{
			Console.WriteLine("You see the following items:");
			foreach (Item item in itemsInRoom)
			{
				Console.WriteLine($"- {item.Name}: {item.Description}");
			}
		}
		else
		{
			Console.WriteLine("There are no items in this room.");
		}
	}
	private void PrintStatus()
	{
		Console.WriteLine($"Player health: {player.Health}");
		Console.WriteLine($"{player.ShowInventory()}");

        Console.WriteLine($"you have {player.GetFreeWeight()} KG free to use");

    }

	// Implement the Take and Drop commands
	private void Take(Command command)
	{
		if (!command.HasSecondWord())
		{
			Console.WriteLine("Take what?");
			return;
		}

		string itemName = command.SecondWord;

		if (player.TakeFromChest(itemName))
		{
			Console.WriteLine($"You have taken the {itemName}.");
		}
		else
		{
			Console.WriteLine($"You could not take the {itemName}.");
		}

	}
	private void Drop(Command command)
	{
		if (!command.HasSecondWord())
		{
			Console.WriteLine("Drop what?");
			return;
		}

		string itemName = command.SecondWord;

		player.DropToChest(itemName);
	}
	private void Use(Command command)
	{
        if (!command.HasSecondWord())
        {
            Console.WriteLine("Use what?");
            return;
        }
		
        Console.WriteLine(player.UseItem(command.SecondWord));
    }
}