using AutoMapper;
using BlogAPI.DTOs;
using BlogAPI.Models;

namespace BlogAPI.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Post mappings
            CreateMap<Post, PostDto>()
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy.DisplayName ?? src.CreatedBy.UserName))
                .ForMember(dest => dest.UpdatedBy, opt => opt.MapFrom(src => src.UpdatedBy != null ? src.UpdatedBy.DisplayName ?? src.UpdatedBy.UserName : null));

            CreateMap<PostCreateDto, Post>();
            CreateMap<PostUpdateDto, Post>();

            // Comment mappings
            CreateMap<Comment, CommentDto>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User.DisplayName ?? src.User.UserName));

            CreateMap<CommentCreateDto, Comment>();
            CreateMap<CommentUpdateDto, Comment>();
        }
    }
}
