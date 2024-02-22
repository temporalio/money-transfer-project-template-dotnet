public class Account
{
    public string AccountNumber { get; private set; }
    public int Balance { get; set; }

    public Account(string accountNumber, int balance)
    {
        AccountNumber = accountNumber;
        Balance = balance;
    }
}

public class Bank
{
    private List<Account> _accounts;

    public Bank(List<Account> accounts)
    {
        _accounts = accounts ?? new List<Account>();
    }

    public Account FindAccount(string accountNumber)
    {
        var account = _accounts.FirstOrDefault(acc => acc.AccountNumber == accountNumber);
        if (account == null)
            throw new InvalidAccountError($"The account number {accountNumber} is invalid.");
        return account;
    }
}

public class InvalidAccountError : Exception
{
    public InvalidAccountError(string message) : base(message) { }
}

public class InsufficientFundsError : Exception
{
    public InsufficientFundsError(string message) : base(message) { }
}

public class BankingService
{
    private readonly string _hostname;
    private readonly Bank _mockBank;

    public BankingService(string hostname)
    {
        _hostname = hostname;
        _mockBank = new Bank(new List<Account>
        {
            new Account("85-150", 2000),
            new Account("43-812", 0)
        });
    }

    public async Task<string> Withdraw(string accountNumber, int amount, string referenceId)
    {
        var account = _mockBank.FindAccount(accountNumber);
        if (amount > account.Balance)
            throw new InsufficientFundsError($"The account {accountNumber} has insufficient funds to complete this transaction.");

        account.Balance -= amount; 
        await Task.Delay(100); 
        return GenerateTransactionId("W");
    }

    public async Task<string> Deposit(string accountNumber, int amount, string referenceId)
    {
        var account = _mockBank.FindAccount(accountNumber);
        account.Balance += amount; 
        await Task.Delay(100); 
        return GenerateTransactionId("D");
    }

    public async Task<string> Refund(string sourceAccount, int amount, string referenceID)
    {
        // Simulate processing delay
        await Task.Delay(100);
        return GenerateTransactionId("R");
    }

    private string GenerateTransactionId(string prefix)
    {
        return $"{prefix}-{Guid.NewGuid()}";
    }
}

