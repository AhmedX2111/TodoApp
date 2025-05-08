using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore; // Added this using directive
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TodoApp.Data;
using TodoApp.Services;

// Ensure the EF Core SQL Server package is installed
// Install-Package Microsoft.EntityFrameworkCore.SqlServer

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AuthDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));


builder.Services.AddDbContext<TodoDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<AuthService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new()
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,
			ValidIssuer = builder.Configuration["Jwt:Issuer"],
			ValidAudience = builder.Configuration["Jwt:Audience"],
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
				builder.Configuration["Jwt:Key"]!))
		};
	});


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
