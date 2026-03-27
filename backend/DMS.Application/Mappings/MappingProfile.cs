using AutoMapper;
using DMS.Application.DTOs;
using DMS.Core.Entities;

namespace DMS.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Document, DocumentDto>()
            .ForMember(d => d.CategoryName, opt => opt.MapFrom(s => s.Category.Name))
            .ForMember(d => d.UploadedByName, opt => opt.MapFrom(s => s.UploadedBy.FullName));

        CreateMap<DocumentVersion, DocumentVersionDto>()
            .ForMember(d => d.UploadedByName, opt => opt.MapFrom(s => s.UploadedBy.FullName));

        CreateMap<Category, CategoryDto>();
        CreateMap<CreateCategoryDto, Category>();

        CreateMap<AuditLog, AuditLogDto>()
            .ForMember(d => d.UserName, opt => opt.MapFrom(s => s.User.FullName));
    }
}
