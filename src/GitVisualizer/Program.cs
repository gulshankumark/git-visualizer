using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using GitVisualizer;
using GitVisualizer.Services;
using GitVisualizer.Interop;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddSingleton(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddMudServices();

// Blazor.LocalStorage.WebAssembly 9.0.1 — DI extension method: AddLocalStorageServices()
builder.Services.AddLocalStorageServices();

builder.Services.AddSingleton<IThemeService, ThemeService>();
builder.Services.AddSingleton<SplitPaneJsInterop>();

builder.Services.AddSingleton<GitJsInterop>();
builder.Services.AddSingleton<ISessionStorageService, SessionStorageService>();
builder.Services.AddSingleton<IGitSimulatorService, GitSimulatorService>();

// Future singletons — added in Stories 2.3–2.5:
builder.Services.AddSingleton<ICommandParserService, CommandParserService>();
builder.Services.AddSingleton<IGraphRenderService, GraphRenderService>();
builder.Services.AddSingleton<GraphRendererJsInterop>();

// Story 3.3 — Storage quota monitoring
builder.Services.AddSingleton<StorageJsInterop>();
builder.Services.AddSingleton<IStorageJsInterop>(sp => sp.GetRequiredService<StorageJsInterop>());
builder.Services.AddSingleton<IStorageMonitorService, StorageMonitorService>();

// Story 4.1 — Service Worker Update Detection
builder.Services.AddSingleton<IServiceWorkerUpdateService, ServiceWorkerUpdateService>();

// Story 5.3 — Viewport breakpoint detection
builder.Services.AddSingleton<ViewportJsInterop>();

await builder.Build().RunAsync();
