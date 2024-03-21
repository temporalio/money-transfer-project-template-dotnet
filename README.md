# Money Transfer Project

## Prerequisites

Before running this application, ensure you have the following installed:

* [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) or later
* [Temporal CLI](https://learn.temporal.io/getting_started/dotnet/dev_environment/)

## Steps to get started

## Start the Temporal Server
Run the command:
`temporal server start-dev`

## Clone the Repo
Follow these steps to copy to you local machine.

```
git clone https://github.com/temporalio/money-transfer-project-template-dotnet

cd money-transfer-temporal-template-dotnet
```

## Run The Workflow
Run the following commands to initiate the Workflow that starts the money transfer process.

1. **Run the following command to begin the client**

   ```
   dotnet run --project MoneyTransferClient
   ```

## Run the Worker
In a separate terminal, run the following commands:

2. **Run the following command to begin the worker**

   ```
   dotnet run --project MoneyTransferWorker
   ```

## Open the Web UI
To monitor and inspect the progress and status of your money transfer workflows, open the Temporal UI in your browser. This allows you to view running workflows, completed workflows, and detailed execution histories.

Go to [localhost:8233](http://localhost:8233/).
