using Microsoft.AspNetCore.Identity;
using ShopApi.Models;
using ShopApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ISRPO_PR4Context>();

builder.Services.AddScoped<AuthService>();

var app = builder.Build();

// Configure the HTTP request pipeline.


