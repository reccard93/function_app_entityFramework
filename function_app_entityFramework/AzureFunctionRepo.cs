using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;
using MyClassLibrary.models;
using MyClassLibrary.Data;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using MyClassLibrary.Repositories;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using function_app_entityFramework;
using MyClassLibrary.Repositories;
using MyClassLibrary.UnitOfWork;

[assembly: FunctionsStartup(typeof(function_app_entityFramework.Startup))]

namespace function_app_entityFramework
{
    public class AzureFunctionRepo
    {
        private IUnitOfWork _unitOfWork;

        public AzureFunctionRepo(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [FunctionName("GetToDoList")]
        public async Task<IActionResult> GetToDoList(
             [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
             ILogger log)
        {
            var toDoList = await _unitOfWork.ToDoListRepository.GetAll();
            await _unitOfWork.Save();
            return new OkObjectResult(toDoList);
        }

        [FunctionName("GetToDoListItem")]
        public async Task<IActionResult> GetToDoListItem(
         [HttpTrigger(AuthorizationLevel.Function, "get", Route = "todolist/{id}")] HttpRequest req,
         [FromRoute] int id,
         ILogger log)
        {
            var toDoList = await _unitOfWork.ToDoListRepository.Get(id);
            if (toDoList == null)
            {
                return new NotFoundResult();
            }
            await _unitOfWork.Save();
            return new OkObjectResult(toDoList);
        }

        [FunctionName("UpdateToDoListItem")]
        public  async Task<IActionResult> UpdateToDoListItem(
        [HttpTrigger(AuthorizationLevel.Function, "put", Route = "todolist/{Id}")] HttpRequest req,
        [FromRoute] int id,
        ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var updatedToDoList = JsonConvert.DeserializeObject<ToDoList>(requestBody);
            var toDoList = await _unitOfWork.ToDoListRepository.Get(id);
            if (toDoList == null)
            {
                return new NotFoundResult();
            }

            toDoList.Description = updatedToDoList.Description;
            toDoList.IsCompleted = updatedToDoList.IsCompleted;

            await _unitOfWork.ToDoListRepository.Update(toDoList);
            await _unitOfWork.Save();

            return new OkObjectResult(toDoList);
        }

        [FunctionName("CreateToDoListItem")]
        public  async Task<IActionResult> CreateToDoListItem(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "todolist")] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var newToDoList = JsonConvert.DeserializeObject<ToDoList>(requestBody);
            await _unitOfWork.ToDoListRepository.Add(newToDoList);
            await _unitOfWork.Save();

            return new OkObjectResult(newToDoList);
        }

        [FunctionName("DeleteToDoListItem")]
        public  async Task<IActionResult> DeleteToDoListItem(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "todolist/{id}")] HttpRequest req,
             ILogger log,
            [FromRoute] int id)
        {
            var toDoList = await _unitOfWork.ToDoListRepository.Get(id);
            if (toDoList == null)
            {
                return new NotFoundResult();
            }
            await _unitOfWork.ToDoListRepository.Delete(id);
            await _unitOfWork.Save();
            return new OkObjectResult("L'item � stato cancellato con successo ");
        }
        //api dipendenti=======================

        [FunctionName("GetAllDipendenti")]
        public  async Task<IActionResult> GetAllDipendenti(
         [HttpTrigger(AuthorizationLevel.Function, "get", Route = "dipendenti")] HttpRequest req,
             ILogger log)
        {
            var Dipendenti = _unitOfWork.DependentsRepository.GetAllDependents();
            await _unitOfWork.Save();
            return new OkObjectResult(Dipendenti);
        }

        [FunctionName("CreateDipendente")]
        public  async Task<IActionResult> CreateDipendente(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "createdipendente")] HttpRequest req,
        ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var newDipendente = JsonConvert.DeserializeObject<Dipendenti>(requestBody);
            var existingDipendente = _unitOfWork.DependentsRepository.GetAllDependents().FirstOrDefault(x => x.Name == newDipendente.Name);
            if (existingDipendente != null)
            {
                return new BadRequestObjectResult("Esiste gi� un dipendente con lo stesso nome");
            }

            await _unitOfWork.DependentsRepository.Add(newDipendente);
            await _unitOfWork.Save();
            return new OkObjectResult(newDipendente);
        }

        [FunctionName("UpdateDipendente")]
        public  async Task<IActionResult> UpdateDipendente(
        [HttpTrigger(AuthorizationLevel.Function, "put", Route = "dipendente/{Id}")] HttpRequest req,
        [FromRoute] int id,
            ILogger log)
        {
            var Dipendente = await _unitOfWork.DependentsRepository.Get(id);
            if (Dipendente == null)
            {
                return new NotFoundResult();
            }
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var updatedDipendente = JsonConvert.DeserializeObject<Dipendenti>(requestBody);

            Dipendente.Name = updatedDipendente.Name;

            await _unitOfWork.DependentsRepository.Update(Dipendente);
            await _unitOfWork.Save();

            return new OkObjectResult(Dipendente);
        }
         [FunctionName("GetDependent")]
            public  async Task<IActionResult> GetDependent(
         [HttpTrigger(AuthorizationLevel.Function, "get", Route = "dependent/{id}")] HttpRequest req,
         [FromRoute] int id,
                ILogger log)
         {
            var toDoList = await _unitOfWork.DependentsRepository.Get(id);
            if (toDoList == null)
            {
                return new NotFoundResult();
            }
                await _unitOfWork.Save();
                return new OkObjectResult(toDoList);
         }


        [FunctionName("DeleteDipendente")]
        public  async Task<IActionResult> DeleteDipendente(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "DeleteDipendente/{id}")] HttpRequest req, ILogger log, int id)
        {
            var dipendente = await _unitOfWork.DependentsRepository.Get(id);

            if (dipendente == null)
            {
                return new NotFoundResult();
            }

            await _unitOfWork.DependentsRepository.Delete(id);

            await _unitOfWork.Save();

            return new OkResult();
        }
    }
}
