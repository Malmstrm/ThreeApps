using Shared.Enums;

namespace Application.DTOs
{
    public record CreateShapeCommand(ShapeType ShapeType, IEnumerable<ParameterDTO> Parameters);
}
