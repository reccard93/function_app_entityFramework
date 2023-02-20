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
using System.Net.Http;
using System.Net;
using AutoMapper;
using AzureFunctionDTO.ResponseDTO;
using AzureFunctionDTO.RequestDTO;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;

[assembly: FunctionsStartup(typeof(function_app_entityFramework.Startup))]

namespace function_app_entityFramework
{
    public class AzureFunctionRepo
    {
        private IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AzureFunctionRepo(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [FunctionName("GetToDoList")]
        [OpenApiOperation(operationId: "GetToDoList", tags: new[] { "todolist" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<ToDoListDto>), Description = "The OK response")]
        public async Task<IActionResult> GetToDoList(
    [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
    ILogger log)
        {
            var toDoListItems = await _unitOfWork.ToDoListRepository.GetAll();
            var toDoListDto = _mapper.Map<List<ToDoListDto>>(toDoListItems);
            await _unitOfWork.Save();
            return new OkObjectResult(toDoListDto);
        }

        [FunctionName("GetToDoListItem")]
        [OpenApiOperation(operationId: "GetToDoListItem", tags: new[] { "todolist" })]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(int), Description = "The **id** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(ToDoListDto), Description = "The OK response")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.NotFound, contentType: "text/plain", bodyType: typeof(string), Description = "The Not Found response")]
        public async Task<IActionResult> GetToDoListItem(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "todolist/{id}")] HttpRequest req,
            [FromRoute] int id,
            ILogger log)
        {
            var toDoListItem = await _unitOfWork.ToDoListRepository.Get(id);
            if (toDoListItem == null)
            {
                return new NotFoundResult();
            }
            var toDoListItemDto = _mapper.Map<ToDoListDto>(toDoListItem);
            await _unitOfWork.Save();
            return new OkObjectResult(toDoListItemDto);
        }

