# IsotopesStats Refactoring Checklist

## 1. Repository & Data Layer
- [ ] **Consolidate Mapping Logic:** Replace manual mapping in `ModelMappers.cs` with a more maintainable approach (e.g., source-generated mappers or refined generic mapping).
- [x] **Optimize Aggregate Queries:** Moved in-memory LINQ aggregations to Supabase **Database Functions** (`get_player_stats_summary_all`, `get_team_stats_summary_all`) and updated repository to use RPC.
- [x] **Unify Filter Logic:** Replaced magic numbers (`-1`) with explicit `GetAll*Async` methods across all layers to improve clarity and reduce branching.
- [ ] **DTO/Model Alignment:** Audit DTOs and Domain models to ensure properties are only duplicated where strictly necessary for the separation of layers.

## 2. Service Layer & Abstraction
- [ ] **Evaluate Pass-Through Services:** Decide whether to keep `StatsService` and `AuthService` as abstractions or inject repositories directly into UI components for simplicity.
- [ ] **Generic CRUD Operations:** Implement a base repository or helper to handle standard Add/Update/Delete patterns for Players, Seasons, and Opponents.
- [ ] **Standardize Logging:** Refine the `DeleteManager` and other logging points to use a more consistent, type-safe approach for identifying entities (e.g., an `INameable` interface).

## 3. UI & Shared Components
- [x] **Extract Shared UI Logic:** Created a reusable `SeasonSelector` component with centered text and anti-layout-shift logic to replace redundant selection UI in stats pages.
- [ ] **Generic Sorting:** Refactor the large `switch` statements for sorting in `PlayerStats.razor` into a more dynamic, property-based sorting system.
- [ ] **Loading State Consolidation:** Standardize the "Skeleton" or "Spinner" loading UI across all main views.
- [ ] **Consistent Result Handling:** Unify the "Win/Loss/Tie" badge and logic used in `GameStats.razor` and `PlayerLogs.razor`.

## 4. Technical Debt & Consistency
- [x] **Centralize Timezone Logic:** Moved `ToWhitbyTime` logic from the mapper into a dedicated `DateTimeService` in the Domain project to ensure consistent timezone handling across all layers.
- [ ] **Standardize Nullable Usage:** Audit the codebase to ensure consistent use of C# 12 nullable features and suppressions (avoiding `null!` where possible).
- [ ] **Clean Up Styles:** Audit `site.css` and component-specific CSS to remove any remaining unused classes from the migration.
