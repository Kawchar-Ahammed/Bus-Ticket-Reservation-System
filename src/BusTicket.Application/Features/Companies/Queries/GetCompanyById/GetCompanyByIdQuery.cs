using BusTicket.Application.Contracts.Repositories;
using MediatR;

namespace BusTicket.Application.Features.Companies.Queries.GetCompanyById;

public record GetCompanyByIdQuery(Guid Id) : IRequest<CompanyDetailDto?>;

public class GetCompanyByIdQueryHandler : IRequestHandler<GetCompanyByIdQuery, CompanyDetailDto?>
{
    private readonly ICompanyRepository _companyRepository;

    public GetCompanyByIdQueryHandler(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task<CompanyDetailDto?> Handle(GetCompanyByIdQuery request, CancellationToken cancellationToken)
    {
        var company = await _companyRepository.GetByIdAsync(request.Id);
        
        if (company == null)
            return null;
        
        return new CompanyDetailDto
        {
            Id = company.Id,
            Name = company.Name,
            Email = company.Email,
            ContactNumber = company.ContactNumber,
            Description = company.Description,
            IsActive = company.IsActive,
            CreatedAt = company.CreatedAt,
            UpdatedAt = company.UpdatedAt,
            CreatedBy = company.CreatedBy,
            UpdatedBy = company.UpdatedBy
        };
    }
}
