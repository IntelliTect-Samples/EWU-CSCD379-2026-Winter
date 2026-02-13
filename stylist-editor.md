## Plan: Stylist List & Editor Page

Build a mobile-friendly Vuetify-based page in `salon.web` that lists all stylists with thumbnail images, supports editing stylist details via a modal dialog (including image upload), and allows adding new stylists. Extend the API with missing CRUD endpoints and configure CORS.

---

### Steps

**1. Install Vuetify in `salon.web`**

- Add `vuetify-nuxt-module` and `@mdi/font` as dependencies in `salon.web/package.json` (follow the pattern in `wordle.web/package.json`)
- Update `salon.web/nuxt.config.ts` to add `build: { transpile: ['vuetify'] }`, `modules: ["vuetify-nuxt-module"]`, and `vuetify: {}` (matching `wordle.web/nuxt.config.ts`)
- Run `npm install` in `salon.web/`

**2. Configure CORS in the API**

- In `SalonManagementService.Api/Program.cs`, add a CORS policy that allows the Nuxt dev server origin (likely `http://localhost:3000`) with any header and any method
- Add `app.UseCors(...)` before `app.MapControllers()`

**3. Add `IsActive` to the Stylist model**

- In `SalonManagementService.Api/Models/Stylist.cs`, add a new property: `IsActive` (bool, default true)
- Create a new migration to add this column to the database
- Update the seeder to set `IsActive = true` for all seeded stylists

**4. Create a unified `StylistDto` in the API**

- New file: `SalonManagementService.Api/Dtos/StylistDto.cs`
- Properties: `StylistId` (Guid, nullable or Guid.Empty for new records), `Name` (string, required), `PhoneNumber` (string, required), `ChairName` (string, required), `WorkStartTime24H` (decimal), `WorkEndTime24H` (decimal), `ImageUrl` (string, nullable), `IsActive` (bool)
- This DTO is used for list display, create, and update requests (image is handled separately)
- Remove the old `StylistListDto.cs` file

**5. Add CRUD endpoints to `StylistController`**

- In `SalonManagementService.Api/Controllers/StylistController.cs`:
  - Update `GET /Stylist/List` to return `IEnumerable<StylistDto>` with all properties, filtered to only return stylists where `IsActive == true`
  - Add `POST /Stylist` (upsert endpoint) — accepts `StylistDto`:
    - If `StylistId` is null or `Guid.Empty`: create a new `Stylist` with a new Guid, set `IsActive = true`, save to DB, return the created `StylistDto`
    - If `StylistId` has a value: find existing stylist, update all fields (Name, PhoneNumber, ChairName, WorkStartTime24H, WorkEndTime24H), save to DB, return updated `StylistDto`
    - Return 404 if updating and stylist not found
  - Add `DELETE /Stylist/{id}` (soft delete) — finds the stylist, sets `IsActive = false`, saves to DB, returns 204 No Content (or 404 if not found)
  - The existing `PUT /Stylist/Image/{id}` endpoint is already suitable for image upload

**6. Update `salon.web/app/app.vue` — Vuetify layout shell**

- Replace `<NuxtWelcome />` with a Vuetify layout: `v-app` → `v-app-bar` (with title "Salon Management") → `v-main` → `v-container` → `<NuxtPage />`
- Follow the same pattern used in `wordle.web/app/app.vue`

**7. Create the Stylists list page**

- New file: `salon.web/app/pages/stylists-list.vue`
- Use `<script setup lang="ts">` with `useFetch` to call `GET /Stylist/List`
- Display stylists in a responsive `v-row` / `v-col` grid of `v-card` components (cols="12" sm="6" md="4" lg="3" for mobile-friendliness)
- Each card shows:
  - Thumbnail image via `v-img` pointing to `/Stylist/Image/{id}` (with a placeholder/avatar icon if no image)
  - Stylist name, phone number, chair name, formatted work hours
  - An "Edit" `v-btn` (icon: `mdi-pencil`) that opens the edit dialog for that stylist
