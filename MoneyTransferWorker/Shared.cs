// @@@SNIPSTART money-transfer-project-template-dotnet-shared
namespace Temporalio.MoneyTransferProject.Shared;
public record PaymentDetails(
    string SourceAccount,
    string TargetAccount,
    int Amount,
    string ReferenceId);

// @@@SNIPEND