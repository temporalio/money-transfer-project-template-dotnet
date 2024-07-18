namespace Temporalio.MoneyTransferProject.MoneyTransferWorker;

using System.Collections.ObjectModel;
using Temporalio.Exceptions;
using Temporalio.MoneyTransferProject.BankingService.Exceptions;
public class BankingService
{
    private readonly Bank bank;

    public BankingService(string hostname)
    {
        bank = new Bank(new Collection<Account>(new List<Account>
        {
            new Account("85-150", 2000),
            new Account("43-812", 0),
        }));
    }

    public async Task<string> WithdrawAsync(string accountNumber, int amount, string referenceId)
    {
        var account = bank.FindAccount(accountNumber);
        if (amount > account.Balance)
        {
            throw new InsufficientFundsException($"The account {accountNumber} has insufficient funds to complete this transaction.");
        }

        account.Balance -= amount;
        await Task.Delay(100); // Simulate processing delay
        return GenerateTransactionId("W");
    }

    public async Task<string> DepositAsync(string accountNumber, int amount, string referenceId)
    {
        var account = bank.FindAccount(accountNumber);
        account.Balance += amount;
        await Task.Delay(100);
        return GenerateTransactionId("D");
    }

    public async Task<string> DepositThatFailsAsync(string accountNumber, int amount, string referenceId)
    {
        await Task.Delay(100);
        throw new ApplicationFailureException("This deposit has failed.");
    }

    public async Task<string> RefundAsync(string sourceAccount, int amount, string referenceId)
    {
        await Task.Delay(100);
        return GenerateTransactionId("R");
    }

    private static string GenerateTransactionId(string prefix) => $"{prefix}-{Guid.NewGuid()}";
}
