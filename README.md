# Money Transfer Project

## Prerequisites

Before running this application, ensure you have the following installed:

* [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) or later
* Temporal Server

## Steps to get started

## Start the Temporal Server
Run the command: 
`temporal server start-dev`

## Clone the Repo
Follow these steps to copy to you local machine. 

```
git clone https://github.com/jsundai/money-transfer-temporal-template-dotnet.git

cd money-transfer-temporal-template-dotnet
```

## Restore Dependencies 
Run the following command: 
```
dotnet restore 
```

## Run The Workflow 
Run the following commands to initiate the Workflow that starts the money transfer process.

1. **Navigate to the Client folder** 

   ```
   cd MoneyTransferClient
   ```
   
1. **Run the following command to begin the client**

   ```
   dotnet run
   ```

## Run the Worker
In a separate terminal, run the following commands: 

1. **Navigate to the Worker folder.**
```
cd MoneyTransferWorker
```
2. **Run the following command.** 
```
dotnet run
```

## Open the Web UI 
To monitor and inspect the progress and status of your money transfer workflows, open the Temporal UI in your browser. This allows you to view running workflows, completed workflows, and detailed execution histories.

Go to [localhost:8233](http://localhost:8233/). 
