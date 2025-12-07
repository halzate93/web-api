using UserManagement.Models;
using UserManagement.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.All;
    logging.RequestBodyLogLimit = 4096;
    logging.ResponseBodyLogLimit = 4096;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Add built-in HTTP logging middleware
app.UseHttpLogging();

app.UseHttpsRedirection();

// In-memory user store
var userStore = new Dictionary<Guid, User>();

// GET: Retrieve all users
app.MapGet("/users", () =>
{
    return Results.Ok(userStore.Values);
})
.WithName("GetAllUsers");

// GET: Retrieve a specific user by ID
app.MapGet("/users/{id:guid}", (Guid id) =>
{
    if (!userStore.ContainsKey(id))
    {
        return Results.NotFound(new { error = "User not found", statusCode = 404 });
    }

    return Results.Ok(userStore[id]);
})
.WithName("GetUserById");

// POST: Add a new user
app.MapPost("/users", (User? user) =>
{
    // Check if user object is null
    if (user == null)
    {
        return Results.BadRequest(new { error = "User data is required", statusCode = 400 });
    }

    // Validate Name
    var nameValidation = ValidationHelper.ValidateName(user.Name);
    if (!nameValidation.isValid)
    {
        return Results.BadRequest(new { error = nameValidation.errorMessage, statusCode = 400 });
    }

    // Validate Username
    var usernameValidation = ValidationHelper.ValidateUsername(user.Username);
    if (!usernameValidation.isValid)
    {
        return Results.BadRequest(new { error = usernameValidation.errorMessage, statusCode = 400 });
    }

    // Check for duplicate username
    if (userStore.Values.Any(u => u.Username.Equals(user.Username.Trim(), StringComparison.OrdinalIgnoreCase)))
    {
        return Results.BadRequest(new { error = "Username already exists", statusCode = 400 });
    }

    // Validate Email
    var emailValidation = ValidationHelper.ValidateEmail(user.Email);
    if (!emailValidation.isValid)
    {
        return Results.BadRequest(new { error = emailValidation.errorMessage, statusCode = 400 });
    }

    // Check for duplicate email
    if (userStore.Values.Any(u => u.Email.Equals(user.Email.Trim(), StringComparison.OrdinalIgnoreCase)))
    {
        return Results.BadRequest(new { error = "Email already exists", statusCode = 400 });
    }

    // Generate new ID and add user with trimmed values
    user.Id = Guid.NewGuid();
    user.Name = user.Name.Trim();
    user.Username = user.Username.Trim();
    user.Email = user.Email.Trim();
    userStore[user.Id] = user;

    return Results.Created($"/users/{user.Id}", user);
})
.WithName("CreateUser");

// PUT: Update an existing user
app.MapPut("/users/{id:guid}", (Guid id, User? updatedUser) =>
{
    // Check if user object is null
    if (updatedUser == null)
    {
        return Results.BadRequest(new { error = "User data is required", statusCode = 400 });
    }

    if (!userStore.ContainsKey(id))
    {
        return Results.NotFound(new { error = "User not found", statusCode = 404 });
    }

    // Validate Name
    var nameValidation = ValidationHelper.ValidateName(updatedUser.Name);
    if (!nameValidation.isValid)
    {
        return Results.BadRequest(new { error = nameValidation.errorMessage, statusCode = 400 });
    }

    // Validate Username
    var usernameValidation = ValidationHelper.ValidateUsername(updatedUser.Username);
    if (!usernameValidation.isValid)
    {
        return Results.BadRequest(new { error = usernameValidation.errorMessage, statusCode = 400 });
    }

    // Check for duplicate username (excluding current user)
    if (userStore.Values.Any(u => u.Id != id && u.Username.Equals(updatedUser.Username.Trim(), StringComparison.OrdinalIgnoreCase)))
    {
        return Results.BadRequest(new { error = "Username already exists", statusCode = 400 });
    }

    // Validate Email
    var emailValidation = ValidationHelper.ValidateEmail(updatedUser.Email);
    if (!emailValidation.isValid)
    {
        return Results.BadRequest(new { error = emailValidation.errorMessage, statusCode = 400 });
    }

    // Check for duplicate email (excluding current user)
    if (userStore.Values.Any(u => u.Id != id && u.Email.Equals(updatedUser.Email.Trim(), StringComparison.OrdinalIgnoreCase)))
    {
        return Results.BadRequest(new { error = "Email already exists", statusCode = 400 });
    }

    // Update user, preserving the ID and trimming values
    updatedUser.Id = id;
    updatedUser.Name = updatedUser.Name.Trim();
    updatedUser.Username = updatedUser.Username.Trim();
    updatedUser.Email = updatedUser.Email.Trim();
    userStore[id] = updatedUser;

    return Results.Ok(updatedUser);
})
.WithName("UpdateUser");

// DELETE: Remove a user by ID
app.MapDelete("/users/{id:guid}", (Guid id) =>
{
    if (!userStore.ContainsKey(id))
    {
        return Results.NotFound(new { error = "User not found", statusCode = 404 });
    }

    userStore.Remove(id);
    return Results.NoContent();
})
.WithName("DeleteUser");

app.Run();
