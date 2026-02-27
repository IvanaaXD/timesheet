using TimeSheet.Domain.Interfaces;
using TimeSheet.Infrastructure.Data;
using TimeSheet.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using TimeSheet.Application.Services;
using TimeSheet.Application.Abstractions;
using TimeSheet.Infrastructure.Email;
using TimeSheet.Infrastructure.Identity;
using TimeSheet.Application.Common.Models;
using AutoMapper;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("PostgresConnection");
builder.Services.AddDbContext<TimeSheetDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddAutoMapper(typeof(ICategoryService).Assembly);
builder.Services.AddAutoMapper(typeof(ICountryService).Assembly);
builder.Services.AddAutoMapper(typeof(IClientService).Assembly);
builder.Services.AddAutoMapper(typeof(IMemberService).Assembly);
builder.Services.AddAutoMapper(typeof(IProjectService).Assembly);

builder.Services.AddScoped<IActivityRepository, ActivityRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IMemberRepository, MemberRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IProjectLeadRepository, ProjectLeadRepository>();

builder.Services.AddScoped<IActivityService, ActivityService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICountryService, CountryService>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IProjectLeadService, ProjectLeadService>();

builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IIdentityService, IdentityService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();