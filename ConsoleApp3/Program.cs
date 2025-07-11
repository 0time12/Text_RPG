using System;
using System.Collections.Generic;
using System.Threading;


public class Item //아이템 정보 관리
{
    public string Name { get; }
    public string Description { get; }
    public int Price { get; }
    public int AttackBonus { get; }
    public int DefenseBonus { get; }
    public int HealthBonus { get; } = 0;
    public bool IsPurchased { get; set; } = false;
    public bool IsEquipped { get; set; } = false;

    public Item(string name, string description, int price, int attackBonus = 0, int defenseBonus = 0, int healthBonus = 0)
    {
        Name = name;
        Description = description;
        Price = price;
        AttackBonus = attackBonus;
        DefenseBonus = defenseBonus;
        HealthBonus = healthBonus;
    }
}

public class Character //캐릭터 기본 정보
{
    public int Level { get; private set; } = 1;
    public string Name { get; private set; }
    public string ClassType { get; private set; }
    public int BaseAttack { get; private set; } = 10;
    public int BaseDefense { get; private set; } = 5;
    public int BaseHealth { get; private set; } = 100;
    public int CurrentHealth { get; set; }
    public int Gold { get; private set; } = 1500;
    public int Damage { get; set; } = 0; 

    private List<Item> inventory = new();
    private List<Item> equippedItems = new List<Item>();

    public Character(string name, string classType)
    {
        Name = name;
        ClassType = classType;
        CurrentHealth = TotalHealth - Damage;
    }

    // 인벤토리 관련
    public IReadOnlyList<Item> Inventory
    {
        get
        {
            return inventory.AsReadOnly();
        }
    }
    // 장착 아이템 관련
    public IReadOnlyList<Item> EquippedItems
    {
        get
        {
            return equippedItems.AsReadOnly();
        }
    }
    public void AddItemToInventory(Item item)
    {
        inventory.Add(item);
    }

    // 장착 , 해제
    public void ToggleEquipItem(int inventoryIndex)
    {
        if (inventoryIndex < 0 || inventoryIndex >= inventory.Count) 
            throw new ArgumentOutOfRangeException(); //다른 번호를 고르면 예외 발생

        var item = inventory[inventoryIndex];
        if (item.IsEquipped)
        {
            item.IsEquipped = false; //장착일때 고르면 해제
            equippedItems.Remove(item);
        }
        else
        {
            item.IsEquipped = true; //해제일때 고르면 장착    
            equippedItems.Add(item);
        }
    }
    // 아이템 제거
    public bool RemoveItem(Item item)
    {
        return inventory.Remove(item);
    }

    // 보유 골드 확인 및 계산
    public bool CanAfford(int amount)
    {
        return Gold >= amount;
    }
    public bool SpendGold(int amount) //골드 지불
    {
        if (CanAfford(amount))
        {
            Gold -= amount;
            return true;
        }
        return false;
    }
    public void GetGold(int amount) //골드 획득
    {
        Gold += amount;
    }

    // 장착 아이템에 따른 스탯 계산
    // 공격
    private int SumAttackBonus()
    {
        int sum = 0;
        foreach (var item in equippedItems)
        {
            sum += item.AttackBonus;
        }
        return sum;
    }
    public int TotalAttack
    {
        get
        {
            return BaseAttack + SumAttackBonus();
        }
    }

    //방어
    private int SumDefenseBonus()
    {
        int sum = 0;
        foreach (var item in equippedItems)
        {
            sum += item.DefenseBonus;
        }
        return sum;
    }
    public int TotalDefense
    {
        get
        {
            return BaseDefense + SumDefenseBonus();
        }
    }
    //체력
    private int SumHealthBonus()
    {
        int sum = 0;
        foreach (var item in equippedItems)
        {
            sum += item.HealthBonus;
        }
        return sum;
    }
    public int TotalHealth
    {
        get
        {
            return BaseHealth + SumHealthBonus(); 
        }
    }

    public void PrintStatus()
    {
        Console.WriteLine("상태 보기\n캐릭터의 정보가 표시됩니다.\n");
        Console.WriteLine($"Lv. {Level:00}");
        Console.WriteLine($"{Name} ( {ClassType} )");
        Console.WriteLine($"공격력 : {BaseAttack}" + (TotalAttack > BaseAttack ? $" (+{TotalAttack - BaseAttack})" : ""));
        Console.WriteLine($"방어력 : {BaseDefense}" + (TotalDefense > BaseDefense ? $" (+{TotalDefense - BaseDefense})" : ""));
        Console.WriteLine($"체 력 : {TotalHealth}");
        Console.WriteLine($"Gold : {Gold} G\n");
        Console.WriteLine("0. 나가기");
        Console.Write("원하시는 행동을 입력해주세요.\n>> ");
    }

