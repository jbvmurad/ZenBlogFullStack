using AutoMapper;
using GenericRepository;
using Microsoft.EntityFrameworkCore;
using ZenBlog.Application.Features.ZenBlogFeatures.MessageFeatures.Commands.CreateMessage;
using ZenBlog.Application.Features.ZenBlogFeatures.MessageFeatures.Commands.DeleteMessage;
using ZenBlog.Application.Features.ZenBlogFeatures.MessageFeatures.Commands.UpdateMessage;
using ZenBlog.Application.Services.ZenBlogService;
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
        var Message = await _messageRepository
            .Where(x=>x.Id==request.Id).FirstOrDefaultAsync();

        if (Message is null)
            throw new KeyNotFoundException("Message not found.");

        _messageRepository.Delete(Message);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public IQueryable<Message> GetAllMessages()
    {
        return _messageRepository.GetAll().AsQueryable();
    }

    public async Task UpdateReadStateAsync(UpdateMessageCommand request, CancellationToken cancellationToken)
    {
        var Message = await _messageRepository
            .Where(x => x.Id == request.Id).FirstOrDefaultAsync();

        if (Message is null)
            throw new KeyNotFoundException("Message not found.");

        Message.IsRead = request.IsRead;

        _messageRepository.Update(Message);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
