using ShopApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ISRPO_PR4Context>();

var app = builder.Build();

// Configure the HTTP request pipeline.


