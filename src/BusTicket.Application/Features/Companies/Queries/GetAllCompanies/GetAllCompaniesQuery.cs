using BusTicket.Application.Contracts.Repositories;
using MediatR;

namespace BusTicket.Application.Features.Companies.Queries.GetAllCompanies;

public record GetAllCompaniesQuery : IRequest<List<CompanyDto>>;

public class GetAllCompaniesQueryHandler : IRequestHandler<GetAllCompaniesQuery, List<CompanyDto>>
{
    private readonly ICompanyRepository _companyRepository;

    public GetAllCompaniesQueryHandler(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task<List<CompanyDto>> Handle(GetAllCompaniesQuery request, CancellationToken cancellationToken)
    {
        var companies = await _companyRepository.GetAllAsync();
        
        return companies.Select(c => new CompanyDto
        {
            Id = c.Id,
            Name = c.Name,
            Email = c.Email,
            ContactNumber = c.ContactNumber,
            Description = c.Description,
            IsActive = c.IsActive,
            CreatedAt = c.CreatedAt
        }).ToList();
    }
}
