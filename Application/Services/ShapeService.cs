using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Shared.Enums;

namespace Application.Services;

public class ShapeService : IShapeService
{
    private readonly IShapeRepository _repo;
    private readonly IMapper _mapper;

    public ShapeService(IShapeRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<ShapeCalculationDTO> CreateAsync(CreateShapeCommand cmd, CancellationToken ct = default)
    {
        // 1) Kontroll: cmd.Parameters får inte vara null eller tom
        if (cmd.Parameters == null || !cmd.Parameters.Any())
        {
            throw new ArgumentException("At least one ParameterDto must be provided.");
        }

        // 2) Bygg upp en dictionary av ParameterType → värde
        var paramDict = new Dictionary<ParameterType, double>();
        foreach (var p in cmd.Parameters)
        {
            if (paramDict.ContainsKey(p.ParameterType))
            {
                throw new ArgumentException($"Duplicate parameter: {p.ParameterType}");
            }
            paramDict[p.ParameterType] = p.Value;
        }

        // 3) Validera att nödvändiga ParameterType finns för vald ShapeType
        void ValidatePresence(ParameterType key)
        {
            if (!paramDict.ContainsKey(key))
            {
                throw new ArgumentException($"Missing required parameter: {key}");
            }
        }

        switch (cmd.ShapeType)
        {
            case ShapeType.Rectangle:
                ValidatePresence(ParameterType.Width);
                ValidatePresence(ParameterType.Height);
                break;

            case ShapeType.Parallelogram:
                ValidatePresence(ParameterType.Base);
                ValidatePresence(ParameterType.SideA);
                ValidatePresence(ParameterType.Height);
                break;

            case ShapeType.Triangle:
                ValidatePresence(ParameterType.SideA);
                ValidatePresence(ParameterType.Base);
                ValidatePresence(ParameterType.SideC);
                ValidatePresence(ParameterType.Height);
                break;

            case ShapeType.Rhombus:
                ValidatePresence(ParameterType.Diagonal1);
                ValidatePresence(ParameterType.Diagonal2);
                ValidatePresence(ParameterType.SideA);
                break;

            default:
                throw new InvalidOperationException($"Unsupported ShapeType: {cmd.ShapeType}");
        }

        // 4) Beräkna area och perimeter med en vanlig switch-sats:
        double area, peri;
        switch (cmd.ShapeType)
        {
            case ShapeType.Rectangle:
                area = paramDict[ParameterType.Width] * paramDict[ParameterType.Height];
                peri = 2 * (paramDict[ParameterType.Width] + paramDict[ParameterType.Height]);
                break;

            case ShapeType.Parallelogram:
                {
                    double b = paramDict[ParameterType.Base];
                    double s = paramDict[ParameterType.SideA];
                    double h = paramDict[ParameterType.Height];
                    area = b * h;
                    peri = 2 * (b + s);
                }
                break;

            case ShapeType.Triangle:
                {
                    double sideA = paramDict[ParameterType.SideA];
                    double bas = paramDict[ParameterType.Base];
                    double sideC = paramDict[ParameterType.SideC];
                    double height = paramDict[ParameterType.Height];
                    // Omkrets = alla tre sidor
                    peri = sideA + bas + sideC;

                    // Area = bas × höjd / 2
                    area = (bas * height) / 2.0;
                }
                break;

            case ShapeType.Rhombus:
                {
                    double d1 = paramDict[ParameterType.Diagonal1];
                    double d2 = paramDict[ParameterType.Diagonal2];
                    double side = paramDict[ParameterType.SideA];
                    area = (d1 * d2) / 2.0;
                    peri = 4 * side;
                }
                break;

            default:
                throw new InvalidOperationException("Unsupported ShapeType.");
        }

        // Runda av till två decimaler
        area = Math.Round(area, 2);
        peri = Math.Round(peri, 2);

        // 5) Skapa entitet …
        var entity = new ShapeCalculation
        {
            ShapeType = cmd.ShapeType,
            Area = area,
            Perimeter = peri
        };

        // … lägg på parametrar och spara …
        foreach (var p in cmd.Parameters)
        {
            entity.ShapeParameters.Add(new ShapeParameter
            {
                ParameterType = p.ParameterType,
                Value = p.Value
            });
        }

        var saved = await _repo.AddAsync(entity, ct);
        return _mapper.Map<ShapeCalculationDTO>(saved);
    }


    public async Task<IReadOnlyList<ShapeCalculationDTO>> GetAllAsync(CancellationToken ct = default)
    {
        var list = await _repo.GetAllAsync(ct);
        return list.Select(e => _mapper.Map<ShapeCalculationDTO>(e)).ToList();
    }

    public async Task<ShapeCalculationDTO?> GetByIdAsyc(int id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(id, ct);
        return entity is null ? null : _mapper.Map<ShapeCalculationDTO>(entity);
    }

    public async Task UpdateAsync(UpdateShapeCommand cmd, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(cmd.Id, ct);
        if (entity is null)
            throw new KeyNotFoundException($"ShapeCalculation med Id {cmd.Id} hittades inte.");

        if(cmd.Parameters == null || !cmd.Parameters.Any())
            throw new ArgumentException("At least one ParameterDTO must be provided.");

        var paramDict = new Dictionary<ParameterType, double>();
        foreach (var p in cmd.Parameters)
        {
            if (paramDict.ContainsKey(p.ParameterType))
                throw new ArgumentException($"Duplicate parameter: {p.ParameterType}");
            paramDict[p.ParameterType] = p.Value;
        }

        void ValidatePresence(ParameterType key)
        {
            if (!paramDict.ContainsKey(key))
                throw new ArgumentException($"Missing required parameter: {key}");
        }

        switch (cmd.ShapeType)
        {
            case ShapeType.Rectangle:
                ValidatePresence(ParameterType.Width);
                ValidatePresence(ParameterType.Height);
                break;

            case ShapeType.Parallelogram:
                ValidatePresence(ParameterType.SideA);
                ValidatePresence(ParameterType.SideB);
                ValidatePresence(ParameterType.Height);
                break;

            case ShapeType.Triangle:
                ValidatePresence(ParameterType.SideA);
                ValidatePresence(ParameterType.Base);
                ValidatePresence(ParameterType.SideC);
                ValidatePresence(ParameterType.Height);
                break;

            case ShapeType.Rhombus:
                ValidatePresence(ParameterType.Diagonal1);
                ValidatePresence(ParameterType.Diagonal2);
                ValidatePresence(ParameterType.SideA);
                break;

            default:
                throw new InvalidOperationException($"Unsupported ShapeType: {cmd.ShapeType}");
        }

        double area, peri;
        switch (cmd.ShapeType)
        {
            case ShapeType.Rectangle:
                {
                    double w = paramDict[ParameterType.Width];
                    double h = paramDict[ParameterType.Height];
                    area = w * h;
                    peri = 2 * (w + h);
                }
                break;

            case ShapeType.Parallelogram:
                {
                    double sideA = paramDict[ParameterType.SideA];
                    double sideB = paramDict[ParameterType.SideB];
                    double height = paramDict[ParameterType.Height];
                    area = sideA * height;                // bas = sideA, höjd = height
                    peri = 2 * (sideA + sideB);
                }
                break;

            case ShapeType.Triangle:
                {
                    double sideA = paramDict[ParameterType.SideA];
                    double bas = paramDict[ParameterType.Base];
                    double sideC = paramDict[ParameterType.SideC];
                    double height = paramDict[ParameterType.Height];
                    peri = sideA + bas + sideC;
                    area = (bas * height) / 2.0;
                }
                break;

            case ShapeType.Rhombus:
                {
                    double d1 = paramDict[ParameterType.Diagonal1];
                    double d2 = paramDict[ParameterType.Diagonal2];
                    double side = paramDict[ParameterType.SideA];
                    area = (d1 * d2) / 2.0;
                    peri = 4 * side;
                }
                break;

            default:
                throw new InvalidOperationException("Unsupported ShapeType.");
        }

        area = Math.Round(area, 2);
        peri = Math.Round(peri, 2);

        entity.ShapeType = cmd.ShapeType;
        entity.Area = area;
        entity.Perimeter = peri;

        entity.ShapeParameters.Clear();
        foreach (var p in cmd.Parameters)
        {
            entity.ShapeParameters.Add(new ShapeParameter
            {
                ParameterType = p.ParameterType,
                Value = p.Value
            });
        }

        await _repo.UpdateAsync(entity, ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        _repo.DeleteAsync(id, ct);
    }


}
