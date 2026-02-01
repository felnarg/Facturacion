using Clientes.Domain.Entities;
using Clientes.Domain.Repositories;
using Clientes.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Clientes.Infrastructure.Repositories;

public sealed class CustomerRepository : ICustomerRepository
{
    private readonly ClientesDbContext _dbContext;

    public CustomerRepository(ClientesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Customers.FirstOrDefaultAsync(customer => customer.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Customer>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Customers.AsNoTracking()
            .OrderBy(customer => customer.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Customer>> SearchAsync(string search, CancellationToken cancellationToken = default)
    {
        var normalized = search.Trim().ToLowerInvariant();

        return await _dbContext.Customers.AsNoTracking()
            .Where(customer =>
                customer.Name.ToLower().Contains(normalized) ||
                customer.Email.ToLower().Contains(normalized) ||
                customer.City.ToLower().Contains(normalized) ||
                customer.Address.ToLower().Contains(normalized) ||
                customer.Phone.ToLower().Contains(normalized) ||
                customer.IdentificationNumber.ToLower().Contains(normalized))
            .OrderBy(customer => customer.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        _dbContext.Customers.Add(customer);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        _dbContext.Customers.Update(customer);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        _dbContext.Customers.Remove(customer);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
