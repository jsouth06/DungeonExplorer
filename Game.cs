using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.Linq.Expressions;
using System.Media;
using System.Runtime.Remoting.Messaging;
using DungeonExplorer;
using static DungeonExplorer.Weapon;

namespace DungeonExplorer
{
    internal class Game
    {
        private Player player;
        private Room currentRoom;
        private List<Room> rooms;
        private string input;
        private List<Item> items;
        private bool playing;
        private List<Monster> monsters;

      // Allows for the random function to be used in my code for random selection of items
        private Random random;

        public Game()
        {
            Console.WriteLine("Please enter player name: ");
            string name = Console.ReadLine();
            player = new Player(name, 100);      // Logs player name and the health value
            random = new Random();
            items = new List<Item>();
            rooms = new List<Room>();
            monsters = new List<Monster>();

            //Adding rooms with an added parameter of random selection from the list of items
            {
                items.Add(new Weapon("Sword", 35));
                items.Add(new Weapon("Axe", 30));
                items.Add(new Weapon("Bow with regular arrows", 30));
                items.Add(new Weapon("Spear", 40));
                items.Add(new Weapon("Dagger", 10));
                items.Add(new Weapon("Crossbow with regular arrows", 50));
                items.Add(new Weapon("Bow with fire arrows", 65));
                items.Add(new Weapon("Crossbow with explosive arrows", 100));
                items.Add(new HealthPotion("Health Boost", 50));
            }
            {
                monsters.Add(new Monster("Minotaur", 120, 40));
                monsters.Add(new Monster("Orc", 100, 20));
                monsters.Add(new Monster("Troll", 150, 30));
                monsters.Add(new Monster("Dragon", 200, 45));
                monsters.Add(new Monster("Giant", 110, 30));
            }
            {
                rooms.Add(new Room("You are in the game room.", items[random.Next(items.Count)], monsters[random.Next(monsters.Count)]));
                rooms.Add(new Room("You are in a ritual chamber.", items[random.Next(items.Count)], monsters[random.Next(monsters.Count)]));
                rooms.Add(new Room("You are in a prison cell.", items[random.Next(items.Count)], monsters[random.Next(monsters.Count)]));
                rooms.Add(new Room("You are in the armoury.", items[random.Next(items.Count)], monsters[random.Next(monsters.Count)]));
                rooms.Add(new Room("You are in the Throne room.", items[random.Next(items.Count)], monsters[random.Next(monsters.Count)]));

              

            }



        }
        public void Start()
        {
            bool playing = true;
            while (playing)
                // Using the try catch block to catch any errors that may occur during my game
                try
                {
                    Console.WriteLine("Shortly, you will be given 5 optional rooms to explore, but there may be monsters, so beware!");
                    Console.WriteLine("Would you like to view the statistics of each monster before you begin? (yes|no)");
                    string input = Console.ReadLine();
                    if (input.ToLower() == "yes")
                    {
                        foreach (var monster in monsters)
                        {
                            Console.WriteLine($"Monster: {monster.Name}, Health: {monster.Health}, Damage: {monster.Damage}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("You have chosen not to view the statistics of the monsters.");
                    }
                    Console.WriteLine($"Player 1: {player.Name}");

                    Console.WriteLine($"Health Points: {player.Health}");
                    Console.WriteLine("Do you want to continue playing? (Yes|No)");
                    string continueinput = Console.ReadLine();
                    if (continueinput.ToLower() == "no")
                    {
                        playing = false;
                        Console.WriteLine("Game ended.");
                    }
                    if (continueinput.ToLower() == "yes")
                    {
                        Console.WriteLine("Which room do you want to go to? (1-5)");
                        string input2 = Console.ReadLine();

                        //Using the if else statement to allow the player to choose which room to go to

                        if (input2 == "1")
                        {
                            currentRoom = rooms[0];
                            Console.WriteLine("Would you like to sneak into the room opposite? (yes|no)");
                            string sneakInput = Console.ReadLine();
                            if (sneakInput.ToLower() == "yes")
                            {
                                currentRoom = rooms[1];
                                Console.WriteLine($"You have successfully snuck into the room");
                                Console.WriteLine($"Current Room: {currentRoom.GetDescription()}");
                            }
                            else
                            {
                                Console.WriteLine("You did not sneak into the room.");
                            }
                            currentRoom.RoomItem = items[random.Next(items.Count)];

                            Console.WriteLine($"There is a {currentRoom.RoomItem} in the room.");
                            Console.WriteLine("Do you want to pick up this item? (yes|no)");
                            string input3 = Console.ReadLine();

                            if (input3.ToLower() == "yes")
                            {
                                player.PickUpItem(currentRoom.RoomItem);
                                Console.WriteLine($"You picked up the {currentRoom.RoomItem}.");
                                if (currentRoom.RoomItem is HealthPotion)
                                {
                                    player.Heal(50);
                                    Console.WriteLine($"You used the {currentRoom.RoomItem} and healed 50 health points.");
                                }
                                else
                                {
                                    Console.WriteLine($"You picked up the {currentRoom.RoomItem}.");
                                }

                            }
                            else
                            {
                                Console.WriteLine("You did not pick up the item.");
                            }

                            if (currentRoom.Monster != null)
                            {
                                Console.WriteLine($"A wild {currentRoom.Monster.Name} appears!");

                                currentRoom.Monster.Attack(player);
                                if (player.Health <= 0)
                                {
                                    Console.WriteLine("You have been defeated. Game Over.");
                                    playing = false;
                                    return;
                                }

                                Console.WriteLine($"Your health: {player.Health}");
                                Console.WriteLine("Do you want to fight back or flee? (fight|flee)");
                                string action = Console.ReadLine().ToLower();

                                if (action == "fight")
                                {
                                    while (currentRoom.Monster != null && player.Health > 0)
                                    {
                                        int playerDamage = player.GetWeaponDamage();
                                        Console.WriteLine($"{player.Name} attacks {currentRoom.Monster.Name} for {playerDamage} damage.");
                                        currentRoom.Monster.TakeDamage(playerDamage);

                                        if (currentRoom.Monster.Health <= 0)
                                        {
                                            Console.WriteLine($"You defeated the {currentRoom.Monster.Name}!");
                                            currentRoom.Monster = null;
                                            break;
                                        }

                                        currentRoom.Monster.Attack(player);
                                        if (player.Health <= 0)
                                        {
                                            Console.WriteLine("You have been defeated. Game Over.");
                                            playing = false;
                                            return;
                                        }

                                        Console.WriteLine($"Your health: {player.Health}");
                                        Console.WriteLine($"Monster health: {currentRoom.Monster.Health}");
                                        Console.WriteLine("Attack again? (yes|no)");
                                        string continueFight = Console.ReadLine();
                                        if (continueFight.ToLower() != "yes")
                                        {
                                            Console.WriteLine("You fled the fight. The monster remains.");
                                            break;
                                        }
                                    }
                                }
                                else if (action == "flee")
                                {
                                    Console.WriteLine("You fled the fight. The monster remains in the room.");
                                }
                                else
                                {
                                    Console.WriteLine("Invalid choice. Player has hesitated and the monster remains!");
                                }
                            }


                            else
                            {
                                Console.WriteLine("You did not pick up the item.");
                            }
                            Console.WriteLine($"Current Room: {currentRoom.GetDescription()}");
                            Console.WriteLine($"Inventory: {player.InventoryContents()}");


                        }
                        if (input2 == "2")
                        {
                            currentRoom = rooms[1];
                            currentRoom.RoomItem = items[random.Next(items.Count)];
                            Console.WriteLine($"There is a {currentRoom.RoomItem} in the room.");
                            Console.WriteLine("Do you want to pick up this item yes|no?");
                            string input3 = Console.ReadLine();
                            if (input3.ToLower() == "yes")
                            {

                                // Using and calling the PickUpItem method to add the item to the player's inventory

                                player.PickUpItem(currentRoom.RoomItem);
                                Console.WriteLine($"You picked up the {currentRoom.RoomItem}.");
                                if (currentRoom.RoomItem is HealthPotion)
                                {
                                    player.Heal(50);
                                    Console.WriteLine($"You used the {currentRoom.RoomItem} and healed 50 health points.");
                                }
                                else
                                {
                                    Console.WriteLine($"You picked up the {currentRoom.RoomItem}.");
                                }
                                Console.WriteLine($"There is a {currentRoom.Monster.Name} in the room.");

                                if (currentRoom.Monster != null)
                                {
                                    currentRoom.Monster.Attack(player);

                                    if (player.Health <= 0)
                                    {
                                        Console.WriteLine("You have been defeated by the monster. Game Over.");
                                        playing = false;
                                        return;
                                    }

                                    Console.WriteLine($"Your health: {player.Health}");
                                    Console.WriteLine("Do you want to fight back or flee? (fight|flee)");
                                    string action = Console.ReadLine().ToLower();

                                    if (action == "fight")
                                    {
                                        while (currentRoom.Monster != null && player.Health > 0)
                                        {
                                            int playerDamage = player.GetWeaponDamage();
                                            Console.WriteLine($"{player.Name} attacks {currentRoom.Monster.Name} for {playerDamage} damage.");
                                            currentRoom.Monster.TakeDamage(playerDamage);

                                            if (currentRoom.Monster.Health <= 0)
                                            {
                                                Console.WriteLine($"You defeated the {currentRoom.Monster.Name}!");
                                                currentRoom.Monster = null;
                                                break;
                                            }

                                            currentRoom.Monster.Attack(player);
                                            if (player.Health <= 0)
                                            {
                                                Console.WriteLine("You have been defeated. Game Over.");
                                                playing = false;
                                                return;
                                            }

                                            Console.WriteLine($"Your health: {player.Health}");
                                            Console.WriteLine($"Monster health: {currentRoom.Monster.Health}");
                                            Console.WriteLine("Attack again? (yes|no)");
                                            string continueFight = Console.ReadLine();
                                            if (continueFight.ToLower() != "yes")
                                            {
                                                Console.WriteLine("You fled the fight. The monster remains.");
                                                break;
                                            }
                                        }
                                    }
                                    else if (action == "flee")
                                    {
                                        Console.WriteLine("You fled the fight. The monster remains in the room.");
                                    }
                                    else
                                    {
                                        Console.WriteLine("Invalid choice. Player has hesitated and the monster remains!");
                                    }
                                }


                                else
                                {
                                    Console.WriteLine("You did not pick up the item.");
                                }
                                Console.WriteLine($"Current Room: {currentRoom.GetDescription()}");
                                Console.WriteLine($"Inventory: {player.InventoryContents()}");

                            }
                        }

                        if (input2 == "3")
                        {
                            currentRoom = rooms[2];
                            currentRoom.RoomItem = items[random.Next(items.Count)];
                            Console.WriteLine($"There is a {currentRoom.RoomItem} in the room.");
                            Console.WriteLine("Do you want to pick up this item yes|no?");
                            string input3 = Console.ReadLine();
                            if (input3.ToLower() == "yes")
                            {
                                player.PickUpItem(currentRoom.RoomItem);
                                Console.WriteLine($"You picked up the {currentRoom.RoomItem}.");
                                if (currentRoom.RoomItem is HealthPotion)
                                {
                                    player.Heal(50);
                                    Console.WriteLine($"You used the {currentRoom.RoomItem} and healed 50 health points.");
                                }
                                else
                                {
                                    Console.WriteLine($"You picked up the {currentRoom.RoomItem}.");
                                }
                                Console.WriteLine($"There is a {currentRoom.Monster.Name} in the room.");

                                if (currentRoom.Monster != null)
                                {
                                    currentRoom.Monster.Attack(player);

                                    if (player.Health <= 0)
                                    {
                                        Console.WriteLine("You have been defeated by the monster. Game Over.");
                                        playing = false;
                                        return;
                                    }
                                    Console.WriteLine($"Your health: {player.Health}");
                                    Console.WriteLine("Do you want to fight back or flee? (fight|flee)");
                                    string action = Console.ReadLine().ToLower();

                                    if (action == "fight")
                                    {
                                        while (currentRoom.Monster != null && player.Health > 0)
                                        {
                                            int playerDamage = player.GetWeaponDamage();
                                            Console.WriteLine($"{player.Name} attacks {currentRoom.Monster.Name} for {playerDamage} damage.");
                                            currentRoom.Monster.TakeDamage(playerDamage);

                                            if (currentRoom.Monster.Health <= 0)
                                            {
                                                Console.WriteLine($"You defeated the {currentRoom.Monster.Name}!");
                                                currentRoom.Monster = null;
                                                break;
                                            }

                                            currentRoom.Monster.Attack(player);
                                            if (player.Health <= 0)
                                            {
                                                Console.WriteLine("You have been defeated. Game Over.");
                                                playing = false;
                                                return;
                                            }

                                            Console.WriteLine($"Your health: {player.Health}");
                                            Console.WriteLine($"Monster health: {currentRoom.Monster.Health}");
                                            Console.WriteLine("Attack again? (yes|no)");
                                            string continueFight = Console.ReadLine();
                                            if (continueFight.ToLower() != "yes")
                                            {
                                                Console.WriteLine("You fled the fight. The monster remains.");
                                                break;
                                            }
                                        }
                                    }
                                    else if (action == "flee")
                                    {
                                        Console.WriteLine("You fled the fight. The monster remains in the room.");
                                    }
                                    else
                                    {
                                        Console.WriteLine("Invalid choice. Player has hesitated and the monster remains!");
                                    }
                                }


                                else
                                {
                                    Console.WriteLine("You did not pick up the item.");
                                }
                                Console.WriteLine($"Current Room: {currentRoom.GetDescription()}");
                                Console.WriteLine($"Inventory: {player.InventoryContents()}");

                            }
                        }

                        if (input2 == "4")
                        {
                            currentRoom = rooms[3];
                            currentRoom.RoomItem = items[random.Next(items.Count)];
                            Console.WriteLine($"There is a {currentRoom.RoomItem} in the room.");

                            Console.WriteLine("Do you want to pick up the item yes|no?");

                            string input3 = Console.ReadLine();
                            if (input3.ToLower() == "yes")
                            {
                                player.PickUpItem(currentRoom.RoomItem);
                                Console.WriteLine($"You picked up the {currentRoom.RoomItem}.");
                                if (currentRoom.RoomItem is HealthPotion)
                                {
                                    player.Heal(50);
                                    Console.WriteLine($"You used the {currentRoom.RoomItem} and healed 50 health points.");
                                }
                                else
                                {
                                    Console.WriteLine($"You picked up the {currentRoom.RoomItem}.");
                                }
                                Console.WriteLine($"There is a {currentRoom.Monster.Name} in the room.");

                                if (currentRoom.Monster != null)
                                {
                                    currentRoom.Monster.Attack(player);

                                    if (player.Health <= 0)
                                    {
                                        Console.WriteLine("You have been defeated by the monster. Game Over.");
                                        playing = false;
                                        return;
                                    }
                                    Console.WriteLine($"Your health: {player.Health}");
                                    Console.WriteLine("Do you want to fight back or flee? (fight|flee)");
                                    string action = Console.ReadLine().ToLower();

                                    if (action == "fight")
                                    {
                                        while (currentRoom.Monster != null && player.Health > 0)
                                        {
                                            int playerDamage = player.GetWeaponDamage();
                                            Console.WriteLine($"{player.Name} attacks {currentRoom.Monster.Name} for {playerDamage} damage.");
                                            currentRoom.Monster.TakeDamage(playerDamage);

                                            if (currentRoom.Monster.Health <= 0)
                                            {
                                                Console.WriteLine($"You defeated the {currentRoom.Monster.Name}!");
                                                currentRoom.Monster = null;
                                                break;
                                            }

                                            currentRoom.Monster.Attack(player);
                                            if (player.Health <= 0)
                                            {
                                                Console.WriteLine("You have been defeated. Game Over.");
                                                playing = false;
                                                return;
                                            }

                                            Console.WriteLine($"Your health: {player.Health}");
                                            Console.WriteLine($"Monster health: {currentRoom.Monster.Health}");
                                            Console.WriteLine("Attack again? (yes|no)");
                                            string continueFight = Console.ReadLine();
                                            if (continueFight.ToLower() != "yes")
                                            {
                                                Console.WriteLine("You fled the fight. The monster remains.");
                                                break;
                                            }
                                        }
                                    }
                                    else if (action == "flee")
                                    {
                                        Console.WriteLine("You fled the fight. The monster remains in the room.");
                                    }
                                    else
                                    {
                                        Console.WriteLine("Invalid choice. Player has hesitated and the monster remains!");
                                    }
                                }


                                else
                                {
                                    Console.WriteLine("You did not pick up the item.");
                                }
                                Console.WriteLine($"Current Room: {currentRoom.GetDescription()}");
                                Console.WriteLine($"Inventory: {player.InventoryContents()}");

                            }
                        }



                        if (input2 == "5")
                        {
                            currentRoom = rooms[4];
                            currentRoom.RoomItem = items[random.Next(items.Count)];
                            Console.WriteLine($"There is a {currentRoom.RoomItem} in the room.");
                            Console.WriteLine("Do you want to pick up this item yes|no?");
                            string input3 = Console.ReadLine();
                            if (input3.ToLower() == "yes")
                            {
                                player.PickUpItem(currentRoom.RoomItem);
                                Console.WriteLine($"You picked up the {currentRoom.RoomItem}.");
                                if (currentRoom.RoomItem is HealthPotion)
                                {
                                    player.Heal(50);
                                    Console.WriteLine($"You used the {currentRoom.RoomItem} and healed 50 health points.");
                                }
                                else
                                {
                                    Console.WriteLine($"You picked up the {currentRoom.RoomItem}.");
                                }
                                Console.WriteLine($"There is a {currentRoom.Monster.Name} in the room.");

                                if (currentRoom.Monster != null)
                                {
                                    currentRoom.Monster.Attack(player);

                                    if (player.Health <= 0)
                                    {
                                        Console.WriteLine("You have been defeated by the monster. Game Over.");
                                        playing = false;
                                        return;
                                    }
                                    Console.WriteLine($"Your health: {player.Health}");
                                    Console.WriteLine("Do you want to fight back or flee? (fight|flee)");
                                    string action = Console.ReadLine().ToLower();

                                    if (action == "fight")
                                    {
                                        while (currentRoom.Monster != null && player.Health > 0)
                                        {
                                            int playerDamage = player.GetWeaponDamage();
                                            Console.WriteLine($"{player.Name} attacks {currentRoom.Monster.Name} for {playerDamage} damage.");
                                            currentRoom.Monster.TakeDamage(playerDamage);

                                            if (currentRoom.Monster.Health <= 0)
                                            {
                                                Console.WriteLine($"You defeated the {currentRoom.Monster.Name}!");
                                                currentRoom.Monster = null;
                                                break;
                                            }

                                            currentRoom.Monster.Attack(player);
                                            if (player.Health <= 0)
                                            {
                                                Console.WriteLine("You have been defeated. Game Over.");
                                                playing = false;
                                                return;
                                            }

                                            Console.WriteLine($"Your health: {player.Health}");
                                            Console.WriteLine($"Monster health: {currentRoom.Monster.Health}");
                                            Console.WriteLine("Attack again? (yes|no)");
                                            string continueFight = Console.ReadLine();
                                            if (continueFight.ToLower() != "yes")

                                            {
                                                Console.WriteLine("You fled the fight. The monster remains.");
                                                break;
                                            }
                                        }
                                    }
                                    else if (action == "flee")
                                    {
                                        Console.WriteLine("You fled the fight. The monster remains in the room.");
                                    }
                                    else
                                    {
                                        Console.WriteLine("Invalid choice. Player has hesitated and the monster remains!");
                                    }
                                }


                                else
                                {
                                    Console.WriteLine("You did not pick up the item.");
                                }
                                Console.WriteLine($"Current Room: {currentRoom.GetDescription()}");
                                Console.WriteLine($"Inventory: {player.InventoryContents()}");

                            }
                        }
                    }
                }




                catch
                {

                    Console.WriteLine("Error");

                }
        }
    }
}
                