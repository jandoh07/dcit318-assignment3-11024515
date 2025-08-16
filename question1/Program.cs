using System;
using System.Collections.Generic;
public record Transaction(int Id, DateTime Date, decimal Amount, string Category);

public interface ITransactionProcessor
{
    void Process(Transaction transaction);
}

public class BankTransferProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"[Bank Transfer] Processed {transaction.Amount:C} for {transaction.Category}");
    }
}

public class MobileMoneyProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"[Mobile Money] Processed {transaction.Amount:C} for {transaction.Category}");
    }
}

public class CryptoWalletProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"[Crypto Wallet] Processed {transaction.Amount:C} for {transaction.Category}");
    }
}
public class Account
{
    public string AccountNumber { get; }
    public decimal Balance { get; protected set; }

    public Account(string accountNumber, decimal initialBalance)
    {
        AccountNumber = accountNumber;
        Balance = initialBalance;
    }

    public virtual void ApplyTransaction(Transaction transaction)
    {
        Balance -= transaction.Amount;
        Console.WriteLine($"Transaction applied: {transaction.Amount:C} | New Balance: {Balance:C}");
    }
}

public sealed class SavingsAccount : Account
{
    public SavingsAccount(string accountNumber, decimal initialBalance)
        : base(accountNumber, initialBalance)
    {
    }

    public override void ApplyTransaction(Transaction transaction)
    {
        if (transaction.Amount > Balance)
        {
            Console.WriteLine("Insufficient funds");
        }
        else
        {
            Balance -= transaction.Amount;
            Console.WriteLine($"Transaction applied: {transaction.Amount:C} | Updated Balance: {Balance:C}");
        }
    }
}

public class FinanceApp
{
    private readonly List<Transaction> _transactions = new();

    public void Run()
    {
        var account = new SavingsAccount("SA-12345", 1000m);

        var transaction1 = new Transaction(1, DateTime.Now, 150m, "Groceries");
        var transaction2 = new Transaction(2, DateTime.Now, 200m, "Utilities");
        var transaction3 = new Transaction(3, DateTime.Now, 50m, "Entertainment");

        ITransactionProcessor mobileMoneyProcessor = new MobileMoneyProcessor();
        ITransactionProcessor bankTransferProcessor = new BankTransferProcessor();
        ITransactionProcessor cryptoWalletProcessor = new CryptoWalletProcessor();

        mobileMoneyProcessor.Process(transaction1);
        bankTransferProcessor.Process(transaction2);
        cryptoWalletProcessor.Process(transaction3);

        account.ApplyTransaction(transaction1);
        account.ApplyTransaction(transaction2);
        account.ApplyTransaction(transaction3);

        _transactions.Add(transaction1);
        _transactions.Add(transaction2);
        _transactions.Add(transaction3);

        Console.WriteLine("\nAll Transactions Recorded:");
        foreach (var tx in _transactions)
        {
            Console.WriteLine($"{tx.Id}: {tx.Category} - {tx.Amount:C} on {tx.Date}");
        }
    }
}

class Program
{
    static void Main()
    {
        var app = new FinanceApp();
        app.Run();
    }
}
