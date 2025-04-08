namespace Temporalio.MoneyTransferProject.MoneyTransferWorker;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Temporalio.MoneyTransferProject.BankingService.Exceptions;

public class Bank
{
    private readonly Collection<Account> accounts;

    public Bank(Collection<Account> accounts)
    {
        this.accounts = accounts ?? new Collection<Account>(new List<Account>());
    }

    public Account FindAccount(string accountNumber)
    {
        var account = accounts.FirstOrDefault(acc => acc.AccountNumber == accountNumber) ?? throw new InvalidAccountException($"The account number {accountNumber} is invalid.");
        return account;
    }
}
