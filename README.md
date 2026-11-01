# ABS611 Board — Interactive Task Board

A responsive Kanban-style task board for organising work across three stages: **To Do**, **In Progress** and **Completed**.
Built with plain **HTML5, CSS3 and vanilla JavaScript (ES6+)**, with no frameworks.

**Live demo:** https://abdullahwaseem6.github.io/Interactive-Task-Board/

---

## Features

### Core
- **Add tasks** with a title, an optional description, a category (Development, Design, Marketing) and a priority (Low, Medium, High).
- **Move tasks** between columns using the dropdown on each card, or by dragging the card.
- **Delete tasks** from the board.
- **Persistence:** tasks are saved in the browser's `localStorage`, so they survive a page refresh.

### Interface
- **Responsive design:** three columns on desktop; on phones the form fields and the columns stack into one column.
- **Priority colours:** High = coral red, Medium = amber, Low = green. The colour shows on the card's left edge and on its priority label.
- **Search and filter:** search tasks by title and filter by category. Empty columns explain why they are empty.

### Bonus features
- **Drag and drop** between columns using the native HTML5 Drag and Drop API.
- **Dark / light mode** toggle. Dark is the default, and the choice is remembered.
- **Task counter** in the header showing active vs completed tasks.

---

## How the code works

- All tasks live in one array, `tasks`. Every action (add, move, delete) follows the same pattern:
  **change the array → save it to `localStorage` → redraw the board with `renderTasks()`**.
- Search and category filters only change what `renderTasks()` shows. They never change the saved data.
- All colours are CSS variables in `:root`. Light mode redefines the same variables under `[data-theme="light"]`, so switching theme is a single attribute change on `<html>`.
- Drag and drop: each card stores its task ID when dragging starts. Each column reads its `data-status` attribute on drop and calls the same `updateTaskStatus()` function the dropdown uses.
- User text is escaped before it is put into the page, so typed HTML is shown as text and never runs.

## Project structure

```
├── index.html   Page structure: header, form, filters and the three columns
├── styles.css   Dark and light themes, layout, cards and phone styles
└── script.js    Tasks, localStorage, rendering, drag and drop, theme switch
```

## Run locally

Open `index.html` in a browser, or use the Live Server extension in VS Code.

## Changes from the starter files

- Added the description field (form, saving and display on cards).
- Fixed two invalid CSS values (`align-items: centre` → `center`).
- New dark premium design with a light theme, using CSS variables for every colour.
- Added the task counter, drag and drop, dark/light toggle and empty-column messages.
- Escaped category and priority text as well as the title, and blocked tasks with an empty title.
- Phone layout improvements.
