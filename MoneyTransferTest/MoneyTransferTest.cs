using NUnit.Framework;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Temporalio.Client;
using Temporalio.Testing;
using Temporalio.Worker;
using MoneyTransferActivity;
using MoneyTransferShared;
using MoneyTransferProject;

[TestFixture]
public class MoneyTransferTests
{
    private WorkflowEnvironment? environment;
    private ITemporalClient? client;

    [OneTimeSetUp]
    public async Task Setup()
    {
        environment = await WorkflowEnvironment.StartTimeSkippingAsync();
        client = environment?.Client;
    }

    [OneTimeTearDown]
    public async Task Cleanup()
    {
        if (environment != null)
        {
            await environment.DisposeAsync();
        }
    }

    [Test]
    public async Task TestWithdrawActivity()
    {
        var details = new PaymentDetails("85-150", "43-812", 400, "12345");
        var activities = new BankingActivities();
        var output = new StringWriter();
        Console.SetOut(output);

        await activities.WithdrawAsync(details);

        const string withdrawalPattern = @"Withdrawing \$(\d+) from account (\d+-\d+)";
        var match = Regex.Match(output.ToString(), withdrawalPattern);
        if (!match.Success)
        {
            throw new InvalidOperationException("Failed to parse withdrawal output.");
        }

        var amount = int.Parse(match.Groups[1].Value);
        var accountNumber = match.Groups[2].Value;

        Console.WriteLine("Withdrawal succeeded.");
    }

    [Test]
    public async Task TestDepositActivity()
    {
        var details = new PaymentDetails("85-150", "43-812", 400, "12345");
        var activities = new BankingActivities();
        var output = new StringWriter();
        Console.SetOut(output);

        await activities.DepositAsync(details);

        const string depositPattern = @"Depositing \$(\d+) into account (\d+-\d+)";
        var match = Regex.Match(output.ToString(), depositPattern);
        if (!match.Success)
        {
            throw new InvalidOperationException("Failed to parse deposit output.");
        }

        var amount = int.Parse(match.Groups[1].Value);
        var accountNumber = match.Groups[2].Value;

        Console.WriteLine("Deposit succeeded.");
    }
}
