using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using GitVisualizer;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddMudServices();

// Blazor.LocalStorage.WebAssembly 9.0.1 — DI extension method: AddLocalStorageServices()
builder.Services.AddLocalStorageServices();

// Future singletons — added in Stories 2.3–2.5:
// builder.Services.AddSingleton<IGitSimulatorService, GitSimulatorService>();
// builder.Services.AddSingleton<ICommandParserService, CommandParserService>();
// builder.Services.AddSingleton<IGraphRenderService, GraphRenderService>();

await builder.Build().RunAsync();
