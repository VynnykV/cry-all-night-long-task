﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectStructure.BLL.Exceptions;
using ProjectStructure.BLL.Interfaces;
using ProjectStructure.Common.DTO.Task;
using ProjectStructure.DAL.Entities;
using ProjectStructure.DAL.Interfaces;
using Assignment = ProjectStructure.DAL.Entities.Task;
using Task = System.Threading.Tasks.Task;

namespace ProjectStructure.BLL.Services
{
    public class TaskService : BaseService, ITaskService
    {
        public TaskService(
            IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        public async Task<TaskDTO> AddTask(TaskCreateDTO task)
        {
            var taskEntity = _mapper.Map<Assignment>(task);
            taskEntity.CreatedAt = DateTime.Now;
            taskEntity.State = TaskState.Created;
            await _unitOfWork.TaskRepository.Create(taskEntity);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<TaskDTO>(taskEntity);
        }

        public async Task<IEnumerable<TaskDTO>> GetAll()
        {
            return _mapper.Map<IEnumerable<TaskDTO>>(await _unitOfWork.TaskRepository
                .Query()
                .Include(t=>t.Performer)
                .ThenInclude(u=>u.Team)
                .Include(t=>t.Project)
                .ThenInclude(p=>p.Team)
                .ToListAsync());
        }

        public async Task<TaskDTO> GetTaskById(int id)
        {
            var taskEntity = await _unitOfWork.TaskRepository
                .Query()
                .Include(t=>t.Performer)
                .ThenInclude(u=>u.Team)
                .Include(t=>t.Project)
                .ThenInclude(p=>p.Team)
                .FirstOrDefaultAsync(t=>t.Id == id);
            if (taskEntity is null)
                throw new NotFoundException(nameof(Assignment), id);
            return _mapper.Map<TaskDTO>(taskEntity);
        }

        public async Task UpdateTask(TaskUpdateDTO task)
        {
            var taskEntity = await _unitOfWork.TaskRepository.GetById(task.Id);
            if (taskEntity is null)
                throw new NotFoundException((nameof(Assignment), task.Id));
            taskEntity.PerformerId = task.PerformerId;
            taskEntity.Name = task.Name;
            taskEntity.Description = task.Description;
            _unitOfWork.TaskRepository.Update(taskEntity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateTaskState(int id, TaskState state)
        {
            var taskEntity = await _unitOfWork.TaskRepository.GetById(id);
            if (taskEntity is null)
                throw new NotFoundException((nameof(Assignment), id));
            taskEntity.State = state;
            if(taskEntity.State == TaskState.Done)
                taskEntity.FinishedAt = DateTime.Now;
            _unitOfWork.TaskRepository.Update(taskEntity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteTask(int id)
        {
            var taskEntity = await _unitOfWork.TaskRepository.GetById(id);
            if (taskEntity is null)
                throw new NotFoundException(nameof(Assignment), id);
            await _unitOfWork.TaskRepository.Delete(id);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}