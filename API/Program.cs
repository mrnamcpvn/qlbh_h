using API.Configurations;
using API.Helpers.Utilities;
using API.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();

// Add services to the container.

builder.Services.AddControllers();

// Setting DBContexts
builder.Services.AddDatabaseConfiguration(builder.Configuration);

// Add Authentication
builder.Services.AddAuthenticationConfigufation(builder.Configuration);

// RepositoryAccessor and Service
builder.Services.AddDependencyInjectionConfiguration();

// Swagger Config
builder.Services.AddSwaggerGenConfiguration();

// Aspose Config
AsposeUtility.Install();

//Exception Handling Middleware Error
builder.Services.AddTransient<ExceptionHandlingMiddleware>();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(x => x.SetIsOriginAllowed(_ => true).AllowAnyHeader().AllowAnyMethod().AllowCredentials());
// app.UseHttpsRedirection();

app.UseRouting();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHub<NotificationHub>("/hubs/notifications");

app.Run();