var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});



builder.Configuration.AddEnvironmentVariables();


var app = builder.Build();


// ENABLE SWAGGER IN BOTH LOCAL + RENDER
app.UseSwagger();
app.UseSwaggerUI();


// CORS
app.UseCors("AllowAll");


// HTTPS
app.UseHttpsRedirection();


// CONTROLLERS
app.MapControllers();

app.Run();