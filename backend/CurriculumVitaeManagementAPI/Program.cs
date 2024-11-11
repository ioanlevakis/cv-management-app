using CurriculumVitaeManagementAPI.AppDbContext;
using CurriculumVitaeManagementAPI.Interfaces;
using CurriculumVitaeManagementAPI.Services;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<DatabaseContext>(options => options.UseInMemoryDatabase(databaseName: "CurriculumVitaeManagementDatabase"));
builder.Services.AddScoped<ICandidateService, CandidateService>();
builder.Services.AddScoped<IDegreeService,DegreeService>();
builder.Services.AddScoped<ISessionValidationService, SessionValidationService>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<AutoValidateAntiforgeryTokenAttribute>();
});

builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "XSS-CSRF-TOKEN";
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 25 * 1024 * 1024;
});



builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        builder =>
        {
            builder.WithOrigins("https://localhost:3000")
                                .AllowAnyHeader()
                                .AllowAnyMethod()
                                .AllowCredentials();
        });
});

var app = builder.Build();

app.UseRouting();

app.UseCors("AllowSpecificOrigins");

app.UseHsts();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
