using Domain.Entities;
using Shared.Enums;

namespace Infrastructure.Data;

public static class DataInitializer
{
    public static void Seed(AppDbContext context)
    {
        // Om databasen redan är seed:ad (minst en ShapeCalculation och en CalculatorCalculation finns), gör ingenting.
        if (context.ShapeCalculations.Any() || context.Calculations.Any())
            return;

        // ====== 1) Skapa exempeldata för Shape ======

        var shapes = new List<ShapeCalculation>();

        // --- Rektangel (Rectangle) ---
        var rect = new ShapeCalculation
        {
            ShapeType = ShapeType.Rectangle,
            // Bredd = 5, Höjd = 3 ⇒ area = 15, perimeter = 16
            Area = 5 * 3,
            Perimeter = 2 * (5 + 3),
            ShapeParameters = new List<ShapeParameter>
            {
                new ShapeParameter { ParameterType = ParameterType.Width,  Value = 5 },
                new ShapeParameter { ParameterType = ParameterType.Height, Value = 3 }
            }
        };
        shapes.Add(rect);

        // --- Parallellogram (Parallelogram) ---
        var para = new ShapeCalculation
        {
            ShapeType = ShapeType.Parallelogram,
            // Bas = 6, Sidor = 4, Höjd = 2 ⇒ area = base * height = 6*2 = 12, perimeter = 2*(6+4)=20
            Area = 6 * 2,
            Perimeter = 2 * (6 + 4),
            ShapeParameters = new List<ShapeParameter>
            {
                new ShapeParameter { ParameterType = ParameterType.Base,  Value = 6 },
                new ShapeParameter { ParameterType = ParameterType.SideA, Value = 4 }, // sida A
                new ShapeParameter { ParameterType = ParameterType.Height, Value = 2 }
            }
        };
        shapes.Add(para);

        // --- Triangel (Triangle) ---
        var tri = new ShapeCalculation
        {
            ShapeType = ShapeType.Triangle,
            // Sidor: a=3, b=4, c=5, höjd mot bas=4 ⇒ area via bas*höjd/2 = 5*4/2 = 10, perimeter = 3+4+5 = 12
            Area = (5 * 4) / 2.0,
            Perimeter = 3 + 4 + 5,
            ShapeParameters = new List<ShapeParameter>
            {
                new ShapeParameter { ParameterType = ParameterType.SideA, Value = 3 },
                new ShapeParameter { ParameterType = ParameterType.SideB, Value = 4 },
                new ShapeParameter { ParameterType = ParameterType.SideC, Value = 5 },
                new ShapeParameter { ParameterType = ParameterType.Height, Value = 4 } // höjd
            }
        };
        shapes.Add(tri);

        // --- Romboid (Rhombus) ---
        var rhomb = new ShapeCalculation
        {
            ShapeType = ShapeType.Rhombus,
            // Diagonaler: d1=6, d2=4, sida=5 ⇒ area=(d1*d2)/2=12, perimeter=4*5=20
            Area = (6 * 4) / 2.0,
            Perimeter = 4 * 5,
            ShapeParameters = new List<ShapeParameter>
            {
                new ShapeParameter { ParameterType = ParameterType.Diagonal1, Value = 6 },
                new ShapeParameter { ParameterType = ParameterType.Diagonal2, Value = 4 },
                new ShapeParameter { ParameterType = ParameterType.SideA,    Value = 5 }
            }
        };
        shapes.Add(rhomb);

        // Lägg in alla ShapeCalculation-objekt i kontexten
        context.ShapeCalculations.AddRange(shapes);



        // ====== 2) Skapa exempeldata för Calculator ======

        var calcs = new List<CalculatorCalculation>();

        // --- Addition: 2 + 3 = 5.00 ---
        calcs.Add(new CalculatorCalculation
        {
            FirstValue = 2,
            SecondValue = 3,
            Operator = CalculatorOperator.Addition,
            Result = Math.Round((double)2 + 3, 2)
        });

        // --- Subtraktion: 10 - 4 = 6.00 ---
        calcs.Add(new CalculatorCalculation
        {
            FirstValue = 10,
            SecondValue = 4,
            Operator = CalculatorOperator.Subtraction,
            Result = Math.Round((double)10 - 4, 2)
        });

        // --- Multiplikation: 3 * 5 = 15.00 ---
        calcs.Add(new CalculatorCalculation
        {
            FirstValue = 3,
            SecondValue = 5,
            Operator = CalculatorOperator.Multiplication,
            Result = Math.Round((double)3 * 5, 2)
        });

        // --- Division: 20 / 4 = 5.00 ---
        calcs.Add(new CalculatorCalculation
        {
            FirstValue = 20,
            SecondValue = 4,
            Operator = CalculatorOperator.Division,
            Result = Math.Round(20.0 / 4.0, 2)
        });

        // --- Modulus: 10 % 3 = 1.00 ---
        calcs.Add(new CalculatorCalculation
        {
            FirstValue = 10,
            SecondValue = 3,
            Operator = CalculatorOperator.Modulus,
            Result = Math.Round((double)10 % 3, 2)
        });

        // --- Kvadratroten av 16 = 4.00 ---
        calcs.Add(new CalculatorCalculation
        {
            FirstValue = 16,
            SecondValue = null,
            Operator = CalculatorOperator.SquareRoot,
            Result = Math.Round(Math.Sqrt(16), 2)
        });

        context.Calculations.AddRange(calcs);


        context.SaveChanges();
    }
}
