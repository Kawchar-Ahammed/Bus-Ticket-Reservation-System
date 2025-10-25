using BusTicket.Application.Contracts.Repositories;
using BusTicket.Domain.Entities;
using BusTicket.Domain.Exceptions;
using MediatR;

namespace BusTicket.Application.Features.Companies.Commands.CreateCompany;

public record CreateCompanyCommand : IRequest<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? ContactNumber { get; set; }
    public string? Description { get; set; }
}

public class CreateCompanyCommandHandler : IRequestHandler<CreateCompanyCommand, Guid>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCompanyCommandHandler(ICompanyRepository companyRepository, IUnitOfWork unitOfWork)
    {
        _companyRepository = companyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
    {
        // Validate
        var validator = new CreateCompanyCommandValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        
        if (!validationResult.IsValid)
        {
            var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new ValidationException(errors);
        }

        // Create entity
        var company = Company.Create(
            request.Name,
            request.Email,
            request.ContactNumber,
            request.Description
        );

        // Save
        await _companyRepository.AddAsync(company);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return company.Id;
    }
}
