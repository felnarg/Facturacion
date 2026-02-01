using Clientes.Application.Abstractions;
using Clientes.Application.DTOs;
using Clientes.Domain.Entities;
using Clientes.Domain.Repositories;
using Facturacion.Shared.Events;

namespace Clientes.Application.Services;

public sealed class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _repository;
    private readonly IEventBus _eventBus;

    public CustomerService(ICustomerRepository repository, IEventBus eventBus)
    {
        _repository = repository;
        _eventBus = eventBus;
    }

    public async Task<IReadOnlyList<CustomerDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var customers = await _repository.GetAllAsync(cancellationToken);
        return customers.Select(Map).ToList();
    }

    public async Task<IReadOnlyList<CustomerDto>> SearchAsync(string search, CancellationToken cancellationToken = default)
    {
        var customers = await _repository.SearchAsync(search, cancellationToken);
        return customers.Select(Map).ToList();
    }

    public async Task<CustomerDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customer = await _repository.GetByIdAsync(id, cancellationToken);
        return customer is null ? null : Map(customer);
    }

    public async Task<bool?> GetCreditApprovalAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customer = await _repository.GetByIdAsync(id, cancellationToken);
        return customer?.IsCreditApproved;
    }

    public async Task<CustomerDto> CreateAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default)
    {
        var customer = new Customer(
            request.Name,
            request.Email,
            request.City,
            request.Phone,
            request.Address,
            request.Type,
            request.IdentificationType,
            request.IdentificationNumber,
            request.IsCreditApproved,
            request.ApprovedCreditLimit,
            request.ApprovedPaymentTermDays);
        await _repository.AddAsync(customer, cancellationToken);

        await PublishCreditApprovalIfNeeded(customer, cancellationToken);
        return Map(customer);
    }

    public async Task<CustomerDto?> UpdateAsync(
        Guid id,
        UpdateCustomerRequest request,
        CancellationToken cancellationToken = default)
    {
        var customer = await _repository.GetByIdAsync(id, cancellationToken);
        if (customer is null)
        {
            return null;
        }

        customer.UpdateDetails(
            request.Name,
            request.Email,
            request.City,
            request.Phone,
            request.Address,
            request.Type,
            request.IdentificationType,
            request.IdentificationNumber);
        customer.SetCreditApproval(
            request.IsCreditApproved,
            request.ApprovedCreditLimit,
            request.ApprovedPaymentTermDays);

        await _repository.UpdateAsync(customer, cancellationToken);
        await PublishCreditApprovalIfNeeded(customer, cancellationToken);
        return Map(customer);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var customer = await _repository.GetByIdAsync(id, cancellationToken);
        if (customer is null)
        {
            return false;
        }

        await _repository.DeleteAsync(customer, cancellationToken);
        return true;
    }

    private static CustomerDto Map(Customer customer)
    {
        return new CustomerDto(
            customer.Id,
            customer.Name,
            customer.Email,
            customer.City,
            customer.Phone,
            customer.Address,
            customer.Type,
            customer.IdentificationType,
            customer.IdentificationNumber,
            customer.IsCreditApproved,
            customer.ApprovedCreditLimit,
            customer.ApprovedPaymentTermDays,
            customer.Points,
            customer.CreatedAt,
            customer.UpdatedAt);
    }

    private async Task PublishCreditApprovalIfNeeded(Customer customer, CancellationToken cancellationToken)
    {
        if (!customer.IsCreditApproved)
        {
            return;
        }

        var creditApproved = new CustomerCreditApproved(
            customer.Id,
            customer.Name,
            customer.IdentificationType.ToString(),
            customer.IdentificationNumber,
            customer.ApprovedCreditLimit,
            customer.ApprovedPaymentTermDays);

        await _eventBus.PublishAsync(creditApproved, "customer.credit.approved", cancellationToken);
    }
}
