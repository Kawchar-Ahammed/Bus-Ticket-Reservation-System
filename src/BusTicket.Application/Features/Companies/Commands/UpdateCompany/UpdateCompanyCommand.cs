using BusTicket.Application.Contracts.Repositories;
using BusTicket.Domain.Entities;
using BusTicket.Domain.Exceptions;
using MediatR;

namespace BusTicket.Application.Features.Companies.Commands.UpdateCompany;

public record UpdateCompanyCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? ContactNumber { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}

public class UpdateCompanyCommandHandler : IRequestHandler<UpdateCompanyCommand, Unit>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCompanyCommandHandler(ICompanyRepository companyRepository, IUnitOfWork unitOfWork)
    {
        _companyRepository = companyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(UpdateCompanyCommand request, CancellationToken cancellationToken)
    {
        // Validate
        var validator = new UpdateCompanyCommandValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        
        if (!validationResult.IsValid)
        {
            var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new ValidationException(errors);
        }

        // Get existing company
        var company = await _companyRepository.GetByIdAsync(request.Id);
        
        if (company == null)
        {
            throw new NotFoundException($"Company with ID {request.Id} not found");
        }

        // Update properties
        company.UpdateDetails(
            request.Name,
            request.Description,
            request.ContactNumber,
            request.Email
        );

        if (request.IsActive)
            company.Activate();
        else
            company.Deactivate();

        // Save
        await _companyRepository.UpdateAsync(company);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Unit.Value;
    }
}
