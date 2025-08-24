using Microsoft.EntityFrameworkCore;
using pandaTeste.api.Application.Interfaces;
using pandaTeste.api.Application.Service;
using pandaTeste.api.Core.Interfaces;
using pandaTeste.api.Core.Repositories;
using pandaTeste.api.Infrastructure.Context;

var builder = WebApplication.CreateBuilder(args);

//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
//builder.Services.AddDbContext<PandaDbContext>(options =>
//    options.UseSqlServer(connectionString));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IViagemService, ViagemService>();
builder.Services.AddScoped<IViagemRepository, ViagemRepository>();

builder.Services.AddScoped<IEstoqueService, EstoqueService>();
builder.Services.AddSingleton<IEstoqueRepository, EstoqueRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
