using System.Net.Mime;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Commands.Classroom;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Exceptions;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Queries;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Services;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Interfaces.REST.Resources.Classroom;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Interfaces.REST.Transform.Classroom;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Interfaces.REST;

[ApiController]
[Authorize]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Available Classroom Endpoints")]
public class ClassroomsController(
    IClassroomQueryService classroomQueryService,
    IClassroomCommandService classroomCommandService) : ControllerBase
{
    [HttpGet("{id:int}")]
    [SwaggerOperation(Summary = "Get a classroom by id", OperationId = "GetClassroomById")]
    [SwaggerResponse(StatusCodes.Status200OK, "The classroom was successfully retrieved", typeof(ClassroomResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "The classroom was not found")]
    public async Task<IActionResult> GetClassroomById(int id)
    {
        var query = new GetClassroomByIdQuery(id);
        var classroom = await classroomQueryService.Handle(query);
        if (classroom is null) return NotFound();
        return Ok(ClassroomResourceFromEntityAssembler.ToResourceFromEntity(classroom));
    }

    [HttpPost("teachers/{teacherId:int}")]
    [SwaggerOperation(Summary = "Create a classroom with a teacher in charge", OperationId = "CreateClassroom")]
    [SwaggerResponse(StatusCodes.Status201Created, "The classroom was successfully created", typeof(ClassroomResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "The classroom was not created")]
    public async Task<IActionResult> CreateClassroom(int teacherId, [FromBody] CreateClassroomResource resource)
    {
        var command = CreateClassroomCommandFromResourceAssembler.ToCommandFromResource(teacherId, resource);
        var classroom = await classroomCommandService.Handle(command);
        if (classroom is null) return BadRequest();
        var classroomResource = ClassroomResourceFromEntityAssembler.ToResourceFromEntity(classroom);
        return CreatedAtAction(nameof(GetClassroomById), new { id = classroom.Id }, classroomResource);
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Get all classrooms", OperationId = "GetAllClassrooms")]
    [SwaggerResponse(StatusCodes.Status200OK, "The classrooms were successfully retrieved",
        typeof(IEnumerable<ClassroomResource>))]
    public async Task<IActionResult> GetAllClassrooms()
    {
        var classrooms = await classroomQueryService.Handle(new GetAllClassroomsQuery());
        return Ok(classrooms.Select(ClassroomResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpGet("teachers/{teacherId:int}")]
    [SwaggerOperation(Summary = "Get classrooms by teacher ID", OperationId = "GetClassroomsByTeacherId")]
    [SwaggerResponse(StatusCodes.Status200OK, "Classrooms retrieved successfully",
        typeof(IEnumerable<ClassroomResource>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "No classrooms found for the given teacher ID")]
    public async Task<IActionResult> GetClassroomsByTeacherId(int teacherId)
    {
        var classrooms = await classroomQueryService.Handle(new GetAllClassroomsByTeacherIdQuery(teacherId));
        return Ok(classrooms.Select(ClassroomResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpPut("{id:int}")]
    [SwaggerOperation(Summary = "Update a classroom", OperationId = "UpdateClassroom")]
    [SwaggerResponse(200, "The classroom was updated successfully", typeof(ClassroomResource))]
    [SwaggerResponse(404, "The classroom was not found")]
    public async Task<IActionResult> UpdateClassroom([FromRoute] int id, [FromBody] UpdateClassroomResource resource)
    {
        try
        {
            // Map the resource to the UpdateClassroomCommand
            var command = UpdateClassroomCommandFromResourceAssembler.ToCommandFromResource(id, resource);
            var updatedClassroom = await classroomCommandService.Handle(command);
            if (updatedClassroom is null) return NotFound(new { Message = "Classroom not found." });
            return Ok(ClassroomResourceFromEntityAssembler.ToResourceFromEntity(updatedClassroom));
        }
        catch (ClassroomNotFoundException ex)
        {
            return NotFound(new { ex.Message });
        }
        catch (TeacherNotFoundForClassroomException ex)
        {
            return BadRequest(new { ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    [SwaggerOperation(Summary = "Delete a classroom", OperationId = "DeleteClassroom")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "The classroom was deleted successfully.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Classroom not found.")]
    public async Task<IActionResult> DeleteClassroom([FromRoute] int id)
    {
        try
        {
            await classroomCommandService.Handle(new DeleteClassroomCommand(id));
            return NoContent();
        }
        catch (ClassroomNotFoundException ex)
        {
            return NotFound(new { ex.Message });
        }
    }
}
