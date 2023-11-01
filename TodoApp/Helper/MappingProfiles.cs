using AutoMapper;
using TodoApp.Dto;
using TodoApp.Models;

namespace TodoApp.Helper
{
    public class MappingProfiles : Profile
    {

        public MappingProfiles() 
        {
            CreateMap<Todo, TodoListDto>();
            CreateMap<TodoListDto, Todo>();

        }

    }
}
