using Dapper;
using Handson.IRepository;
using Handson.Models;

namespace Handson.Repository;

public class UserRepository : IUserRepository
{
    private readonly DapperContext _context;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(DapperContext context, ILogger<UserRepository> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    public async Task<IEnumerable<User>> GetAllAsync()
    {
        using var connection = _context.CreateConnection();
        
        const string sql = @"
                    SELECT u.*, r.RoleName 
                    FROM Users u
                    LEFT JOIN UserRoles ur ON u.Id = ur.UserId
                    LEFT JOIN Roles r ON ur.RoleId = r.Id";

        var userDictionary = new Dictionary<int, User>();

        await connection.QueryAsync<User, string, User>(
            sql,
            (user, role) => {
                if (!userDictionary.TryGetValue(user.Id, out var existingUser))
                {
                    existingUser = user;
                    existingUser.Roles = new List<string>();
                    userDictionary.Add(user.Id, existingUser);
                }

                if (!string.IsNullOrEmpty(role))
                {
                    existingUser.Roles.Add(role);
                }

                return existingUser;
            },
            splitOn: "RoleName"
        );

        return userDictionary.Values;

    }

    public async Task<User?> GetByIdAsync(int id)
    {
        using var connection = _context.CreateConnection();
        const string sql = @"
                    SELECT u.*, r.RoleName 
                    FROM Users u
                    LEFT JOIN UserRoles ur ON u.Id = ur.UserId
                    LEFT JOIN Roles r ON ur.RoleId = r.Id
                    WHERE u.Id = @Id";

        var userDictionary = new Dictionary<int, User>();

        await connection.QueryAsync<User, string, User>(
            sql,
            (user, role) => {
                if (!userDictionary.TryGetValue(user.Id, out var existingUser))
                {
                    existingUser = user;
                    existingUser.Roles = new List<string>();
                    userDictionary.Add(user.Id, existingUser);
                }

                if (!string.IsNullOrEmpty(role))
                {
                    existingUser.Roles.Add(role);
                }

                return existingUser;
            },
            new { Id = id },
            splitOn: "RoleName"
        );

        return userDictionary.Values.FirstOrDefault();
        
    }

    public async Task<int> AddAsync(User entity)
    {
        using var connection = _context.CreateConnection();
        //try with async-await if not required to open connection for transaction
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            // Insert user
            const string insertUserSql = @"
                        INSERT INTO Users (Username, Email, PasswordHash, PasswordSalt, CreatedAt, IsActive)
                        VALUES (@Username, @Email, @PasswordHash, @PasswordSalt, @CreatedAt, @IsActive) returning id;
                        --SELECT CAST(SCOPE_IDENTITY() as int)";

            var userId = await connection.ExecuteScalarAsync<int>(insertUserSql,
                new
                {
                    entity.Username,
                    entity.Email,
                    entity.PasswordHash,
                    entity.PasswordSalt,
                    entity.CreatedAt,
                    entity.IsActive
                }, transaction);

            // Insert roles
            if (entity.Roles.Any())
            {
                foreach (var role in entity.Roles)
                {
                    // Get or create role
                    const string getRoleSql = "SELECT Id FROM Roles WHERE RoleName = @RoleName";
                    var roleId =
                        await connection.ExecuteScalarAsync<int?>(getRoleSql, new { RoleName = role }, transaction);

                    if (roleId == null)
                    {
                        const string insertRoleSql = @"
                                    INSERT INTO Roles (RoleName) VALUES (@RoleName) returning id;
                                    --SELECT CAST(SCOPE_IDENTITY() as int)";
                        roleId = await connection.ExecuteScalarAsync<int>(insertRoleSql, new { RoleName = role },
                            transaction);
                    }

                    // Assign role to user
                    const string assignRoleSql = "INSERT INTO UserRoles (UserId, RoleId) VALUES (@UserId, @RoleId)";
                    await connection.ExecuteAsync(assignRoleSql, new { UserId = userId, RoleId = roleId }, transaction);
                }
            }

            transaction.Commit();
            return userId;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task<bool> UpdateAsync(User entity)
    {
        using var connection = _context.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            // Update user
            const string updateUserSql = @"
                        UPDATE Users 
                        SET Username = @Username, 
                            Email = @Email, 
                            LastLogin = @LastLogin,
                            IsActive = @IsActive
                        WHERE Id = @Id";

            var result = await connection.ExecuteAsync(updateUserSql, entity, transaction);

            if (result > 0 && entity.Roles.Any())
            {
                // Clear existing roles
                const string clearRolesSql = "DELETE FROM UserRoles WHERE UserId = @UserId";
                await connection.ExecuteAsync(clearRolesSql, new { UserId = entity.Id }, transaction);

                // Assign new roles
                foreach (var role in entity.Roles)
                {
                    // Get or create role
                    const string getRoleSql = "SELECT Id FROM Roles WHERE RoleName = @RoleName";
                    var roleId =
                        await connection.ExecuteScalarAsync<int?>(getRoleSql, new { RoleName = role }, transaction);

                    if (roleId == null)
                    {
                        const string insertRoleSql = @"
                                    INSERT INTO Roles (RoleName) VALUES (@RoleName);
                                    SELECT CAST(SCOPE_IDENTITY() as int)";
                        roleId = await connection.ExecuteScalarAsync<int>(insertRoleSql, new { RoleName = role },
                            transaction);
                    }

                    // Assign role to user
                    const string assignRoleSql = "INSERT INTO UserRoles (UserId, RoleId) VALUES (@UserId, @RoleId)";
                    await connection.ExecuteAsync(assignRoleSql, new { UserId = entity.Id, RoleId = roleId },
                        transaction);
                }
            }

            transaction.Commit();
            return result > 0;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var connection = _context.CreateConnection();
        using var transaction = connection.BeginTransaction();

        try
        {
            // Delete user roles first
            const string deleteRolesSql = "DELETE FROM UserRoles WHERE UserId = @UserId";
            await connection.ExecuteAsync(deleteRolesSql, new { UserId = id }, transaction);

            // Delete user
            const string deleteUserSql = "DELETE FROM Users WHERE Id = @Id";
            var result = await connection.ExecuteAsync(deleteUserSql, new { Id = id }, transaction);

            transaction.Commit();
            return result > 0;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        using var connection = _context.CreateConnection();
        const string sql = @"
                    SELECT u.*, r.RoleName 
                    FROM Users u
                    LEFT JOIN UserRoles ur ON u.Id = ur.UserId
                    LEFT JOIN Roles r ON ur.RoleId = r.Id
                    WHERE u.Username = @Username";

        var userDictionary = new Dictionary<int, User>();

        await connection.QueryAsync<User, string, User>(
            sql,
            (user, role) => {
                if (!userDictionary.TryGetValue(user.Id, out var existingUser))
                {
                    existingUser = user;
                    existingUser.Roles = new List<string>();
                    userDictionary.Add(user.Id, existingUser);
                }

                if (!string.IsNullOrEmpty(role))
                {
                    existingUser.Roles.Add(role);
                }

                return existingUser;
            },
            new { Username = username },
            splitOn: "RoleName"
        );

        return userDictionary.Values.FirstOrDefault();
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        using var connection = _context.CreateConnection();
        const string sql = @"
                    SELECT u.*, r.RoleName 
                    FROM Users u
                    LEFT JOIN UserRoles ur ON u.Id = ur.UserId
                    LEFT JOIN Roles r ON ur.RoleId = r.Id
                    WHERE u.Email = @Email";

        var userDictionary = new Dictionary<int, User>();

        await connection.QueryAsync<User, string, User>(
            sql,
            (user, role) => {
                if (!userDictionary.TryGetValue(user.Id, out var existingUser))
                {
                    existingUser = user;
                    existingUser.Roles = new List<string>();
                    userDictionary.Add(user.Id, existingUser);
                }

                if (!string.IsNullOrEmpty(role))
                {
                    existingUser.Roles.Add(role);
                }

                return existingUser;
            },
            new { Email = email },
            splitOn: "RoleName"
        );

        return userDictionary.Values.FirstOrDefault();
    }
}