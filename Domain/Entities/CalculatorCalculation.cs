using Domain.Commons;

namespace Domain.Entities
{
    public class CalculatorCalculation : BaseEntity
    {
        public double FirstValue { get; set; }
        public double? SecondValue { get; set; }
        public CalculatorOperator Operator { get; set; }
        public double Result { get; set; }
    }
}
