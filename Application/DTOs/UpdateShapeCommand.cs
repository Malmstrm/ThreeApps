using Shared.Enums;

namespace Application.DTOs;

public record UpdateShapeCommand(int Id, ShapeType ShapeType, IEnumerable<ParameterDTO> Parameters);