    public void PrintInventory()
    {
        Console.WriteLine("인벤토리\n보유 중인 아이템을 관리할 수 있습니다.\n");
        Console.WriteLine("[아이템 목록]");
        for (int i = 0; i < inventory.Count; i++)
        {
            var item = inventory[i];
            string equippedMark = item.IsEquipped ? "[E]" : "";

            string bonus = "";
            if (item.AttackBonus != 0)
                bonus += $"공격력 +{item.AttackBonus}";
            if (item.DefenseBonus != 0)
            {
                if (bonus != "") bonus += ", ";
                bonus += $"방어력 +{item.DefenseBonus}";
            }
            if (item.HealthBonus != 0)
            {
                if (bonus != "") bonus += ", ";
                bonus += $"체력 +{item.HealthBonus}";
            }

            Console.WriteLine($"- {equippedMark}{item.Name,-14} | {bonus,-12} | {item.Description}");
        }
        Console.WriteLine();
        Console.WriteLine("1. 장착 관리");
        Console.WriteLine("0. 나가기");
        Console.Write("원하시는 행동을 입력해주세요.\n>> ");
    }

    public void PrintInventoryWithNumbers()
    {
        Console.WriteLine("인벤토리 - 장착 관리\n보유 중인 아이템을 관리할 수 있습니다.\n");
        Console.WriteLine("[아이템 목록]");
        for (int i = 0; i < inventory.Count; i++)
        {
            var item = inventory[i];
            string equippedMark = item.IsEquipped ? "[E]" : "";
            string bonus = "";

            if (item.AttackBonus != 0)
                bonus += $"공격력 +{item.AttackBonus}";
            if (item.DefenseBonus != 0)
            {
                if (bonus != "") bonus += ", ";
                bonus += $"방어력 +{item.DefenseBonus}";
            }
            if (item.HealthBonus != 0)
            {
                if (bonus != "") bonus += ", ";
                bonus += $"체력 +{item.HealthBonus}";
            }

            Console.WriteLine($"{i + 1} {equippedMark}{item.Name,-14} | {bonus,-12} | {item.Description}");
        }
        Console.WriteLine();
        Console.WriteLine("0. 나가기");
        Console.Write("원하시는 행동을 입력해주세요.\n>> ");
    }
}

public class Shop //상점 정보
{
    private List<Item> itemsForSale;
    private Character character;

    public Shop(Character character)
    {
        this.character = character;
        itemsForSale = new List<Item>
        {
            new Item("수련자 갑옷", "수련에 도움을 주는 갑옷입니다.", 1000, 0, 5, 0),
            new Item("무쇠갑옷", "무쇠로 만들어져 튼튼한 갑옷입니다.", 1500, 0, 9, 0), 
            new Item("스파르타의 갑옷", "스파르타의 전사들이 사용했다는 전설의 갑옷입니다.", 3500, 0, 15, 0),
            new Item("낡은 검", "쉽게 볼 수 있는 낡은 검 입니다.", 600, 2, 0, 0),
            new Item("청동 도끼", "어디선가 사용됐던거 같은 도끼입니다.", 1500, 5, 0, 0),
            new Item("스파르타의 창", "스파르타의 전사들이 사용했다는 전설의 창입니다.", 3000, 7, 0, 0),
            new Item("날카로운 방패", "무기로도 사용이 가능하도록 설계된 방패입니다.", 2000, 4, 4, 0),
            new Item("음식 주머니", "음식을 넣을 수 있는 주머니 입니다.", 1000, 0, 0, 50),
        };
    }

