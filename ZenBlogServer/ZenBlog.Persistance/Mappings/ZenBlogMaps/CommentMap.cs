using AutoMapper;
using ZenBlog.Application.Features.ZenBlogFeatures.CommentFeatures.Commands.CreateComment;
using ZenBlog.Application.Features.ZenBlogFeatures.CommentFeatures.Commands.UpdateComment;
using ZenBlog.Domain.Entities.ZenBlogEntities;

namespace ZenBlog.Persistance.Mappings.ZenBlogMaps;

public sealed class CommentMap:Profile
{
    public CommentMap()
    {
        CreateMap<CreateCommentCommand, Comment>().ReverseMap();
        CreateMap<UpdateCommentCommand, Comment>().ReverseMap();
    }
}
