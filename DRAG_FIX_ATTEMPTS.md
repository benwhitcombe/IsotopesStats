# Documentation of Attempts to Resolve Drag "Denied" Icon Flicker

## Issue Description
When starting a drag operation on draggable elements (especially in `Lineup.razor`), a "denied" icon (red circle with slash) would briefly appear before switching to the correct "move" cursor/drag image. This caused a distracting flicker.

## Attempts Made

### 1. Synchronous JavaScript Initialization (`dragstart`)
*   **Strategy:** Intercept the `dragstart` event at the window/document level synchronously to initialize `dataTransfer` before the browser could default to a "denied" state.
*   **Changes:** Added a global `dragstart` listener in `site.js` that set `setData`, `effectAllowed`, and proactively called `setDragImage`.
*   **Result:** Reduced the duration of the flicker but did not eliminate it entirely in all browsers.

### 2. Global `dragover` and `dragenter` Prevention
*   **Strategy:** Browsers show the "denied" icon if the current hovered area isn't a valid drop target (determined by `preventDefault()` on `dragover`). By globally preventing default on the entire window, we tell the browser that the whole app is a valid drop zone.
*   **Changes:** Added global `dragover` and `dragenter` listeners that called `e.preventDefault()` whenever an internal drag was active.
*   **Result:** Improved cursor stability once the drag was moving, but the initial frame still flickered.

### 3. Native HTML Attributes on `<body>`
*   **Strategy:** HTML attributes (`ondragover="..."`) execute at a higher priority/speed than JavaScript event listeners added via `addEventListener`.
*   **Changes:** Added `ondragover="event.preventDefault();"` to the `<body>` tag in `index.html`.
*   **Result:** Highly effective in some browsers, but raised concerns about potential side effects and platform-specific behavior (e.g., Samsung Internet).

### 4. Blazor Event Modifiers (`:preventDefault`)
*   **Strategy:** Use Blazor's built-in event modifiers to ensure synchronous prevention of default behavior.
*   **Changes:** Added `@ondragover:preventDefault` and `@ondragenter:preventDefault` to reorderable rows and cells.
*   **Result:** Identified as a major issue for Samsung Internet and some Android WebViews, where the rendered attribute names (e.g., `_bl_@ondragover:preventDefault`) contain illegal characters that crash the renderer.

### 5. Removal of Asynchronous Bottlenecks
*   **Strategy:** Avoid `await JSRuntime.InvokeAsync` calls inside `HandleDragStart` in C#.
*   **Changes:** Refactored `HandleDragStart` to be synchronous where possible or to not wait for JS interop results before allowing the event to complete.
*   **Result:** Slightly improved response time but the root flicker remained.

### 6. CSS Stabilizations
*   **Strategy:** Prevent text selection from interfering with the drag start.
*   **Changes:** Added `user-select: none` and `cursor: move` to draggable elements in CSS.
*   **Result:** Prevented accidental text dragging but didn't solve the "denied" icon issue.

## Conclusion
Despite multiple layers of prevention (Global JS, Native HTML, Blazor Modifiers), the "denied" icon flicker proved persistent, likely due to a fundamental race condition between the browser's UI thread and the Blazor/JavaScript execution context during the initial drag-start frame.
