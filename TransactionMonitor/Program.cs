using TransactionMonitor;
using TransactionMonitor.Services;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHttpClient<TeamsNotifier>();
builder.Services.AddSingleton<TeamsNotifier>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
