using AutoMapper;
using ZenBlog.Application.Features.ZenBlogFeatures.SubCommentFeatures.Commands.CreateSubComment;
using ZenBlog.Application.Features.ZenBlogFeatures.SubCommentFeatures.Commands.UpdateSubComment;
using ZenBlog.Domain.DTOs.ZenBlogResponses;

namespace ZenBlog.Persistance.Mappings.ZenBlogMaps;

public sealed class SubComment :Profile
{
    public SubComment()
    {
        CreateMap<CreateSubCommentCommand,SubComment>().ReverseMap();
        CreateMap<UpdateSubCommentCommand,SubComment>().ReverseMap();
        CreateMap<SubComment, SubCommentResponse>();
    }
}
