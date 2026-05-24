using Microsoft.EntityFrameworkCore;

public interface IUserService
{
    // CRUD
    Task<IEnumerable<User>> GetUsersAsync();
    Task<User?> GetUserByIdAsync(int id);
    Task<User> CreateUserAsync(User user);
    Task<User?> UpdateUserAsync(int id, User user);
    Task<User?> DeleteUserAsync(int id);
}

// SERVICE
public class UserService : IUserService
{
    private readonly UsersContext _context;

    public UserService(UsersContext context)
    {
        _context = context;
    }

    // GET ALL
    public async Task<IEnumerable<User>> GetUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }

    // GET BY ID
    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    // CREATE
    public async Task<User> CreateUserAsync(User user)
    {
        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        return user;
    }

    // UPDATE
    public async Task<User?> UpdateUserAsync(int id, User user)
    {
        var existingUser = await _context.Users.FindAsync(id);

        if (existingUser == null)
            return null;

        existingUser.Name = user.Name;
        existingUser.Email = user.Email;

        await _context.SaveChangesAsync();

        return existingUser;
    }

    // DELETE
    public async Task<User?> DeleteUserAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
            return null;

        _context.Users.Remove(user);

        await _context.SaveChangesAsync();

        return user;
    }
}