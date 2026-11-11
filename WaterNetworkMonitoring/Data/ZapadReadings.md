# Zapad sample readings

This is a synthetic October 2026 dataset for the existing 67 Zapad building markers.
It is not utility telemetry, an official network layout, or evidence of an actual leak.

## Set up the database

The migration `20261110230614_AddZapadOctober2026Readings` is already included.
Close the running application, choose WaterNetworkMonitoring as the default project in
Visual Studio's Package Manager Console, and run:

```powershell
EntityFrameworkCore\Update-Database
```

Do not add another migration for this dataset. EF applies the existing migrations in order
and records them in `__EFMigrationsHistory`; running Update-Database again does not insert
another copy. This also works on an empty database. Application startup does not apply
migrations automatically.

The default SQL Server remains `DenislavPC\MSSQLSERVER01`, database `WaterNetworkDB`.
Someone using another computer must supply their own SQL Server connection, for example
in Package Manager Console before running Update-Database:

```powershell
$env:WATERNETWORK_CONNECTION_STRING = 'Server=YOUR_SERVER;Database=WaterNetworkDB;Trusted_Connection=True;TrustServerCertificate=True;'
EntityFrameworkCore\Update-Database
```

Set the same environment variable for the application process when running it. No password
or machine-specific replacement connection string needs to be committed to source control.

## Tables and units

| Table | Contents |
| --- | --- |
| WaterMeters | Existing six-digit device IDs and map numbers; latest counter/date |
| Loggers | Zapad inlet 200001 and Skaptopara supply branch 200002; explicit parent and supplied-meter links |
| LoggerReadings | Flow (m³/hour, average over the preceding 15 minutes), pressure (bar), timestamp, simulated flag |
| WaterMeterReadings | Meter ID, eight-digit cumulative counter in **litres**, timestamp, simulated flag |

The measured amount between two meter readings is the difference between their counters.
A displayed `00001234` means 1,234 cumulative litres, not 1,234 m³ and not that interval's usage.
Logger interval volume in m³ is `FlowM3PerHour * 0.25`. Do not sum both inlet and branch flow:
the branch is already included in the inlet.

Each meter has two readings per local day, on either 02:00/14:00 or 06:00/18:00.
Skaptopara is map number **57**, meter **100058**, OSM way **992665907**, on 06:00/18:00.
There are **4,154 October meter readings** and **5,960 October logger readings**.
Pre/post-month endpoints allow full balance windows across month boundaries. An additional
8 November 22:00 reading per meter keeps the original latest-reading date consistent.
Totals including these endpoints are **4,355 meter readings** and **6,104 logger readings**.

Timestamps retain Bulgaria's UTC offset. On 25 October there are 100 quarter-hour logger
readings per device because the clock goes back. Local clock schedules are preserved;
the 02:00–14:00 window that day lasts 13 elapsed hours. Calculations use the actual duration.

The migration replaces a current meter counter only when it is still the original
`00000000` placeholder dated 8 November 2026 22:00+02:00. It does not overwrite other current
values, delete accounts, or change surviving meter IDs. Known older map numbers are repaired as described below. Rolling back this
migration drops its three new tables; current meter counters are retained.

If the old AddZapadWaterMeters migration was applied before its numbering was rewritten,
the new migration first repairs the known city-wide and 1–68 Zapad numbering by device ID.
Existing counters/dates are retained, missing prototype devices are restored, and excluded
meter 100052 is removed only if it still has its original empty prototype reading/date.
The unique map-number index is recreated in the same transaction to avoid collisions while
renumbering. Unrecognized mappings, unrelated meters occupying target numbers, or non-prototype
data on the excluded meter stop the import without losing data. Corrected map numbers are
retained on rollback. Update-Database does not replay the earlier migration.

## How the sample is generated

`ZapadOctober2026Seed.cs` contains the fixed C# building/device mapping, occupancy assumptions,
daily profiles and repeatable variation. It generates one quarter-hour consumption timeline
and derives both logger and meter readings from it. It does not load readings from JSON or
download data during migration. The map's OSM JSON remains map geometry only.

Residential demand starts at **149 litres/person/day**, the NSI's 2024 Blagoevgrad **district**
household average, not a measured 2026 rate for these buildings. Occupancy is invented for the
demo (24–120 people per apartment footprint, 4–12 for unspecified small buildings).
The sports hall, clinic, market and service demand assumptions are respectively 4,000,
2,400, 900 and 100 litres/day, with day-to-day variation; a roof footprint has zero demand.
Morning/evening residential peaks and daytime facility activity distribute these volumes.
OSM footprints do not prove actual water meter installations or resident counts.

The simulated leak starts **30 October 2026 06:00+02:00** upstream of Skaptopara's meter:
125 litres per quarter hour (500 litres/hour) enter the branch without entering that meter.
Both upstream loggers include this lost water. The pressure response is an assumed demo
decrease, not a calibrated hydraulic model.

`LeakDetectionService.AnalyzeBranch` compares a branch logger against its supplied meter over
complete matching windows. A CUSUM of excess flow detects persistent imbalance; pressure
provides supporting evidence. The caller supplies a known-normal baseline period. For this
dataset, use readings through 29 October 18:00+02:00. The first complete affected window ends
30 October 18:00: supplied volume exceeds metered use by 6 m³, or 500 litres/hour. Missing
quarter-hour data invalidates a balance window. The detector never reads the planted leak
date. It identifies the supply branch, not an exact underground pipe-break position.
This helper is ready for the future report/analysis UI; map clicks still show numbers only.

This versioned seed is called by the migration and must stay frozen after release.
Use a new migration/scenario class for future data changes. History is intentionally not
expanded into thousands of HasData entries in the model snapshot.

Sources:

- [NSI Environment 2024, published 2026, pp. 27 and 96](https://www.nsi.bg/index.php/file/37559/Okolna_sreda_2024.pdf)
- [EPA water loss guidance: compare supply and metered use over matching periods](https://www.epa.gov/sites/default/files/2015-05/documents/epa816r10019.pdf)
- [NIST CUSUM control charts](https://www.itl.nist.gov/div898/handbook/pmc/section3/pmc323.htm)

## Regression checks

From the repository root:

```powershell
dotnet run --project WaterNetworkMonitoring.PrototypeChecks
```

These checks do not connect to your SQL Server. They use an isolated in-memory database to
exercise the numbering repair, including old numbering and data-conflict cases, and validate map/device identities,
migration operations, sample counts, cumulative counters, schedules, DST, volume balance,
leak detection, and handling of missing/duplicate measurements.
