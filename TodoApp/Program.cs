using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore; // Added this using directive
using Microsoft.Identity.Web;
using TodoApp.Data;

// Ensure the EF Core SQL Server package is installed
// Install-Package Microsoft.EntityFrameworkCore.SqlServer

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

// Add EF Core with SQL Server
builder.Services.AddDbContext<TodoDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
	options.AddDefaultPolicy(policy =>
	{
		policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
	});
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseDefaultFiles();    
app.UseStaticFiles();
app.UseDirectoryBrowser();

app.UseRouting();


app.UseCors(builder =>
	builder.AllowAnyOrigin()
		   .AllowAnyMethod()
		   .AllowAnyHeader());

app.UseCors("AllowAll");


app.UseAuthentication();  
app.UseAuthorization();

app.MapControllers();

app.Run();
