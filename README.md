# CMCS.Mvc — Contract Monthly Claim System (UI Prototype, MVC)

**Assignment Fit**
- MVC (.NET 8) web project with *non-functional* UI (no data saved).
- Dummy/seeded data only; buttons show static JS messages.
- Clear navigation for Submit Claim, Claim Status, Admin Review.

## Run
1. Open `CMCS.Mvc.csproj` in Visual Studio 2022 (or `dotnet run`).
2. Start the app and visit:
   - `/Claims/Create` → Submit Claim (alert only)
   - `/Claims` → Claim Status (seeded claims)
   - `/Claims/Review` → Admin Review (alerts only)

## Part 2
Replace DummyDataProvider with EF Core repository and add POST actions.
