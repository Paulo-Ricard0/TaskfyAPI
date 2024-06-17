using AutoMapper;
using Taskfy.API.DTOs.Tarefas;
using Taskfy.API.DTOs.Tarefas.Request;
using Taskfy.API.Models;

namespace Taskfy.API.DTOs.Mappings;

public class TarefaDTOMappingProfile : Profile
{
	public TarefaDTOMappingProfile()
	{
		CreateMap<Tarefa, TarefaRequestDTO>().ReverseMap();
		CreateMap<Tarefa, TarefaDTO>().ReverseMap();
	}
}
