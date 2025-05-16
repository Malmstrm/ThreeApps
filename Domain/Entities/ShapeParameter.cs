using Domain.Commons;

namespace Domain.Entities
{
    public class ShapeParameter : BaseEntity
    {
        public int ShapeCalculationId { get; set; }
        public ParameterType ParameterType { get; set; }
        public double Value { get; set; }

        public ShapeCalculation ShapeCalculation { get; set; } = null!;
    }
}