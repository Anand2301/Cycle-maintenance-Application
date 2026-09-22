using Cycle_App.Data;
using CycleMaintenance.Services.Authentication;
using Dapper;
using FastEndpoints;

namespace CycleMaintenance.Features.Users.CreateUser;

public class CreateUserEndpoint
    : Endpoint<CreateUserRequest, CreateUserResponse>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IAccountActivationService _accountActivationService;

    public CreateUserEndpoint(
        IDbConnectionFactory dbConnectionFactory,
        IPasswordHasher passwordHasher,
        IAccountActivationService accountActivationService)
    {
        _dbConnectionFactory = dbConnectionFactory;
        _passwordHasher = passwordHasher;
        _accountActivationService = accountActivationService;
    }

    public override void Configure()
    {
        Post("/api/users");

        // Temporary during development.
        // Later this endpoint will be restricted to Admin users.
        AllowAnonymous();
    }

    public override async Task HandleAsync(
        CreateUserRequest req,
        CancellationToken ct)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        using var transaction = connection.BeginTransaction();

        try
        {
            // 1. Hash the user's password.
            var passwordHash =
                _passwordHasher.HashPassword(req.Password);

            // 2. Create the user as inactive.
            const string userSql = """
                INSERT INTO Users
                (
                    FirstName,
                    LastName,
                    Email,
                    PhoneNumber,
                    PasswordHash,
                    IsActive
                )
                VALUES
                (
                    @FirstName,
                    @LastName,
                    @Email,
                    @PhoneNumber,
                    @PasswordHash,
                    FALSE
                )
                RETURNING
                    Id,
                    FirstName,
                    LastName,
                    Email,
                    PhoneNumber,
                    IsActive,
                    CreatedAt;
                """;

            var user = await connection.QuerySingleAsync<CreateUserResponse>(
                userSql,
                new
                {
                    req.FirstName,
                    req.LastName,
                    req.Email,
                    req.PhoneNumber,
                    PasswordHash = passwordHash
                },
                transaction);

            // 3. Assign roles to the user.
            const string roleSql = """
                INSERT INTO UserRoles
                (
                    Id,
                    UserId,
                    RoleId,
                    CreatedAt
                )
                VALUES
                (
                    gen_random_uuid(),
                    @UserId,
                    @RoleId,
                    NOW()
                );
                """;

            foreach (var roleId in req.RoleIds.Distinct())
            {
                await connection.ExecuteAsync(
                    roleSql,
                    new
                    {
                        UserId = user.Id,
                        RoleId = roleId
                    },
                    transaction);
            }

            // 4. Assign groups to the user.
            const string groupSql = """
                INSERT INTO UserGroups
                (
                    Id,
                    UserId,
                    GroupId,
                    CreatedAt
                )
                VALUES
                (
                    gen_random_uuid(),
                    @UserId,
                    @GroupId,
                    NOW()
                );
                """;

            foreach (var groupId in req.GroupIds.Distinct())
            {
                await connection.ExecuteAsync(
                    groupSql,
                    new
                    {
                        UserId = user.Id,
                        GroupId = groupId
                    },
                    transaction);
            }

            // 5. Commit user, roles, and groups together.
            transaction.Commit();

            // 6. Generate an account activation token.
            var activationToken =
                await _accountActivationService.CreateActivationTokenAsync(
                    user.Id,
                    ct);

            // Temporary development output.
            Console.WriteLine(
                $"Activation token for user {user.Id}: {activationToken}");

            // 7. Return the newly created user.
            await Send.OkAsync(user, ct);
        }
        catch
        {
            // Roll back user + role/group assignments
            // if any database operation fails.
            transaction.Rollback();

            throw;
        }
    }
}