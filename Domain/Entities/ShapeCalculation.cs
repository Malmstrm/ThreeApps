using Domain.Commons;
using Shared.Enums;

namespace Domain.Entities
{
    public class ShapeCalculation : BaseEntity
    {
        public ShapeType ShapeType { get; set; }
        public double Area { get; set; }
        public double Perimeter { get; set; }
        public ICollection<ShapeParameter> ShapeParameters { get; set; } = new List<ShapeParameter>();
    }
}
