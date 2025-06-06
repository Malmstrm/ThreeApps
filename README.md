ThreeApps – komplett .NET 9
Shapes • Calculator • Rock Paper Scissors

använder Entity Framework Core (Code-First)

följer ren arkitektur-tänk (Domain → Application → Infrastructure → UI)

injicerar beroenden med Autofac

simpel renderar färg, menyer och tabeller via Spectre.Console

stöder CRUD-operationer, modern input-validering och “Esc/Cancel” i alla menyer


Innehåll
Avsnitt	Innehåll
1. Förkrav	
2. Katalogstruktur	
3. Kom-igång	
4. Databas & seed	
5. Bygga & köra	
6. Använda apparna	
7. Arkitektur & teknikval	
8. Viktiga designmönster	

1. Förkrav

.NET SDK	9.0-
SQL Server
Git	valfri	för klon & commits
(Windows Terminal)	valfri	ger snygg ANSI-färg

2. Katalogstruktur
bash
Kopiera
Redigera
ThreeApps/
│
├── ThreeApps.sln
│
├── Domain/                 # Enbart POCO-entiteter + basklass BaseEntity
│   └── Entities/           # RPSGame, ShapeCalculation, CalculatorCalculation …
│
├── Shared/                 # Enums, interface, NavigationService, helpers
│
├── Application/            # DTOs, Commands, MappingProfiles, Services
│   ├── Interfaces/         # IRpsService, IShapeService, ICalculatorService
│   └── Services/           # business-logik (Strategy + Guard Clauses)
│
├── Infrastructure/
│   ├── Data/               # AppDbContext, DataInitializer, migrations
│   └── Repositories/       # generiska + RpsRepository, ShapeRepository …
│
├── MainMenu/               # Start-projekt, Autofac-bootstrap, MenuRunner
│
├── RPS/                    # Rock Paper Scissors-runner
├── Shape/                  # Shape-runner
└── Calculator/             # Calculator-runner

4. Kom-igång

clone https://github.com/<ditt-konto>/ThreeApps.git

# justera anslutningssträng (MainMenu/appsettings.json):
# "Server=.;Database=ThreeAppsDb;Trusted_Connection=True;TrustServerCertificate=True;"
4. Databas & seed
Migrationer ligger i Infrastructure/Data/Migrations.

Vid första uppstart körs:

db.Database.Migrate();      // skapar / uppgraderar schema
DataInitializer.Seed(db);   // lägger 4 Shapes + 6 Calculator-poster

5. Bygga & köra	

> RPS - Rock, Paper & Scissors
  Shape - Shape Calculator
  Calc - Calculator
  Exit - Exit
Navigera med ↑/↓ + Enter, eller tryck Esc för att backa.

6. Använda apparna
6.1 Rock Paper Scissors
Val	Funktion
Play	välj Rock/Paper/Scissors → spelet sparas, resultatpanel visas
History	paginerad tabell med datum, drag, utfall, kumulativt känt Win/Loss/Tie
Esc	tillbaka till Main Menu

6.2 Shape Calculator
Val	Beskrivning
New Calculation	välj form → mata in dimensioner → Area & Perimeter sparas
List All	Spectre-tabell med Id, Datum, Form, Area, Perimeter, parametrar
Update	ange Id → redigera valfria fält (tom input = behåll)
Delete	ange Id → bekräfta borttag
Esc	tillbaka

Stöd för: Rectangle, Parallelogram, Triangle (A, B, C, Height) och Rhombus.

6.3 Calculator
Val	Beskrivning
New Calculation	tal 1 → operator ( +, −, *, /, √, % ) → tal 2 (om behövs)
Division/Modulus by 0 stoppar och ber användaren korrigera.
List All	tabell med historik
Update	ange Id → ändra A, operator, B (guards mot bokstäver & noll-division)
Delete	ange Id → bekräfta
Esc	tillbaka

Skriv cancel på vilken prompt som helst för att avbryta och gå tillbaka.

7. Arkitektur & teknikval
Lager	Syfte	Referenser
Domain	Kärna (ingen beroende på andra lager)	—
Application	DTOs + affärslogik (Services)	Domain, Shared
Infrastructure	Databas, Repositories	Domain, Application
UI (MainMenu, Shape, Calculator, RPS)	Presentation/Console	Application, Shared, Infrastructure (via DI)


Paket	Version	Användning
Microsoft.EntityFrameworkCore	9.0.5	ORM
Microsoft.EntityFrameworkCore.SqlServer	9.0.5	SQL Server provider
Autofac.Extensions.DependencyInjection	9.0.0	DI-container
Spectre.Console	0.47	Färgrik CLI-UI
AutoMapper	13	Entitet ⇆ DTO-mapping

8. Viktiga designmönster
Kort beskrivning
Repository	Infrastructure/Repositories/*Repository.cs	Isolerar datalager från Application-logik
Strategy	Application/Services (olika switch beroende på ShapeType, CalculatorOperator)	Väljer beräkningsstrategi utifrån typ/­operator
Dependency Injection	Konfigureras i MainMenu/Program.cs	Autofac registrerar NavigationService, Services, Repositories
Guard Clauses	TryReadDouble, division-by-zero check, null/empty checks	Stoppar otillåtna värden tidigt
