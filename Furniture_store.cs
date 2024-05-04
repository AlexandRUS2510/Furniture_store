using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
class FurnitureStore
{
    static Dictionary<string, (int quantity, decimal price)> warehouse = new Dictionary<string, (int, decimal)>();
    static async Task Main()
    {
        InitializeWarehouse();
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        List<Task> tasks = new List<Task>();
        for (int i = 0; i < 100; i++)
        {
            tasks.Add(Task.Run(() => PlaceOrder("стол", 1)));
        }
        await Task.WhenAll(tasks);
        stopwatch.Stop();
        Console.WriteLine($"Время выполнения: {stopwatch.ElapsedMilliseconds} миллисекунд");
        Console.WriteLine("Все потоки завершили выполнение.");
    }
    static void InitializeWarehouse()
    {
        warehouse.Add("стол", (9999, 15000m));
        warehouse.Add("шкаф", (3, 20000m));
        warehouse.Add("кровать", (2, 30000m));
    }
    static readonly object locker = new object();
    static async Task PlaceOrder(string itemName, int quantity)
    {
        await Task.Run(() => TestBranchesAndOperators(itemName, quantity));
    }
    static void TestBranchesAndOperators(string itemName, int quantity)
    {
        lock (locker)
        {
            if (warehouse.ContainsKey(itemName))
            {
                int availableQuantity = warehouse[itemName].quantity;
                if (availableQuantity >= 1)
                {
                    decimal itemPrice = warehouse[itemName].price;
                    decimal totalPrice = itemPrice * quantity;
                    if (availableQuantity >= quantity && quantity > 0)
                    {
                        warehouse[itemName] = (availableQuantity - 1, itemPrice);
                        Console.WriteLine($"Заказ: {quantity} x {itemName} - Цена за единицу: {itemPrice} - Общая стоимость: {totalPrice}");
                        Console.WriteLine("Заказ успешно оформлен. Спасибо за покупку!");
                    }
                    else
                    {
                        Console.WriteLine("Ошибка при оформлении заказа. Недостаточно товара на складе.");
                    }
                }
                else
                {
                    Console.WriteLine($"Извините, товара '{itemName}' не найдено на складе.");
                }
            }
        }
    }
}