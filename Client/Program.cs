using MabrukBlazor2026.Client;
using MabrukBlazor2026.Client.Auth;
using MabrukBlazor2026.Client.Helper;
using MabrukBlazor2026.Client.Repository;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient("Blazor.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));

// Supply HttpClient instances that include access tokens when making requests to the server project
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Blazor.ServerAPI"));

ConfigureServices(builder.Services);

void ConfigureServices(IServiceCollection services)
{
    services.AddLocalization();
    services.AddScoped<IRepository, Repository>();
    services.AddSweetAlert2();

    services.AddAuthorizationCore();

    services.AddScoped<CustomAuthenticationProvider>();
    services.AddScoped<AuthenticationStateProvider, CustomAuthenticationProvider>(
        provider => provider.GetRequiredService<CustomAuthenticationProvider>());
    services.AddScoped<ILoginService, CustomAuthenticationProvider>(
        provider => provider.GetRequiredService<CustomAuthenticationProvider>());

    services.AddScoped<DialogService>();

}

var host = builder.Build();
await host.SetDefaultCulture();
await host.RunAsync();