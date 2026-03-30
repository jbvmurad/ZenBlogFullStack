using AutoMapper;
using ZenBlog.Application.Features.ZenBlogFeatures.MessageFeatures.Commands.CreateMessage;
using ZenBlog.Application.Features.ZenBlogFeatures.MessageFeatures.Commands.UpdateMessage;
using ZenBlog.Domain.DTOs.ZenBlogResponses;
using ZenBlog.Domain.Entities.ZenBlogEntities;

namespace ZenBlog.Persistance.Mappings.ZenBlogMaps;

public sealed class MessageMap :Profile
{
    public MessageMap()
    {
        CreateMap<CreateMessageCommand, Message>().ReverseMap();
        CreateMap<UpdateMessageCommand, Message>().ReverseMap();
        CreateMap<Message, MessageItemResponse>();
    }
}
