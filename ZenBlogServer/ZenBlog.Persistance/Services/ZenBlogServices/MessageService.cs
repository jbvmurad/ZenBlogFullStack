using AutoMapper;
using AutoMapper.QueryableExtensions;
using GenericRepository;
using Microsoft.EntityFrameworkCore;
using ZenBlog.Application.Features.ZenBlogFeatures.MessageFeatures.Commands.CreateMessage;
using ZenBlog.Application.Features.ZenBlogFeatures.MessageFeatures.Commands.DeleteMessage;
using ZenBlog.Application.Features.ZenBlogFeatures.MessageFeatures.Commands.UpdateMessage;
using ZenBlog.Application.Services.ZenBlogService;
using ZenBlog.Domain.DTOs.ZenBlogResponses;
using ZenBlog.Domain.Entities.ZenBlogEntities;
using ZenBlog.Domain.Repositories.ZenBlogRepositories;

namespace ZenBlog.Persistance.Services.ZenBlogServices;

public sealed class MessageService : IMessageService
{
    private readonly IMessageRepository _messageRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public MessageService(IMessageRepository messageRepository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _messageRepository = messageRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task CreateAsync(CreateMessageCommand request, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<Message>(request);

        if (entity is null)
            throw new InvalidOperationException("Message mapping failed.");

        entity.IsRead = false;

        await _messageRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(DeleteMessageCommand request, CancellationToken cancellationToken)
    {
        var message = await _messageRepository
            .Where(x => x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);

        if (message is null)
            throw new KeyNotFoundException("Message not found.");

        _messageRepository.Delete(message);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public IQueryable<MessageItemResponse> GetAllMessages() => _messageRepository
        .GetAll()
        .AsNoTracking()
        .ProjectTo<MessageItemResponse>(_mapper.ConfigurationProvider);

    public async Task UpdateReadStateAsync(UpdateMessageCommand request, CancellationToken cancellationToken)
    {
        var message = await _messageRepository
            .Where(x => x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);

        if (message is null)
            throw new KeyNotFoundException("Message not found.");

        message.IsRead = request.IsRead;

        _messageRepository.Update(message);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
