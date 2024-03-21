namespace Temporalio.MoneyTransferProject.Worker.BankingService;
using Temporalio.MoneyTransferProject.Worker.BankingServiceExceptions;
using Temporalio.MoneyTransferProject.Worker.BankingServiceAccount;

public class Bank(List<Account> accounts)
{
    private readonly List<Account> _accounts = accounts ?? [];

    public Account FindAccount(string accountNumber)
    {
        var account = _accounts.FirstOrDefault(acc => acc.AccountNumber == accountNumber) ?? throw new InvalidAccountError($"The account number {accountNumber} is invalid.");
        return account;
    }
}
public class BankingService
{
    private readonly Bank _bank;

    public BankingService(string hostname)
    {
        _bank = new Bank(new List<Account>
        {
            new("85-150", 2000),
            new("43-812", 0)
        });
    }

    public async Task<string> WithdrawAsync(string accountNumber, int amount, string referenceId)
    {
        var account = _bank.FindAccount(accountNumber);
        if (amount > account.Balance)
        {
            throw new InsufficientFundsError($"The account {accountNumber} has insufficient funds to complete this transaction.");
        }

        account.Balance -= amount;
        await Task.Delay(100); // Simulate processing delay
        return GenerateTransactionId("W");
    }

    public async Task<string> DepositAsync(string accountNumber, int amount, string referenceId)
    {
        var account = _bank.FindAccount(accountNumber);
        account.Balance += amount;
        await Task.Delay(100);
        return GenerateTransactionId("D");
    }
    public async Task<string> DepositThatFailsAsync(string accountNumber, int amount, string referenceId)
    {
        await Task.Delay(100);
        throw new ApplicationException("This deposit has failed.");
    }

    public async Task<string> RefundAsync(string sourceAccount, int amount, string referenceId)
    {
        await Task.Delay(100);
        return GenerateTransactionId("R");
    }

    private static string GenerateTransactionId(string prefix) => $"{prefix}-{Guid.NewGuid()}";
}
