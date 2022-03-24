using System;
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {
        //Выведите платёжные ссылки для трёх разных систем платежа: 
        //pay.system1.ru/order?amount=12000RUB&hash={MD5 хеш ID заказа}
        //order.system2.ru/pay?hash={MD5 хеш ID заказа + сумма заказа}
        //system3.com/pay?amount=12000&curency=RUB&hash={SHA-1 хеш сумма заказа + ID заказа + секретный ключ от системы}

        var chain = PaymentSystemsChain.Create(new PaymentSystem1(), new PaymentSystem2(), new PaymentSystem3());

        Order order = new Order(1, 12000);

        foreach (var paymentSystem in chain.PaymentSystems)
        {
            Console.WriteLine(paymentSystem.GetPayingLink(order));
        }
    }
}

class Order
{
    public readonly int Id;
    public readonly int Amount;

    public Order(int id, int amount)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(nameof(id));

        if (amount < 0)
            throw new ArgumentOutOfRangeException(nameof(amount));

        Id = id;
        Amount = amount;
    }
}

interface IPaymentSystem
{
    public string GetPayingLink(Order order);
}

abstract class PaymentSystem : IPaymentSystem
{
    protected string Hash;

    public abstract string GetPayingLink(Order order);
}

class PaymentSystem1 : PaymentSystem
{
    public PaymentSystem1() => Hash = "MD5";

    public override string GetPayingLink(Order order)
    {
        if (order == null)
            throw new NullReferenceException();

        return $"pay.system1.ru/order?amount=12000RUB&hash={{{Hash}+{order.Id}}}";
    }
}

class PaymentSystem2 : PaymentSystem
{
    public PaymentSystem2() => Hash = "MD5";

    public override string GetPayingLink(Order order)
    {
        if (order == null)
            throw new NullReferenceException();

        return $"order.system2.ru/pay?hash={{{Hash}+{order.Id}+{order.Amount}}}";
    }
}

class PaymentSystem3 : PaymentSystem
{
    private readonly string _secretKey;
    
    public PaymentSystem3()
    {
        Hash = "SHA-1";
        _secretKey = GenerateKey();
    }

    public override string GetPayingLink(Order order)
    {
        if (order == null)
            throw new NullReferenceException();

        return $"system3.com/pay?amount=12000&curency=RUB&hash={{{Hash}+{order.Amount}+{order.Id}+{_secretKey}}}";
    }

    private string GenerateKey()
    {
        Random random = new Random();
        return random.Next(int.MaxValue).ToString();
    }
}

class PaymentSystemsChain
{
    private List<IPaymentSystem> _paymentSystems = new List<IPaymentSystem>();

    public IReadOnlyList<IPaymentSystem> PaymentSystems => _paymentSystems;

    public PaymentSystemsChain(params IPaymentSystem[] paymentSystems)
    {
        foreach (var system in paymentSystems)
        {
            _paymentSystems.Add(system);
        }
    }

    public static PaymentSystemsChain Create(params IPaymentSystem[] paymentSystems)
    {
        return new PaymentSystemsChain(paymentSystems);
    }
}

