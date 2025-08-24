using pandaTeste.api.Application.Interfaces;
using pandaTeste.api.Application.Service;
using pandaTeste.api.Core.Interfaces;
using pandaTeste.api.Core.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IViagemService, ViagemService>();
builder.Services.AddScoped<IViagemRepository, ViagemRepository>();

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
