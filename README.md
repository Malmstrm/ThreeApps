ThreeApps – komplett .NET 9-svit

Shapes • Calculator • Rock Paper Scissors

En konsolsvit som visar ren arkitektur, EF Core (Code-First), Autofac för DI och Spectre.Console för ett trevligt terminal-UI. Stöd för CRUD, modern input-validering och Esc/Cancel i alla menyer.

Innehåll

Funktioner i korthet

Förkrav

Katalogstruktur

Kom-igång

Databas & seed

Bygga & köra

Använda apparna

Arkitektur & teknikval

Viktiga designmönster

Felsökning

Funktioner i korthet

✅ .NET 9 konsolappar (3 st) under en lösning

✅ EF Core (Code-First) med auto-migrering och seed-data

✅ Clean Architecture (Domain → Application → Infrastructure → UI)

✅ Autofac för beroendeinjektion

✅ Spectre.Console: färg, tabeller, menyer

✅ CRUD, robust validering och Esc/Cancel i alla steg

Förkrav

.NET SDK 9.0

SQL Server (LocalDB/Express funkar fint)

Git (för klon & commits)

Windows Terminal (valfritt men snygg ANSI-färg gör livet gladare)

Katalogstruktur

ThreeApps/
├─ ThreeApps.sln
│
├─ Domain/                         # Endast POCO-entiteter + BaseEntity
│  └─ Entities/                    # RpsGame, ShapeCalculation, CalculatorCalculation, ...
│
├─ Shared/                         # Enums, interfaces, NavigationService, helpers
│
├─ Application/                    # DTOs, Commands, MappingProfiles, Services
│  ├─ Interfaces/                  # IRpsService, IShapeService, ICalculatorService, ...
│  └─ Services/                    # Affärslogik (Strategy + Guard Clauses)
│
├─ Infrastructure/
│  ├─ Data/                        # AppDbContext, DataInitializer, Migrations
│  └─ Repositories/                # Generisk repo + RpsRepository, ShapeRepository, ...
│
├─ MainMenu/                       # Startprojekt (Autofac-bootstrapping, MenuRunner)
├─ RPS/                            # Rock Paper Scissors-runner
├─ Shape/                          # Shape-runner
└─ Calculator/                     # Calculator-runner

Kom-igång

1. Kloning
   git clone https://github.com/<org-eller-användare>/ThreeApps.git
   cd ThreeApps
   
2. Anslutningssträng
Öppna MainMenu/appsettings.json och uppdatera vid behov:
{
  "ConnectionStrings": {
    "Default": "Server=.;Database=ThreeAppsDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}

Databas & seed

Migrationer finns under Infrastructure/Data/Migrations.

Vid första uppstart körs automatiskt:
db.Database.Migrate();       // skapar/uppgraderar schema
DataInitializer.Seed(db);    // lägger in 4 Shapes + 6 Calculator-poster

Använda apparna
1) Rock Paper Scissors (RPS)

Play: välj Rock/Paper/Scissors → resultat sparas → resultatpanel visas.

History: paginerad tabell med datum, drag, utfall samt kumulativ Win/Loss/Tie.

Esc: tillbaka till Main Menu.

2) Shape Calculator

Stöd för Rectangle, Parallelogram, Triangle (A, B, C, Height) och Rhombus.

Val	Beskrivning
New Calculation	Välj form → mata in dimensioner → Area & Perimeter beräknas och sparas.
List All	Tabell med Id, Datum, Form, Area, Perimeter och parametrar.
Update	Ange Id → redigera valfria fält (tom input = behåll).
Delete	Ange Id → bekräfta borttag.
Esc	Tillbaka.
3) Calculator

Operatorer: + − * / √ %

Val	Beskrivning
New Calculation	Tal A → operator → Tal B (om behövs). Division/Modulus by 0 stoppas och ber om korrigering.
List All	Historik i tabell.
Update	Ange Id → ändra A, operator, B (guards mot bokstäver & noll-division).
Delete	Ange Id → bekräfta.
Esc	Tillbaka.

💡 Tips: Skriv cancel på valfri prompt för att avbryta och gå tillbaka.

Arkitektur & teknikval
Lager & referenser
Lager	Syfte	Referenser
Domain	Kärna: entiteter, regler nära datat	—
Application	DTOs, mapping, tjänster/affärslogik	Domain, Shared
Infrastructure	Databas, repos, EF Core	Domain, Application
UI (MainMenu, Shape, Calculator, RPS)	Presentation (Console)	Application, Shared, Infrastructure (via DI)
Paket
Paket	Version	Användning
Microsoft.EntityFrameworkCore	9.0.5	ORM
Microsoft.EntityFrameworkCore.SqlServer	9.0.5	SQL Server-provider
Autofac.Extensions.DependencyInjection	9.0.0	DI-container
Spectre.Console	0.47	Färgrikt CLI-UI
AutoMapper	13.x	Entitet ⇆ DTO-mapping

(Versionerna speglar nuvarande projekt; uppgradera fritt när du vill leva farligt.)

Viktiga designmönster

Repository (Infrastructure/Repositories/*Repository.cs)
Isolerar datalager från applikationslogik.

Strategy (Application/Services)
Väljer beräkningsstrategi utifrån ShapeType/CalculatorOperator.

Dependency Injection (registreras i MainMenu/Program.cs)
Autofac bygger container och registrerar NavigationService, Services & Repositories.

Guard Clauses
TryReadDouble, noll-divisionsskydd, null/empty-checks – fel stoppas tidigt.