        [FunctionName("UpdateToDoListItem")]
        [OpenApiOperation(operationId: "UpdateToDoListItem", tags: new[] { "todolist" })]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(ToDoListDto), Description = "The request body")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(ToDoListDto), Description = "The OK response")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.NotFound, contentType: "text/plain", bodyType: typeof(string), Description = "The Not Found response")]
        public async Task<IActionResult> UpdateToDoListItem(
         [HttpTrigger(AuthorizationLevel.Function, "put", Route = "todolist/{Id}")] HttpRequest req,
         [FromRoute] int id,
         ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var updatedToDoListItemDto = JsonConvert.DeserializeObject<ToDoListDto>(requestBody);

            var toDoListItem = await _unitOfWork.ToDoListRepository.Get(id);
            if (toDoListItem == null)
            {
                return new NotFoundResult();
            }

            toDoListItem.Description = updatedToDoListItemDto.Description;
            toDoListItem.IsCompleted = updatedToDoListItemDto.IsCompleted;

            await _unitOfWork.ToDoListRepository.Update(toDoListItem);
            await _unitOfWork.Save();

            var toDoListItemUpdatedDto = _mapper.Map<ToDoListDto>(toDoListItem);
            return new OkObjectResult(toDoListItemUpdatedDto);
        }

        [FunctionName("CreateToDoListItem")]
        [OpenApiOperation(operationId: "CreateToDoListItem", tags: new[] { "todolist" })]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(ToDoListDto), Description = "The request body")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(ToDoListDto), Description = "The OK response")]
        public async Task<IActionResult> CreateToDoListItem(
         [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
         ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            ToDoListDto toDoListDto = JsonConvert.DeserializeObject<ToDoListDto>(requestBody);

            var toDoList = _mapper.Map<ToDoList>(toDoListDto);
            await _unitOfWork.ToDoListRepository.Add(toDoList);
            await _unitOfWork.Save();

            var toDoListDtoResponse = _mapper.Map<ToDoListDto>(toDoList);
            return new OkObjectResult(toDoListDtoResponse);
        }
        [FunctionName("DeleteToDoListItem")]
        [OpenApiOperation(operationId: "DeleteToDoListItem", tags: new[] { "todolist" })]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(int), Description = "The ID of the item to delete.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> DeleteToDoListItem(
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
            return new OkObjectResult("L'item è stato cancellato con successo ");
        }
        //api dipendenti=======================

        [FunctionName("GetAllDipendenti")]
        [OpenApiOperation(operationId: "GetAllDipendenti", tags: new[] { "Dipendenti" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(IEnumerable<Dipendenti>), Description = "The list of dipendenti")]
        public async Task<IActionResult> GetAllDipendenti(
         [HttpTrigger(AuthorizationLevel.Function, "get", Route = "dipendenti")] HttpRequest req,
             ILogger log)
        {
            var Dipendenti = _unitOfWork.DependentsRepository.GetAllDependents();
            await _unitOfWork.Save();
            return new OkObjectResult(Dipendenti);
        }

        [FunctionName("CreateDipendente")]
        [OpenApiOperation(operationId: "CreateDipendente", tags: new[] { "Dipendenti" })]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(CreateDipendenteDto), Required = true, Description = "The dipendente to create")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(DipendenteDto), Description = "The created dipendente")]
        public async Task<IActionResult> CreateDipendente(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "createdipendente")]
            [FromBody] CreateDipendenteDto createDipendenteDto,
            ILogger log)
        {
            var existingDipendente = await _unitOfWork.DependentsRepository.GetByName(createDipendenteDto.Name);
            if (existingDipendente != null)
            {
                return new BadRequestObjectResult("Esiste già un dipendente con lo stesso nome");
            }

            var newDipendente = new Dipendenti
            {
                Name = createDipendenteDto.Name
            };

            await _unitOfWork.DependentsRepository.Add(newDipendente);
            await _unitOfWork.Save();

            var dipendenteDto = new DipendenteDto
            {
                Name = newDipendente.Name
            };

            return new OkObjectResult(dipendenteDto);
        }


        [FunctionName("UpdateDipendente")]
        [OpenApiOperation(operationId: "UpdateDipendente", tags: new[] { "Dipendenti" })]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(int), Description = "The ID of the dipendente to update")]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(DipendenteDto), Required = true, Description = "The updated dipendente")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Dipendenti), Description = "The updated dipendente")]
        public async Task<IActionResult> UpdateDipendente(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "dipendenti/{id}")]
            [FromBody] DipendenteDto dipendenteDto,
            [FromRoute] int id,
            ILogger log)
        {
            var dipendente = await _unitOfWork.DependentsRepository.Get(id);
            if (dipendente == null)
            {
                return new NotFoundResult();
            }

            dipendente.Name = dipendenteDto.Name;

            await _unitOfWork.DependentsRepository.Update(dipendente);
            await _unitOfWork.Save();

            return new OkObjectResult(dipendente);
        }

        [FunctionName("GetDependent")]
        [OpenApiOperation(operationId: "GetDependent", tags: new[] { "Dipendenti" })]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(int), Description = "The ID of the dipendente to get")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(DipendenteDto), Description = "The dipendente details")]
        public async Task<IActionResult> GetDependent(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "dependent/{id}")] HttpRequest req,
            [FromRoute] int id,
            ILogger log)
        {
            var dipendente = await _unitOfWork.DependentsRepository.Get(id);
            if (dipendente == null)
            {
                return new NotFoundResult();
            }

            var dipendenteDto = new DipendenteDto
            {
                Name = dipendente.Name,
                // assegnare gli altri campi del DTO a seconda delle necessità
            };

            return new OkObjectResult(dipendenteDto);
        }


        [FunctionName("DeleteDipendente")]
        [OpenApiOperation(operationId: "DeleteDipendente", tags: new[] { "Dipendenti" })]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(int), Description = "The ID of the dipendente to delete.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.NotFound, contentType: "application/json", bodyType: typeof(string), Description = "The Not Found response")]
        public async Task<IActionResult> DeleteDipendente(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "DeleteDipendente/{id}")]
            HttpRequest req,
            ILogger log,
            int id)
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
