using AutoMapper;
using Api.DTOs;
using Api.Models;

namespace Api.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserResponseDTO>();
            CreateMap<UserDTO, User>();
            CreateMap<User, UserDTO>();

            CreateMap<Agency, AgencyHierarchyDTO>()
                .ForMember(dest => dest.Parent, opt => opt.MapFrom(src => src.Parent));

            CreateMap<AgencyDTO, Agency>()
                .ForMember(dest => dest.Parent, opt => opt.Ignore());

            CreateMap<Agency, AgencyDTO>()
                .ForMember(dest => dest.ParentId, opt => opt.MapFrom(src => src.Parent.Id));

            CreateMap<Ticket, TicketResponseDTO>()
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location != null ? AgencyHierarchyDTO.ValueOf(src.Location) : null))
                .ForMember(dest => dest.Customers, opt => opt.MapFrom(src => src.Customers != null ? src.Customers.Select(UserSummaryResponseDTO.ValueOf).ToList() : new List<UserSummaryResponseDTO>()))
                .ForMember(dest => dest.Assignees, opt => opt.MapFrom(src => src.Assignees != null ? src.Assignees.Select(UserSummaryResponseDTO.ValueOf).ToList() : new List<UserSummaryResponseDTO>()))
                .ForMember(dest => dest.Observers, opt => opt.MapFrom(src => src.Observers != null ? src.Observers.Select(UserSummaryResponseDTO.ValueOf).ToList() : new List<UserSummaryResponseDTO>()))
                .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Comments != null ? src.Comments.Select(CommentResponseDTO.ValueOf).ToList() : new List<CommentResponseDTO>()));

            CreateMap<TicketDTO, Ticket>();

            CreateMap<Comment, CommentResponseDTO>()
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author != null ? UserResponseDTO.ValueOf(src.Author) : null));

            CreateMap<CommentDTO, Comment>()
                .ForMember(dest => dest.Author, opt => opt.Ignore())
                .ForMember(dest => dest.Ticket, opt => opt.Ignore());
        }
    }
}
