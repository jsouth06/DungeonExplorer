using DungeonExplorer;
using System.Collections.Generic;
using System.Linq;

public class Player
{
    public string Name { get; private set; }
    public int Health { get; private set; }

    private List<Item> inventory = new List<Item>();
    public List<Item> Inventory => inventory;
    public int GetWeaponDamage()
    {
        var weapon = inventory.OfType<Weapon>().FirstOrDefault();
        return weapon != null ? weapon.Damage : 5; 
    }



    public Player(string name, int health)
    {
        Name = name;
        Health = health;
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
    }

    public void Heal(int amount)
    {
        Health += amount;
    }

    public void PickUpItem(Item item)
    {
        if (item != null)
        {
            if (item is Weapon)
            {
                inventory.RemoveAll(i => i is Weapon);
            }
            inventory.Add(item);
        }
    }

    public string InventoryContents()
    {
        return inventory.Count == 0 ? "Empty" : string.Join(", ", inventory);
    }
}
