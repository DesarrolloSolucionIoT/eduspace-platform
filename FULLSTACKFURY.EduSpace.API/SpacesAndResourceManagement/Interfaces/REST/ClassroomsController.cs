using System.Net.Mime;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Commands.Classroom;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Model.Queries;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Domain.Services;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Interfaces.REST.Resources.Classroom;
using FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Interfaces.REST.Transform.Classroom;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FULLSTACKFURY.EduSpace.API.SpacesAndResourceManagement.Interfaces.REST;

[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Available Classroom Endpoints")]
public class ClassroomsController(
    IClassroomQueryService classroomQueryService,
    IClassroomCommandService classroomCommandService) : ControllerBase
{
    /// <summary>
    ///     Get classroom by id
    /// </summary>
    /// <param name="id">
    ///     The classroom id to get
    /// </param>
    /// <returns>
    ///     The <see cref="ClassroomResource" /> classroom if found, otherwise returns <see cref="NotFoundResult" />
    /// </returns>
    [HttpGet("{id:int}")]
    [SwaggerOperation(
        Summary = "Get a classroom by id",
        Description = "Get a classroom by id",
        OperationId = "GetClassroomById"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "The classroom was successfully retrieved", typeof(ClassroomResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "The classroom was not found")]
    public async Task<IActionResult> GetClassroomById(int id)
    {
        var getClassroomByIdQuery = new GetClassroomByIdQuery(id);
        var classroom = await classroomQueryService.Handle(getClassroomByIdQuery);
        if (classroom is null) return NotFound();
        var classroomResource = ClassroomResourceFromEntityAssembler.ToResourceFromEntity(classroom);
        return Ok(classroomResource);
    }

    /// <summary>
    ///     Create a classroom
    /// </summary>
    /// <param name="resource">
    ///     The <see cref="CreateClassroomResource" /> to create the classroom from
    /// </param>
    /// <returns>
    ///     The <see cref="ClassroomResource" /> classroom if created, otherwise returns <see cref="BadRequestResult" />
    /// </returns>
    [HttpPost("teachers/{teacherId:int}")]
    [SwaggerOperation(
        Summary = "Create a classroom with a teacher in charge",
        Description = "Create a classroom",
        OperationId = "CreateClassroom"
    )]
    [SwaggerResponse(StatusCodes.Status201Created, "The classroom was successfully created", typeof(ClassroomResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "The classroom was not created")]
    public async Task<IActionResult> CreateClassroom(int teacherId, [FromBody] CreateClassroomResource resource)
    {
        var createClassroomCommand =
            CreateClassroomCommandFromResourceAssembler.ToCommandFromResource(teacherId, resource);
        var classroom = await classroomCommandService.Handle(createClassroomCommand);
        if (classroom is null) return BadRequest();
        var classroomResource = ClassroomResourceFromEntityAssembler.ToResourceFromEntity(classroom);
        return CreatedAtAction(nameof(GetClassroomById), new { id = classroom.Id }, classroomResource);
    }

    /// <summary>
    ///     Get all classrooms
    /// </summary>
    /// <returns>
    ///     The list of <see cref="ClassroomResource" /> classrooms
    /// </returns>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Get all classrooms",
        Description = "Get all classrooms",
        OperationId = "GetAllClassrooms"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "The classrooms were successfully retrieved",
        typeof(IEnumerable<ClassroomResource>))]
    public async Task<IActionResult> GetAllClassrooms()
    {
        var getAllClassroomsQuery = new GetAllClassroomsQuery();
        var classrooms = await classroomQueryService.Handle(getAllClassroomsQuery);
        var classroomResources = classrooms.Select(ClassroomResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(classroomResources);
    }

    [HttpGet("teachers/{teacherId:int}")]
    [SwaggerOperation(
        Summary = "Get classrooms by teacher ID",
        Description = "Get classrooms by teacher ID",
        OperationId = "GetClassroomsByTeacherId"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Classrooms retrieved successfully",
        typeof(IEnumerable<ClassroomResource>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "No classrooms found for the given teacher ID")]
    public async Task<IActionResult> GetClassroomsByTeacherId(int teacherId)
    {
        var getClassroomByTeacherIdQuery = new GetAllClassroomsByTeacherIdQuery(teacherId);
        var classrooms = await classroomQueryService.Handle(getClassroomByTeacherIdQuery);
        var classroomResources = classrooms.Select(ClassroomResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(classroomResources);
    }

    [HttpPut("{id:int}")]
    [SwaggerOperation(
        Summary = "Updates a classroom",
        Description = "Updates a classroom by its ID with the provided details",
        OperationId = "UpdateClassroom"
    )]
    [SwaggerResponse(200, "The classroom was updated successfully", typeof(ClassroomResource))]
    [SwaggerResponse(404, "The classroom was not found")]
    public async Task<IActionResult> UpdateClassroom([FromRoute] int id, [FromBody] UpdateClassroomResource resource)
    {
        try
        {
            // Map the resource to the UpdateMeetingCommand
            var updateClassroomCommand =
                UpdateClassroomCommandFromResourceAssembler.ToCommandFromResource(id, resource);

            // Handle the update command
            var updatedClassroom = await classroomCommandService.Handle(updateClassroomCommand);

            // If the meeting was not updated, return not found
            if (updatedClassroom is null)
                return NotFound(new { Message = "Classroom not found." });

            // Map the updated meeting entity to the resource
            var classroomResource = ClassroomResourceFromEntityAssembler.ToResourceFromEntity(updatedClassroom);
            return Ok(classroomResource);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    [SwaggerOperation(
        Summary = "Deletes a classroom",
        Description = "Deletes the classroom specified by its ID",
        OperationId = "DeleteClassroom"
    )]
    [SwaggerResponse(200, "The classroom was deleted successfully.")]
    [SwaggerResponse(404, "Classroom not found.")]
    public async Task<IActionResult> DeleteClassroom([FromRoute] int id)
    {
        try
        {
            var deleteClassroomCommand = new DeleteClassroomCommand(id);
            await classroomCommandService.Handle(deleteClassroomCommand);
            return Ok($"Classroom with ID {id} was deleted successfully.");
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
        }
    }
}