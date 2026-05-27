var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy
                .WithOrigins(
                    "https://pharma-nest-frontend.vercel.app",
                    "http://localhost:5173"
                )
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});


builder.Configuration.AddEnvironmentVariables();

var app = builder.Build();


// SWAGGER
app.UseSwagger();
app.UseSwaggerUI();


// IMPORTANT ORDER
app.UseRouting();


// CORS
app.UseCors("AllowFrontend");


// HTTPS
app.UseHttpsRedirection();


// AUTH (if future)
app.UseAuthorization();


// CONTROLLERS
app.MapControllers();

app.Run();