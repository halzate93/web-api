using UserManagement.Models;
using UserManagement.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// In-memory user store
var userStore = new Dictionary<Guid, User>();

// GET: Retrieve all users
app.MapGet("/users", () =>
{
    return Results.Ok(userStore.Values.ToList());
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
app.MapPost("/users", (User user) =>
{
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

    // Validate Email
    var emailValidation = ValidationHelper.ValidateEmail(user.Email);
    if (!emailValidation.isValid)
    {
        return Results.BadRequest(new { error = emailValidation.errorMessage, statusCode = 400 });
    }

    // Generate new ID and add user
    user.Id = Guid.NewGuid();
    userStore[user.Id] = user;

    return Results.Created($"/users/{user.Id}", user);
})
.WithName("CreateUser");

// PUT: Update an existing user
app.MapPut("/users/{id:guid}", (Guid id, User updatedUser) =>
{
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

    // Validate Email
    var emailValidation = ValidationHelper.ValidateEmail(updatedUser.Email);
    if (!emailValidation.isValid)
    {
        return Results.BadRequest(new { error = emailValidation.errorMessage, statusCode = 400 });
    }

    // Update user, preserving the ID
    updatedUser.Id = id;
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
