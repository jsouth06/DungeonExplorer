using System;
using DungeonExplorer;

namespace DungeonExplorer
{
    public class Room
    {
        private string description;
        public Item RoomItem { get; set; }

        public Monster Monster { get; set; }

        public Room(string description, Item item, Monster monster)
        {
            this.description = description;
            this.RoomItem = item;
            this.Monster = monster;
        }


        public string GetDescription()
        {
            return description;
        }
    }
    public class Monster

    {
        public string Name { get; private set; }
        public int Health { get; private set; }
        public int Damage { get; private set; }
        public Monster(string name, int health, int damage)

        {
            Name = name;
            Health = health;
            Damage = damage;
        }
        public void TakeDamage(int damage)
        {
            Health -= damage;
        }
        public void Attack(Player player)
        {
            if (player == null) return;

            Console.WriteLine($"{Name} attacks {player.Name} for {Damage} damage.");
            player.TakeDamage(Damage);
        }
    }
        public abstract class Item
        {
            public string ItemName { get; private set; }

            protected Item(string itemName)
            {
                ItemName = itemName;
            }

            public override string ToString()
            {
                return ItemName;
            }
        }
    public class HealthPotion : Item
    {
        public int HealAmount { get; private set; }

        public HealthPotion(string itemName, int healAmount) : base(itemName)
        {
            this.HealAmount = healAmount;
        }
    }
    public class Weapon : Item
    {
        public int Damage { get; private set; }

        public Weapon(string itemName, int damage) : base(itemName)
        {
            Damage = damage;
        }

        
        
            
     
            
   
            
       
            
        
    }
}