    public void PrintShop() // 상점 출력
    {
        Console.WriteLine("상점\n필요한 아이템을 얻을 수 있는 상점입니다.\n");
        Console.WriteLine($"[보유 골드]\n{character.Gold} G\n");
        Console.WriteLine("[아이템 목록]");
        for (int i = 0; i < itemsForSale.Count; i++)
        {
            var item = itemsForSale[i];
            string priceOrBought = item.IsPurchased ? "구매완료" : $"{item.Price} G";
            Console.WriteLine($"{item.Name,-14} | " +
                              $"공격력 +{item.AttackBonus} 방어력 +{item.DefenseBonus} 체력 +{item.HealthBonus} | " +
                              $"{item.Description,-35} | {priceOrBought}");
        }
        Console.WriteLine();
        Console.WriteLine("1. 아이템 구매");
        Console.WriteLine("2. 아이템 판매");
        Console.WriteLine("0. 나가기");
        Console.Write("원하시는 행동을 입력해주세요.\n>> ");
    }

    public void HandleShopInput() //상점 선택
    {
        while (true)
        {
            Console.Clear();
            PrintShop();

            string input = Console.ReadLine();
            if (input == "0")
            {
                break;
            }
            else if(input == "1")
            {
                HandlePurchase();
            }
            else if(input == "2")
            {
                Handlesell();
            }
            else
            {
                Console.WriteLine("잘못된 입력입니다.");
               Console.ReadKey();
            }
        }
    }

    private void HandlePurchase() //아이템 구매
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("상점 - 아이템 구매\n필요한 아이템을 얻을 수 있는 상점입니다.\n");
            Console.WriteLine($"[보유 골드]\n{character.Gold} G\n");
            Console.WriteLine("[아이템 목록]");
            for (int i = 0; i < itemsForSale.Count; i++)
            {
                var item = itemsForSale[i];
                string priceOrBought = item.IsPurchased ? "구매완료" : $"{item.Price} G";
                Console.WriteLine($"{i + 1}. {item.Name,-14} | " +
                                  $"공격력 +{item.AttackBonus} 방어력 +{item.DefenseBonus} 체력 +{item.HealthBonus} | " +
                                  $"{item.Description,-35} | {priceOrBought}");
            }
            Console.WriteLine();
            Console.WriteLine("0. 나가기");
            Console.Write("원하시는 행동을 입력해주세요.\n>> ");
            string input = Console.ReadLine();

            if (input == "0")
            {
                break;
            }
            else if (int.TryParse(input, out int idx) && idx >= 1 && idx <= itemsForSale.Count)
            {
                var item = itemsForSale[idx - 1];
                if (item.IsPurchased)
                {
                    Console.WriteLine("이미 구매한 아이템입니다.");
                }
                else if (!character.CanAfford(item.Price))
                {
                    Console.WriteLine("Gold 가 부족합니다.");
                }
                else
                {
                    character.SpendGold(item.Price);
                    item.IsPurchased = true;
                    character.AddItemToInventory(item);
                    Console.WriteLine("구매를 완료했습니다.");
                }
            }
            else
            {
                Console.WriteLine("잘못된 입력입니다.");
                Console.ReadKey();
            }
        }
    }
    public void Handlesell() //아이템 판매
    {   

        while (true)
        {
            Console.Clear();
            Console.WriteLine("상점 - 아이템 판매\n필요한 아이템을 얻을 수 있는 상점입니다.\n");
            Console.WriteLine($"[보유 골드]\n{character.Gold} G\n");
            Console.WriteLine("[아이템 목록]");
            var inventory = character.Inventory;

            for (int i = 0; i < inventory.Count; i++)
            {
                var item = inventory[i];
                string equippedMark = item.IsEquipped ? "[E]" : "";
                string bonus = "";

                if (item.AttackBonus != 0)
                    bonus += $"공격력 +{item.AttackBonus}";
                if (item.DefenseBonus != 0)
                {
                    if (bonus != "") bonus += ", ";
                    bonus += $"방어력 +{item.DefenseBonus}";
                }
                if (item.HealthBonus != 0)
                {
                    if (bonus != "") bonus += ", ";
                    bonus += $"체력 +{item.HealthBonus}";
                }

                Console.WriteLine($"{i + 1} {equippedMark}{item.Name,-14} | {bonus,-12} | {item.Description}");
            }
            Console.WriteLine();
            Console.WriteLine("0. 나가기");
            Console.Write("원하시는 행동을 입력해주세요.\n>> ");
            string input = Console.ReadLine();

            if (input == "0") 
            {
                break;
            }
            else if (int.TryParse(input, out int idx) && idx >= 1 && idx <= inventory.Count)
            {
                var item = inventory[idx - 1];
                int sellprice = (int)(item.Price * 0.85f); // 85%로 판매 소수점은 버림
                if (item.IsEquipped)
                {
                    item.IsEquipped = false;
                    item.IsPurchased = false;
                }
                character.RemoveItem(item);
                character.GetGold(sellprice);
                Console.WriteLine($"{sellprice} G를 얻었습니다.");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("잘못된 입력입니다.");
                Console.ReadKey();
            }
            
        }

    }
}

