using AutoMapper;
using GenericRepository;
using Microsoft.EntityFrameworkCore;
using ZenBlog.Application.Features.ZenBlogFeatures.CommentFeatures.Commands.CreateComment;
using ZenBlog.Application.Features.ZenBlogFeatures.CommentFeatures.Commands.DeleteComment;
using ZenBlog.Application.Features.ZenBlogFeatures.CommentFeatures.Commands.UpdateComment;
using ZenBlog.Application.Services.ZenBlogService;
using ZenBlog.Domain.Entities.ZenBlogEntities;
using ZenBlog.Domain.Repositories.ZenBlogRepositories;

namespace ZenBlog.Persistance.Services.ZenBlogServices;

public sealed class CommentService : ICommentService
{
    private readonly ICommentRepository _commentRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public CommentService(ICommentRepository commentRepository, IMapper mapper, IUnitOfWork unitOfWork)
    {
        _commentRepository = commentRepository;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task CreateAsync(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        Comment comment =_mapper.Map<Comment>(request);
        await _commentRepository.AddAsync(comment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(DeleteCommentCommand request, CancellationToken cancellationToken)
    {
        var comment= await _commentRepository.Where(x=>x.Id==request.Id).FirstOrDefaultAsync();
        if (comment is null) throw new ArgumentException($"Comment with Id {request.Id} isn't found");
        _commentRepository.Delete(comment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public IQueryable<Comment> GetAllComments() => _commentRepository.GetAll().AsQueryable();

    public async Task UpdateAsync(UpdateCommentCommand request, CancellationToken cancellationToken)
    {
        var comment = await _commentRepository.Where(x => x.Id == request.Id).FirstOrDefaultAsync();
        if (comment is null) throw new ArgumentException($"Comment with Id {request.Id} isn't found");
        _mapper.Map(request,comment);
        _commentRepository.Update(comment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
