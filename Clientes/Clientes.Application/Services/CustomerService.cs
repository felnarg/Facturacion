using Clientes.Application.Abstractions;
using Clientes.Application.DTOs;
using Clientes.Domain.Entities;
using Clientes.Domain.Repositories;

namespace Clientes.Application.Services;

public sealed class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _repository;

    public CustomerService(ICustomerRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<CustomerDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var customers = await _repository.GetAllAsync(cancellationToken);
        return customers.Select(Map).ToList();
    }

    public async Task<CustomerDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customer = await _repository.GetByIdAsync(id, cancellationToken);
        return customer is null ? null : Map(customer);
    }

    public async Task<CustomerDto> CreateAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default)
    {
        var customer = new Customer(request.Name, request.Email);
        await _repository.AddAsync(customer, cancellationToken);
        return Map(customer);
    }

    private static CustomerDto Map(Customer customer)
    {
        return new CustomerDto(
            customer.Id,
            customer.Name,
            customer.Email,
            customer.Points,
            customer.CreatedAt,
            customer.UpdatedAt);
    }
}
