using ZenBlog.Application.Features.ZenBlogFeatures.MessageFeatures.Commands.CreateMessage;
using ZenBlog.Application.Features.ZenBlogFeatures.MessageFeatures.Commands.DeleteMessage;
using ZenBlog.Application.Features.ZenBlogFeatures.MessageFeatures.Commands.UpdateMessage;
using ZenBlog.Domain.Entities.ZenBlogEntities;

namespace ZenBlog.Application.Services.ZenBlogService;

public interface IMessageService
{
    IQueryable<Message> GetAllMessages();
    Task CreateAsync(CreateMessageCommand request, CancellationToken cancellationToken);
    Task UpdateReadStateAsync(UpdateMessageCommand request, CancellationToken cancellationToken);
    Task DeleteAsync(DeleteMessageCommand request, CancellationToken cancellationToken);
}
