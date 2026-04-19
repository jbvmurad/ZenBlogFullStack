using AutoMapper;
using GenericRepository;
using Microsoft.EntityFrameworkCore;
using ZenBlog.Application.Features.ZenBlogFeatures.SubCommentFeatures.Commands.CreateSubComment;
using ZenBlog.Application.Features.ZenBlogFeatures.SubCommentFeatures.Commands.DeleteSubComment;
using ZenBlog.Application.Features.ZenBlogFeatures.SubCommentFeatures.Commands.UpdateSubComment;
using ZenBlog.Application.Services.ZenBlogService;
using ZenBlog.Domain.Entities.ZenBlogEntities;
using ZenBlog.Domain.Repositories.ZenBlogRepositories;

namespace ZenBlog.Persistance.Services.ZenBlogServices;

public sealed class SubCommentService : ISubCommentService
{
    private readonly ISubCommentRepository _subCommentRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public SubCommentService(ISubCommentRepository subCommentRepository, IMapper mapper, IUnitOfWork unitOfWork)
    {
        _subCommentRepository = subCommentRepository;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task CreateAsync(CreateSubCommentCommand request, CancellationToken cancellationToken)
    {
        ZenBlog.Domain.Entities.ZenBlogEntities.SubComment subComment = _mapper.Map<ZenBlog.Domain.Entities.ZenBlogEntities.SubComment>(request);
        await _subCommentRepository.AddAsync(subComment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(DeleteSubCommentCommand request, CancellationToken cancellationToken)
    {
        var subComment = await _subCommentRepository.Where(x => x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);
        if (subComment is null) throw new ArgumentException($"SubComment with Id {request.Id} isn't found");
        _subCommentRepository.Delete(subComment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public IQueryable<SubComment> GetAllSubComments() => _subCommentRepository.GetAll().AsNoTracking();

    public async Task UpdateAsync(UpdateSubCommentCommand request, CancellationToken cancellationToken)
    {
        var subComment = await _subCommentRepository.Where(x => x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);
        if (subComment is null) throw new ArgumentException($"SubComment with Id {request.Id} isn't found");
        _mapper.Map(request, subComment);
        _subCommentRepository.Update(subComment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
