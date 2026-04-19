using AutoMapper;
using ZenBlog.Application.Features.ZenBlogFeatures.CommentFeatures.Commands.CreateComment;
using ZenBlog.Application.Features.ZenBlogFeatures.CommentFeatures.Commands.UpdateComment;
using ZenBlog.Domain.DTOs.ZenBlogResponses;
using ZenBlog.Domain.Entities.ZenBlogEntities;

namespace ZenBlog.Persistance.Mappings.ZenBlogMaps;

public sealed class CommentMap:Profile
{
    public CommentMap()
    {
        CreateMap<CreateCommentCommand, Comment>().ReverseMap();
        CreateMap<UpdateCommentCommand, Comment>().ReverseMap();
        CreateMap<Comment, CommentResponse>()
            .ForCtorParam(nameof(CommentResponse.CommenterImageUrl), opt => opt.MapFrom(_ => (string?)null));
    }
}
