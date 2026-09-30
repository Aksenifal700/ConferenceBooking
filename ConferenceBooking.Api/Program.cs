using ConferenceBooking.Configuration;
using ConferenceBooking.Application.Configuration;
using ConferenceBooking.DataAccess.Configurations;
using ConferenceBooking.Middlewares;
using ConferenceBooking.Validation;
using FluentValidation;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddSwaggerConfiguration();

builder.Services.AddValidatorsFromAssemblyContaining<CreateRoomValidator>();
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddIdentityConfiguration();
builder.Services.AddJwtConfiguration(builder.Configuration);
builder.Services.AddRepositories();
builder.Services.AddServices();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwaggerConfiguration();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();