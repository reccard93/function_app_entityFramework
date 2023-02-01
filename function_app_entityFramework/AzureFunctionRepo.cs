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
using function_app_entityFramework.models;
using function_app_entityFramework.Data;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using function_app_entityFramework.Repositories;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using function_app_entityFramework;
using MyClassLibrary.Repositories;

[assembly: FunctionsStartup(typeof(function_app_entityFramework.Startup))]

namespace function_app_entityFramework
{
    public class AzureFunctionRepo
    {
        private  IDependentsRepository _Dependentrep;
        private  IToDoListRepository _ToDoListRepositoryrep;

        public AzureFunctionRepo(IDependentsRepository Dependentrep, IToDoListRepository ToDoListRepositoryrep)
        {
            _Dependentrep = Dependentrep;
            _ToDoListRepositoryrep = ToDoListRepositoryrep;
        }

        [FunctionName("GetToDoList")]
        public  async Task<IActionResult> GetToDoList(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            var toDoList = await _ToDoListRepositoryrep.GetAll();
            return new OkObjectResult(toDoList);
        }

        [FunctionName("GetToDoListItem")]
        public  async Task<IActionResult> GetToDoListItem(
         [HttpTrigger(AuthorizationLevel.Function, "get", Route = "todolist/{id}")] HttpRequest req,
         [FromRoute] int id,
         ILogger log)
        {
            var toDoList = await _ToDoListRepositoryrep.Get(id);
            if (toDoList == null)
            {
                return new NotFoundResult();
            }
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
            var toDoList = await _ToDoListRepositoryrep.Get(id);
            if (toDoList == null)
            {
                return new NotFoundResult();
            }

            toDoList.Description = updatedToDoList.Description;
            toDoList.IsCompleted = updatedToDoList.IsCompleted;

            _ToDoListRepositoryrep.Update(toDoList);
            await _ToDoListRepositoryrep.SaveChanges();

            return new OkObjectResult(toDoList);
        }
        [FunctionName("CreateToDoListItem")]
        public  async Task<IActionResult> CreateToDoListItem(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "todolist")] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var newToDoList = JsonConvert.DeserializeObject<ToDoList>(requestBody);
            await _ToDoListRepositoryrep.Add(newToDoList);
            await _ToDoListRepositoryrep.SaveChanges();
            return new OkObjectResult(newToDoList);
        }



        [FunctionName("DeleteToDoListItem")]
        public  async Task<IActionResult> DeleteToDoListItem(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "todolist/{id}")] HttpRequest req,
             ILogger log,
            [FromRoute] int id)
        {
            var toDoList = await _ToDoListRepositoryrep.Get(id);
            if (toDoList == null)
            {
                return new NotFoundResult();
            }
            await _ToDoListRepositoryrep.Delete(id);
            await _ToDoListRepositoryrep.SaveChanges();
            return new OkResult();
        }
        //api dipendenti=======================

        [FunctionName("GetAllDipendenti")]
        public  async Task<IActionResult> GetAllDipendenti(
         [HttpTrigger(AuthorizationLevel.Function, "get", Route = "dipendenti")] HttpRequest req,
             ILogger log)
        {
            var Dipendenti = _Dependentrep.GetAllDependents();
            return new OkObjectResult(Dipendenti);
        }

        [FunctionName("CreateDipendente")]
        public  async Task<IActionResult> CreateDipendente(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "createdipendente")] HttpRequest req,
        ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var newDipendente = JsonConvert.DeserializeObject<Dipendenti>(requestBody);
            var existingDipendente = _Dependentrep.GetAllDependents().FirstOrDefault(x => x.Name == newDipendente.Name);
            if (existingDipendente != null)
            {
                return new BadRequestObjectResult("Esiste già un dipendente con lo stesso nome");
            }

            await _Dependentrep.Add(newDipendente);
            await _Dependentrep.SaveChanges();
            return new OkObjectResult(newDipendente);
        }
        [FunctionName("UpdateDipendente")]
        public  async Task<IActionResult> UpdateDipendente(
        [HttpTrigger(AuthorizationLevel.Function, "put", Route = "dipendente/{Id}")] HttpRequest req,
        [FromRoute] int id,
            ILogger log)
        {
            var Dipendente = await _Dependentrep.Get(id);
            if (Dipendente == null)
            {
                return new NotFoundResult();
            }
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var updatedDipendente = JsonConvert.DeserializeObject<Dipendenti>(requestBody);

            Dipendente.Name = updatedDipendente.Name;

            _Dependentrep.Update(Dipendente);
            await _Dependentrep.SaveChanges();

            return new OkObjectResult(Dipendente);
        }
            [FunctionName("GetDependent")]
            public  async Task<IActionResult> GetDependent(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "dependent/{id}")] HttpRequest req,
            [FromRoute] int id,
                ILogger log)
            {
            var toDoList = await _Dependentrep.Get(id);
            if (toDoList == null)
            {
                return new NotFoundResult();
            }
            return new OkObjectResult(toDoList);
            }


        [FunctionName("DeleteDipendente")]
        public  async Task<IActionResult> DeleteDipendente(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "DeleteDipendente/{id}")] HttpRequest req, ILogger log, int id)
        {
            var dipendente = await _Dependentrep.Get(id);

            if (dipendente == null)
            {
                return new NotFoundResult();
            }

            await _Dependentrep.Delete(id);

            await _Dependentrep.SaveChanges();
            return new OkResult();
        }
    }
}
