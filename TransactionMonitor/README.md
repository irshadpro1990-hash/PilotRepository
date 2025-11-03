# Transaction Monitor

This is a .NET 8 Worker Service that monitors a database for stuck transactions and sends a notification to a Microsoft Teams channel if any are found.

## Setup

1.  **Configure the database connection string:** Open the `appsettings.json` file and replace the placeholder value for `DefaultConnection` with your actual connection string.
2.  **Configure the Teams webhook URL:** Open the `appsettings.json` file and replace the placeholder value for `TeamsWebhookUrl` with your actual webhook URL.
3.  **Run the application:** You can run the application from the command line using the `dotnet run` command.

## How it works

The worker service runs on a schedule (every 5 minutes by default) and performs the following tasks:

1.  Connects to the database and queries for transactions that are in an intermediate state (`Processing` or `Pending`) and have not been updated in the last 5 minutes.
2.  If any stuck transactions are found, it sends a notification to the configured Microsoft Teams channel.
