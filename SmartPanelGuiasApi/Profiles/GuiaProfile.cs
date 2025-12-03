using AutoMapper;
using SmartPanelGuiasApi.Dtos;
using SmartPanelGuiasApi.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

public class GuiaProfile : Profile
{
    public GuiaProfile()
    {
        CreateMap<Guia, GuiaDto>();
        CreateMap<GuiaCreateDto, Guia>();
        CreateMap<GuiaUpdateDto, Guia>();
    }
}
