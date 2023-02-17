using AutoMapper;
using AzureFunctionDTO.RequestDTO;
using AzureFunctionDTO.ResponseDTO;
using MyClassLibrary.models;

namespace AzureFunctionDTO
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ToDoList, ToDoListDto>();
            CreateMap<CreateToDoListDto, ToDoList>()
                .ForMember(dest => dest.DipendenteId, opt => opt.MapFrom(src => src.DipendenteId))
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<Dipendenti, DipendenteDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));
            CreateMap<CreateDipendenteDto, Dipendenti>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));

            // Mapping personalizzato per ToDoListDto e ToDoList
            CreateMap<ToDoListDto, ToDoList>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.IsCompleted, opt => opt.MapFrom(src => src.IsCompleted))
                .ForMember(dest => dest.DipendenteId, opt => opt.MapFrom(src => src.DipendenteId))
                    .ReverseMap();
        }
    }



}