# IsotopesStats Refactoring Checklist

## 1. Repository & Data Layer
- [ ] **Consolidate Mapping Logic:** Replace manual mapping in `ModelMappers.cs` with a more maintainable approach (e.g., source-generated mappers or refined generic mapping).
- [x] **Optimize Aggregate Queries:** Moved in-memory LINQ aggregations to Supabase **Database Functions** (`get_player_stats_summary_all`, `get_team_stats_summary_all`) and updated repository to use RPC.
- [x] **Unify Filter Logic:** Replaced magic numbers (`-1`) with explicit `GetAll*Async` methods across all layers to improve clarity and reduce branching.
- [x] **DTO/Model Alignment:** Renamed all occurrences of `Dto` to `DTO` and verified alignment between Domain models and Repository DTOs for consistency.

## 2. Service Layer & Abstraction
- [ ] **Evaluate Pass-Through Services:** Decide whether to keep `StatsService` and `AuthService` as abstractions or inject repositories directly into UI components for simplicity.
- [ ] **Generic CRUD Operations:** Implement a base repository or helper to handle standard Add/Update/Delete patterns for Players, Seasons, and Opponents.
- [ ] **Standardize Logging:** Refine the `DeleteManager` and other logging points to use a more consistent, type-safe approach for identifying entities (e.g., an `INameable` interface).

## 3. UI & Shared Components
- [x] **Extract Shared UI Logic:** Created a reusable `SeasonSelector` component with centered text and anti-layout-shift logic to replace redundant selection UI in stats pages.
- [x] **Generic Sorting:** Refactored large `switch` statements for sorting into a dynamic system using the `SortByColumn` extension method.
- [x] **Loading State Consolidation:** Created a shared `LoadingIndicator` component and standardized loading UI across all pages. Improved `ModelEditor` reactivity with immediate feedback during save operations.
- [x] **Consistent Result Handling:** Created a reusable `GameResultBadge` component to unify "Win/Loss/Tie" logic and styling across all stats views.
- [x] **Restore Checkbox Styling:** Restored original CSS for selectable button checkboxes to fix double checkboxes and non-uniform sizing.

## 4. Technical Debt & Consistency
- [x] **Centralize Timezone Logic:** Moved `ToWhitbyTime` logic from the mapper into a dedicated `DateTimeService` in the Domain project to ensure consistent timezone handling across all layers.
- [ ] **Standardize Nullable Usage:** Audit the codebase to ensure consistent use of C# 12 nullable features and suppressions (avoiding `null!` where possible).
- [ ] **Clean Up Styles:** Audit `site.css` and component-specific CSS to remove any remaining unused classes from the migration.
