namespace Temporalio.MoneyTransferProject.MoneyTransferTest;
using NUnit.Framework;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Temporalio.Testing;
using Temporalio.MoneyTransferProject.MoneyTransferWorker;


[TestFixture]
public class MoneyTransferTest
{
    [Test]
    public async Task TestWithdrawActivity()
    {
        var details = new PaymentDetails(SourceAccount: "85-150", TargetAccount: "43-812", Amount: 400, ReferenceId: "12345");
        var output = new StringWriter();
        Console.SetOut(output);

        await BankingActivities.WithdrawAsync(details);

        const string withdrawalPattern = @"Withdrawing \$(\d+) from account (\d+-\d+)";
        var match = Regex.Match(output.ToString(), withdrawalPattern);

        Assert.That(match.Success, Is.True, "Failed to parse withdrawal output.");

        var amount = int.Parse(match.Groups[1].Value);
        var accountNumber = match.Groups[2].Value;

        Console.WriteLine("Withdrawal succeeded.");
    }

    [Test]
    public async Task TestDepositActivity()
    {
        var details = new PaymentDetails(SourceAccount: "85-150", TargetAccount: "43-812", Amount: 400, ReferenceId: "12345");
        var output = new StringWriter();
        Console.SetOut(output);

        await BankingActivities.DepositAsync(details);

        const string depositPattern = @"Depositing \$(\d+) into account (\d+-\d+)";
        var match = Regex.Match(output.ToString(), depositPattern);

        Assert.That(match.Success, Is.True, "Failed to parse deposit output.");

        var amount = int.Parse(match.Groups[1].Value);
        var accountNumber = match.Groups[2].Value;

        Console.WriteLine("Deposit succeeded.");
    }
}
