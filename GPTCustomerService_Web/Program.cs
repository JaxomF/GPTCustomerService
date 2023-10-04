using GPTCustomerService_Web.Extensions;
using GPTCustomerService_Web.Hubs;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Host.AddConfiguration();
builder.WebHost.UseUrls();

// Add in configuration options and Semantic Kernel services.
builder.Services
    .AddSingleton<ILogger>(sp => sp.GetRequiredService<ILogger<Program>>()) // some services require an un-templated ILogger
    .AddOptions(builder.Configuration)
    .AddAIResponses()
    //.AddPersistentStore()    
    .AddSemanticKernelServices();


// Add SignalR as the real time relay service
builder.Services.AddSignalR(options => options.MaximumParallelInvocationsPerClient = 10);

builder.Services.AddCorsPolicy();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

app.MapHub<MessageRelayHub>("/messageRelayHub");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors();
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
