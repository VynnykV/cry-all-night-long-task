﻿using AutoMapper;
using ProjectStructure.Common.DTO.Task;
using ProjectStructure.Common.DTO.Team;
using ProjectStructure.DAL.Entities;

namespace ProjectStructure.BLL.MappingProfiles
{
    public class TaskProfile : Profile
    {
        public TaskProfile()
        {
            CreateMap<Task, TaskDTO>().ReverseMap();
            CreateMap<TaskCreateDTO, Task>();
            CreateMap<TaskUpdateDTO, Task>();
        }
    }
}