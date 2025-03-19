using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Metadata;

class Game
{
	// Private fields
	private Parser parser;
	private Player player;
	private Room winnigRoom;
	private Room KeyRoom;
    private Stopwatch stopwatch;


    // Constructor
    public Game()
	{
		parser = new Parser();
		player = new Player();
        stopwatch = new Stopwatch();
        CreateRooms();
    }

	// Initialise the Rooms (and the Items)
	private void CreateRooms()
	{
		// Create the rooms

		Room entrance = new Room("in the entrance of the building");
		Room elevator = new Room("floor one in the elevator");
		Room counter = new Room("in the counter of the building");
		Room garage = new Room("in the garage of the building");
		Room electrecal = new Room("in the electrecal room of the building");

        Room elevator_2 = new Room("floor two in the elevator");
		Room entrance_2 = new Room("floor two in the entrance of the building");
		Room car_wash = new Room("in the car wash of the building");
		Room exhibition = new Room("in the exhibition of the building");
		Room maintenance = new Room("in the maintenance of the building");

        Room elevator_3 = new Room("you are on the roof");
		Room helicopter = new Room("in the helicopter");


        //floor one
        elevator.AddExit("east", entrance);
		entrance.AddExit("west", elevator);

        entrance.AddExit("east", counter);
		counter.AddExit("west", entrance);

        entrance.AddExit("north", garage);
		garage.AddExit("south", entrance);

        entrance.AddExit("south", electrecal);
        electrecal.AddExit("north", entrance);

        //floor two
        elevator.AddExit("up", elevator_2);
        elevator_2.AddExit("down", elevator);

        elevator_2.AddExit("east", entrance_2);
		entrance_2.AddExit("west", elevator_2);

        entrance_2.AddExit("north", car_wash);
        car_wash.AddExit("south", entrance_2);

        entrance_2.AddExit("south", exhibition);
        exhibition.AddExit("north", entrance_2);

        entrance_2.AddExit("east", maintenance);
        maintenance.AddExit("west", entrance_2);


        elevator_2.AddExit("up", elevator_3);
        elevator_3.AddExit("down", elevator_2);

        elevator_3.AddExit("east", helicopter);

		// Create your Items here
		Item healer = new Item(3, "A healer gives you the heal that's you losen when you are visiting the rooms", "healer");
        Item key = new Item(1, "A key to open the helicopter door", "key");
        // And add them to the Rooms
        elevator.AddItem(healer);
		elevator_3.AddItem(key);

		// Start game outside

		KeyRoom = elevator_3;
        player.CurrentRoom = entrance;
		winnigRoom = helicopter;
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
				return;
			}

			if (player.CurrentRoom == winnigRoom)
			{
                Console.WriteLine("Congratulations! You have reached the office and won the game!");
				//Environment.Exit(0); // End the game
				finished = true;
				continue;
			}

            stopwatch.Start();
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
		

            if (stopwatch.ElapsedMilliseconds >= 10000)
        {
            player.Damage(10);
            Console.WriteLine("You have taken 10 damage for staying in the same room for too long.");

		}
		Console.WriteLine(stopwatch.ElapsedMilliseconds /10 );
           stopwatch.Restart(); // Restart the stopwatch


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

        if (nextRoom == winnigRoom && !player.HasItem("key"))
        {
            Console.WriteLine("You need a key to enter the helicopter!");
            player.CurrentRoom = KeyRoom;
            return;
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