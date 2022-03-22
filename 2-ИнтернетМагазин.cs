using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        Good iPhone12 = new Good("IPhone 12");
        Good iPhone11 = new Good("IPhone 11");

        Warehouse warehouse = new Warehouse();

        Shop shop = new Shop(warehouse);

        warehouse.Deliver(iPhone12, 10);
        warehouse.Deliver(iPhone11, 1);

        warehouse.Show();   //Вывод всех товаров на складе с их остатком

        Cart cart = shop.Cart();
        cart.Add(iPhone12, 4);
        cart.Add(iPhone11, 3); //при такой ситуации возникает ошибка так, как нет нужного количества товара на складе

        cart.Show();    //Вывод всех товаров в корзине

        Console.WriteLine(cart.Order().Paylink);

        cart.Add(iPhone12, 9); //Ошибка, после заказа со склада убираются заказанные товары
    }
}

class Good
{
    public readonly string Label;

    public Good(string label)
    {
        Label = label;
    }
}

class Warehouse : IShowGoods, ICheckAvalaibility
{
    private Dictionary<Good, int> _goods = new Dictionary<Good, int>();

    public void Deliver(Good good, int count)
    {
        if (good == null)
            throw new NullReferenceException(nameof(count));

        if (count <= 0)
            throw new ArgumentOutOfRangeException(nameof(count));

        if (_goods.ContainsKey(good) == false)
            _goods.Add(good, count);
        else
            _goods[good] += count;
    }

    public void Show()
    {
        foreach (var good in _goods)
        {
            Console.WriteLine($"{good.Key.Label}, {good.Value}");
        }
    }

    public bool CheckAvailability(Good good, int countInOrder)
    {
        if (_goods.TryGetValue(good, out int countInWarehouse) == false)
            throw new ArgumentOutOfRangeException(nameof(good));

        if (countInWarehouse < countInOrder)
            throw new ArgumentOutOfRangeException(nameof(countInOrder));

        return true;
    }

    public void Ship(Good good, int count)
    {
        _goods[good] -= count;

        if (_goods[good] == 0)
            _goods.Remove(good);
    }
}

class Shop : ICheckAvalaibility
{
    public readonly Warehouse Warehouse;

    public Shop(Warehouse warehouse)
    {
        if (warehouse == null)
            throw new NullReferenceException(nameof(warehouse));

        Warehouse = warehouse;
    }

    public Cart Cart()
    {
        return new Cart(this);
    }

    public bool CheckAvailability(Good good, int countInOrder)
    {
        return Warehouse.CheckAvailability(good, countInOrder);
    }
}

class Cart : IShowGoods, IShowPaylink
{
    private Dictionary<Good, int> _goods = new Dictionary<Good, int>();
    private readonly Shop _shop;
    private readonly IShowPaylink _paylinkInterface;

    public string Paylink { get; }

    public Cart(Shop shop)
    {
        _shop = shop;
        _paylinkInterface = this;
        Paylink = "Заказ успешно обработан";
    }

    public void Add(Good good, int quantity)
    {
        if (_shop.CheckAvailability(good, quantity))
            _goods.Add(good, quantity);
    }

    public IShowPaylink Order()
    {
        foreach (var good in _goods)
        {
            _shop.Warehouse.Ship(good.Key, good.Value);
        }

        _goods.Clear();
        return _paylinkInterface;
    }

    public void Show()
    {
        foreach (var good in _goods)
        {
            Console.WriteLine($"{good.Key.Label}, {good.Value}");
        }
    }
}

interface IShowPaylink
{
    public string Paylink { get; }
}

interface IShowGoods
{
    public void Show();
}

interface ICheckAvalaibility
{
    public bool CheckAvailability(Good good, int countInOrder);
}