- A floating action button (`v-btn` with `mdi-plus` icon, positioned bottom-right via CSS or `v-fab`) to add a new stylist — opens the same dialog in "create" mode

**8. Create the Stylist Edit/Create dialog component**

- New file: `salon.web/components/StylistDialog.vue`
- Props: `modelValue` (boolean for v-model show/hide), `stylist` (nullable — null means "create" mode, object means "edit" mode)
- Emits: `update:modelValue`, `saved` (emitted after successful save to trigger list refresh)
- Uses `v-dialog` with `max-width="600"` containing a `v-card`:
  - `v-card-title`: "Add Stylist" or "Edit Stylist" based on mode
  - `v-card-text` with a `v-form`:
    - `v-text-field` for Name (required, with validation rules)
    - `v-text-field` for Phone Number (required)
    - `v-text-field` for Chair Name (required)
    - `v-text-field` (type number) for Work Start Time (24H format)
    - `v-text-field` (type number) for Work End Time (24H format)
    - Image section: current thumbnail preview (if exists) + `v-file-input` for uploading a new image (accept="image/\*")
  - `v-card-actions`: "Cancel" button (closes dialog), "Save" button (submits form)
- Save logic:
  - If creating: `POST /Stylist` with form data, then if an image was selected, `PUT /Stylist/Image/{newId}` with the file
  - If editing: `PUT /Stylist/{id}` with form data, then if a new image was selected, `PUT /Stylist/Image/{id}` with the file
  - Emit `saved` on success so the parent refreshes the list
- Delete logic (edit mode only): a "Delete" `v-btn` (color red) that confirms via a simple confirm prompt, then calls `DELETE /Stylist/{id}` and emits `saved`

**9. Configure API base URL for the frontend**

- In `salon.web/nuxt.config.ts`, add `runtimeConfig: { public: { apiBase: 'https://localhost:7169' } }` (or use an env variable)
- Use `useRuntimeConfig().public.apiBase` as the base URL prefix for all fetch calls, or configure a Nuxt server proxy

---

### Verification

- **API**: Run the API project, test all endpoints via Swagger UI at `https://localhost:7169/swagger` — verify List returns expanded DTO, Create/Update/Delete work, Image upload works
- **Frontend**: Run `npm run dev` in `salon.web/`, navigate to `http://localhost:3000`:
  - Stylists display in a responsive card grid with thumbnail images
  - Click "Edit" on a card → modal opens pre-filled with stylist data
  - Modify fields and save → list updates with new data
  - Upload an image in the modal → thumbnail updates after save
  - Click the "+" FAB → modal opens empty for creating a new stylist
  - Create a new stylist → it appears in the list
  - Delete a stylist → it disappears from the list
  - Resize browser to mobile width → cards stack vertically in a single column

---

### Decisions

- **Single dialog component for create + edit**: Reuse one `StylistDialog.vue` component in two modes rather than separate pages. Keeps the UX simple and the code DRY.
- **Unified DTO for all operations**: Combining `StylistListDto` and `StylistEditDto` into a single `StylistDto` reduces redundancy and simplifies the API surface. The same DTO works for list display, create, and update.
- **Upsert pattern with single POST endpoint**: Using one `POST /Stylist` endpoint for both create and update (checking `StylistId` to determine the operation) simplifies the client code. While unconventional for pure REST, it's pragmatic for this use case.
- **Soft delete with IsActive flag**: Instead of hard deleting stylists (which would fail due to foreign key constraints), set `IsActive = false`. The list endpoint filters to only show active stylists. This preserves historical data and appointment records.
- **Image upload as a separate step**: Keep the existing `PUT /Stylist/Image/{id}` endpoint. The dialog first saves the text fields (getting back the ID for new stylists), then uploads the image if one was selected. This avoids multipart form complexity on the main create/update endpoint.
- **Vuetify card grid over data table**: Cards with images are more visually appealing and naturally mobile-friendly compared to a `v-data-table`.
