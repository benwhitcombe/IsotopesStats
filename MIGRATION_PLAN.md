# IsotopesStats Migration Plan: Blazor Server to Static WASM + Supabase

## 1. Executive Summary
**Goal:** Transition the IsotopesStats application from a Blazor Server model (running on a paid VPS with a local SQLite database) to a **Blazor WebAssembly (WASM)** model. This will allow for **free hosting** on GitHub Pages while maintaining **multi-user CRUD capabilities** via a cloud-hosted Supabase (PostgreSQL) backend.

---

## 2. The New Tech Stack
*   **Frontend:** Blazor WebAssembly (.NET 8).
*   **Database & API:** **Supabase** (PostgreSQL + Auto-generated REST API).
*   **Authentication:** Supabase Auth (Replacing custom BCrypt/SQLite logic).
*   **Hosting:** GitHub Pages (Static site hosting).
*   **CI/CD:** GitHub Actions (Automated build and deploy).
*   **Estimated Cost:** **$0.00/month** (within free tiers of GitHub and Supabase).

---

## 3. Component Migration Strategy

### A. Data & Models
*   **Models:** Keep existing models (`Game.cs`, `Player.cs`, `StatEntry.cs`, etc.) as they are. They will be shared between the WASM client and the Supabase API.
*   **Schema:** Recreate the SQLite schema in Supabase using PostgreSQL.
*   **Logic:** Move complex SQL queries (like `GetStatsSummaryAsync`) into **PostgreSQL Views** in Supabase to keep the WASM client lightweight.

### B. Management Pages (CRUD)
*   **UI Persistence:** All current management pages (`Management/Games.razor`, `Management/Players.razor`, etc.) will remain in the Blazor project.
*   **Operation:** The buttons and forms will stay exactly as they are. Only the underlying `StatsService` methods will change from executing local SQL to calling the Supabase Client API.
*   **Multi-User Support:** Changes made by any authorized user on any device will persist in the Supabase cloud and be visible to everyone instantly.

### C. Authentication & Users
*   **Auth Service:** Replace the custom `AuthService.cs` (BCrypt + SQLite) with the `supabase-csharp` Auth client.
*   **User Management:** The `Users.razor` page will be updated to use Supabase's user management API.
*   **Security:** Implement **Row Level Security (RLS)** in Supabase to ensure only logged-in admins can modify data, while the public can still view stats.

---

## 4. Step-by-Step Implementation Checklist

### Phase 1: Supabase Setup
- [ ] Create a new project at [supabase.com](https://supabase.com).
- [ ] Run the PostgreSQL schema script (converted from `DatabaseInitializer.cs`).
- [ ] Create Database Views for player and team statistics.
- [ ] Import existing 2025/2026 data from SQLite/JSON.

### Phase 2: Project Refactoring
- [ ] Change `IsotopesStats.csproj` SDK to `Microsoft.NET.Sdk.BlazorWebAssembly`.
- [ ] Remove `Microsoft.Data.Sqlite` and add `supabase-csharp`.
- [ ] Update `Program.cs` to initialize `WebAssemblyHostBuilder`.
- [ ] Rewrite `StatsService` to use Supabase `From<T>().Get()` and `Rpc()` calls.
- [ ] Rewrite `AuthService` to use Supabase Auth (Sign In, Sign Up, Password Reset).

### Phase 3: Deployment
- [ ] Create `.github/workflows/deploy.yml` for GitHub Actions.
- [ ] Configure GitHub Pages to serve from the `gh-pages` branch.
- [ ] Add a `.nojekyll` file to `wwwroot` to ensure `_framework` files are served.

---

## 5. Maintenance & Scalability
*   **Backups:** Supabase provides automated daily backups of the PostgreSQL database.
*   **Realtime Updates:** (Optional) Enable Supabase Realtime so the website updates automatically when a new game is added by another user.
*   **Logs:** Supabase provides an API request log for monitoring site activity.