public class Rest
{
    private Character character;

    public Rest(Character character)
    {
        this.character = character;
    }
    public void RestMenu()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine($"휴식하기\n500G를 내면 체력을 100 회복할 수 있습니다. (현재 체력: {character.CurrentHealth})\n");
            Console.WriteLine($"[보유 골드]\n{character.Gold} G\n");
            Console.WriteLine("1.휴식하기");
            Console.WriteLine("0.나가기");
            Console.Write("원하시는 행동을 입력해주세요.\n>> ");
            string input = Console.ReadLine();

            if (input == "0")
            {
                break;
            }
            else if(input == "1")
            {
                if (character.CanAfford(500))
                {
                    if (character.CurrentHealth == character.TotalHealth)
                    {
                        Console.WriteLine("최대체력입니다.");
                    }
                    else if (character.CurrentHealth >= character.TotalHealth - 100)
                    {
                        character.CurrentHealth = character.TotalHealth;
                        character.SpendGold(500);
                        Console.WriteLine("체력을 최대까지 회복했습니다!");
                    }
                    else
                    {
                        character.CurrentHealth += 100;
                        character.SpendGold(500);
                        Console.WriteLine("체력을 100 회복했습니다!");
                    }
                }
                else
                {
                    Console.WriteLine("Gold 가 부족합니다.");
                }
            }
            else
            {
                Console.WriteLine("잘못된 입력입니다.");
            }
            Console.ReadKey();
        }
    }
}
public class Game
{
    private Character character;
    private Shop shop;
    private Rest rest;

    public Game()
    {
        character = new Character("Chan", "전사");
        shop = new Shop(character);
        rest = new Rest(character);
    }

    public void Run()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("스파르타 마을에 오신 여러분 환영합니다.");
            Console.WriteLine("이곳에서 던전으로 들어가기전 활동을 할 수 있습니다.\n");
            Console.WriteLine("1. 상태 보기");
            Console.WriteLine("2. 인벤토리");
            Console.WriteLine("3. 상점");
            Console.WriteLine("4. 던전입장");
            Console.WriteLine("5. 휴식하기");
            Console.Write("원하시는 행동을 입력해주세요.\n>> ");

            string input = Console.ReadLine();

            if (input == "1")
            {
                ShowStatusMenu();
            }
            else if (input == "2")
            {
                ShowInventoryMenu();
            }
            else if (input == "3")
            {
                shop.HandleShopInput();
            }
            else if (input == "4")
            {
                Console.WriteLine("던전의 문이 막혀있습니다.");
            }
            else if (input == "5")
            {
                rest.RestMenu();
            }
            else
            {
                Console.WriteLine("잘못된 입력입니다.");
                Console.ReadKey();
            }
        }
    }

    private void ShowStatusMenu()
    {
        while (true)
        {
            Console.Clear();
            character.PrintStatus();

            string input = Console.ReadLine();
            if (input == "0")
                break;
        }
    }

    private void ShowInventoryMenu()
    {
        while (true)
        {
            Console.Clear();
            character.PrintInventory();

            string input = Console.ReadLine();

            if (input == "0")
                break;
            else if (input == "1")
            {
                ShowEquipManageMenu();
            }
            else
            {
                Console.WriteLine("잘못된 입력입니다.");
                Console.WriteLine("계속하려면 아무 키나 누르세요...");
                Console.ReadKey();
            }
        }
    }

    private void ShowEquipManageMenu()
    {
        while (true)
        {
            Console.Clear();
            character.PrintInventoryWithNumbers();

            string input = Console.ReadLine();

            if (input == "0")
            {
                break;
            }
            else if(int.TryParse(input, out int idx))
            {
                idx--; 
                if (idx >= 0 && idx < character.Inventory.Count)
                {
                    character.ToggleEquipItem(idx);
                }
                else
                {
                    Console.WriteLine("잘못된 입력입니다.");
                    Console.ReadKey();
                }
            }
            else
            {
                Console.WriteLine("잘못된 입력입니다.");
                Console.ReadKey();
            }
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        Game game = new Game();
        game.Run();
    }
}

