using Compras.Application.Abstractions;
using Compras.Application.DTOs;
using Compras.Domain.Entities;
using Compras.Domain.Repositories;

namespace Compras.Application.Services;

public sealed class SupplierService : ISupplierService
{
    private readonly ISupplierRepository _repository;

    public SupplierService(ISupplierRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<SupplierDto>> GetAllAsync(string? search, CancellationToken cancellationToken = default)
    {
        var suppliers = await _repository.GetAllAsync(search, cancellationToken);
        return suppliers.Select(Map).ToList();
    }

    public async Task<SupplierDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var supplier = await _repository.GetByIdAsync(id, cancellationToken);
        return supplier is null ? null : Map(supplier);
    }

    public async Task<SupplierDto> CreateAsync(CreateSupplierRequest request, CancellationToken cancellationToken = default)
    {
        var supplier = new Supplier(
            request.Name,
            request.ContactName,
            request.Phone,
            request.Email,
            request.Address);
        await _repository.AddAsync(supplier, cancellationToken);
        return Map(supplier);
    }

    public async Task<SupplierDto?> UpdateAsync(Guid id, UpdateSupplierRequest request, CancellationToken cancellationToken = default)
    {
        var supplier = await _repository.GetByIdAsync(id, cancellationToken);
        if (supplier is null)
        {
            return null;
        }

        var updated = new Supplier(
            request.Name,
            request.ContactName,
            request.Phone,
            request.Email,
            request.Address);

        typeof(Supplier).GetProperty(nameof(Supplier.Id))!.SetValue(updated, supplier.Id);
        typeof(Supplier).GetProperty(nameof(Supplier.CreatedAt))!.SetValue(updated, supplier.CreatedAt);

        await _repository.UpdateAsync(updated, cancellationToken);
        return Map(updated);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var supplier = await _repository.GetByIdAsync(id, cancellationToken);
        if (supplier is null)
        {
            return false;
        }

        await _repository.DeleteAsync(supplier, cancellationToken);
        return true;
    }

    private static SupplierDto Map(Supplier supplier)
    {
        return new SupplierDto(
            supplier.Id,
            supplier.Name,
            supplier.ContactName,
            supplier.Phone,
            supplier.Email,
            supplier.Address);
    }
}
