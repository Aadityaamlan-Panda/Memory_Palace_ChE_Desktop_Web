# 🏛️ Memory Palace

A **VR-based spaced repetition learning application** built in Unity (URP) with a Spring Boot backend. Students walk through a 3D apartment environment and interact with objects linked to educational concepts — leveraging the *method of loci* memory technique to improve recall.

---

## 📖 Table of Contents

- [Overview](#overview)
- [Project Structure](#project-structure)
- [Tech Stack](#tech-stack)
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
  - [Backend Setup](#backend-setup)
  - [Unity Frontend Setup](#unity-frontend-setup)
- [API Reference](#api-reference)
- [Concept Data Model](#concept-data-model)
- [VR Controls](#vr-controls)
- [Key Scripts](#key-scripts)
- [Unity Packages](#unity-packages)
- [Known Issues / TODOs](#known-issues--todos)

---

## Overview

Memory Palace places learning content inside a navigable 3D apartment. Each room object (e.g. a bathtub, lamp, door) is bound to one or more study *concepts* stored in a PostgreSQL database via a REST API. Learners walk through the space, focus on objects, and are shown concept cards with text labels and image panels. After reviewing a concept, they rate their recall confidence (Easy / Medium / Tough), and the system adjusts a *strength* score for spaced-repetition scheduling.

The project supports both a **desktop FPS mode** (keyboard + mouse) and was originally designed for **Meta/Oculus XR** (OpenXR).

---

## Project Structure

```
.
├── Assets/                     # Unity project assets
│   ├── Scripts/                # All C# game scripts
│   │   ├── XR/                 # VR rig, door interaction, concept manager
│   │   ├── Models/             # Data model classes and UI card scripts
│   │   ├── UI/                 # CSV loader, concept list/form UI
│   │   └── Managers/           # App-level and wall UI managers
│   ├── Scenes/                 # Unity scenes (MainScene, MainSceneNew)
│   ├── Prefabs/                # Reusable prefabs
│   ├── Resources/              # Runtime-loaded assets (concepts CSV)
│   ├── XR/                     # XR loader and OpenXR settings
│   └── [Third-party assets]    # Brick Project Studio, ithappy, Hovl Studio, etc.
├── Backend/
│   └── memorypalace/           # Spring Boot REST API
│       ├── src/main/java/com/verstappen/memorypalace/
│       │   ├── controller/     # ConceptController, HelloController
│       │   ├── model/          # Concept entity
│       │   ├── service/        # ConceptService, CsvLoaderService
│       │   ├── dto/            # ConceptDTO, ReviewDTO
│       │   ├── repository/     # ConceptRepository (Spring Data JPA)
│       │   └── config/         # WebConfig (CORS)
│       └── src/main/resources/
│           ├── application.properties
│           └── data/concepts.csv
├── Packages/
│   └── manifest.json           # Unity package dependencies
└── ProjectSettings/
```

---

## Tech Stack

| Layer | Technology |
|---|---|
| Game Engine | Unity (URP 17.3.0) |
| XR | OpenXR 1.16.1, Meta XR SDK |
| Language (Unity) | C# (.NET) |
| Backend Framework | Spring Boot 3.5.11 |
| Language (Backend) | Java 21 |
| Database | PostgreSQL |
| ORM | Spring Data JPA / Hibernate |
| CSV Parsing | OpenCSV 5.9 |
| Build Tool | Maven |
| Web Browser in VR | Unity Web Browser (Voltstro, CEF) |

---

## Prerequisites

### Backend
- Java 21+
- Maven 3.8+
- PostgreSQL (running locally on port `5432`)

### Unity Frontend
- Unity 6 (LTS) with Universal Render Pipeline
- Meta XR SDK (via `com.meta.xr.sdk.all` in Packages)
- OpenXR-compatible headset (optional — desktop FPS mode works without one)

---

## Getting Started

### Backend Setup

1. **Create the database**

   ```sql
   CREATE DATABASE memory_palace;
   ```

2. **Configure credentials**

   Edit `Backend/memorypalace/src/main/resources/application.properties`:

   ```properties
   server.port=8080

   spring.datasource.url=jdbc:postgresql://localhost:5432/memory_palace
   spring.datasource.username=postgres
   spring.datasource.password=your_password_here

   spring.jpa.hibernate.ddl-auto=update
   spring.jpa.show-sql=true
   ```

3. **Run the server**

   ```bash
   cd Backend/memorypalace
   ./mvnw spring-boot:run
   ```

   The API will be available at `http://localhost:8080`.

4. **Seed initial data (optional)**

   On first run, `CsvLoaderService` automatically imports concepts from `src/main/resources/data/concepts.csv` if the database is empty.

---

### Unity Frontend Setup

1. Open the project root in **Unity Hub** and select the Unity 6 editor version.
2. Let the Package Manager resolve all dependencies from `Packages/manifest.json`.
3. Open the scene: `Assets/MainSceneNew.unity`
4. In **Project Settings → XR Plug-in Management**, enable **OpenXR** (or leave it unchecked to run in desktop FPS mode).
5. Ensure the backend is running, then press **Play**.

> **Desktop FPS mode** works fully without a headset. See [VR Controls](#vr-controls) below.

---

## API Reference

Base URL: `http://localhost:8080`

| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/concepts` | List concepts (paginated, default 200/page) |
| `GET` | `/concepts/all-sorted` | All concepts, ordered by ID ascending |
| `GET` | `/concepts/{id}` | Get single concept by ID |
| `GET` | `/concepts/search?keyword=` | Search concepts by keyword |
| `POST` | `/concepts` | Create a new concept |
| `PUT` | `/concepts/{id}` | Update a concept |
| `DELETE` | `/concepts/{id}` | Delete a concept |
| `PATCH` | `/concepts/{id}/review` | Submit a recall review score |

### Review Score (`PATCH /concepts/{id}/review`)

```json
{ "score": 2 }
```

| Score | Meaning | Strength change |
|---|---|---|
| `2` | Easy | +2 |
| `1` | Medium | +1 |
| `0` | Tough | −2 |

---

## Concept Data Model

Each concept represents a study topic linked to a physical object in the VR scene.

| Field | Type | Description |
|---|---|---|
| `id` | Long | Auto-generated primary key |
| `title` | String | Concept name |
| `description` | String | Full description (up to 1000 chars) |
| `mediaUrl` | String | URL to a representative image |
| `memoryObject` | String | Name of the Unity scene object this concept is attached to |
| `location` | String | Room/area in the Memory Palace |
| `visualCue` | String | Short mnemonic hint |
| `strength` | int | Recall strength score (starts at 0) |
| `repetitions` | int | Total number of review sessions |
| `lastReviewed` | DateTime | Timestamp of the most recent review |
| `createdAt` | DateTime | Auto-set on create |
| `updatedAt` | DateTime | Auto-updated on save |

**Example CSV row:**

```
815,Manometry,"Explains pressure measurement using manometers...",https://...Slide3.jpg,BathTub,,,1,,
```

---

## VR Controls

The project ships a **FPS desktop rig** (`XRSimpleRig.cs`) as a drop-in replacement for the Meta XR controller rig.

| Input | Action |
|---|---|
| `WASD` | Move |
| `Mouse` | Look around (cursor auto-locked; `Esc` to release, `RMB` to re-lock) |
| `Q / E` | Fly up / down |
| `F` | Interact with door, sliding panel, or teleport trigger |
| `G` | Show concept **text labels and image panel** for the object in the crosshair |

**Crosshair feedback:**

- Left half **green** → a door/slide/teleport is in view
- Right half **green** → a concept-linked object is in view (press `G` to reveal labels + panel)
- Both halves dim → nothing interactive in range

---

## Key Scripts

| Script | Location | Purpose |
|---|---|---|
| `XRSimpleRig.cs` | `Scripts/XR/` | FPS movement, mouse look, raycasting, crosshair feedback, key bindings |
| `XRConceptManager.cs` | `Scripts/XR/` | Spawns concept text labels and image panels above selected objects |
| `XRConceptInteractable.cs` | `Scripts/XR/` | Component on scene objects; fires `OnSelected()` to notify the manager |
| `XRDoorInteract.cs` | `Scripts/XR/` | Smooth door open/close with side-detection and collider management |
| `ConceptItem.cs` | `Scripts/Models/` | Populates text label prefabs; applies traffic-light strength indicator |
| `ConceptImagePanel.cs` | `Scripts/Models/` | Populates the 3D image panel prefab with concept media |
| `CSVLoader.cs` | `Scripts/UI/` | Fallback: loads concepts directly from a local CSV in `Resources/` |
| `ConceptScrollerController.cs` | `Scripts/Models/` | Scrollable concept list UI in-world |
| `BrowserLoader.cs` | `Scripts/` | Loads concept `mediaUrl` in the embedded CEF browser panel |
| `AppManager.cs` | `Scripts/Managers/` | App-level initialization and scene management |

---

## Unity Packages

Key third-party and Unity packages used (from `Packages/manifest.json`):

| Package | Version | Purpose |
|---|---|---|
| `com.unity.render-pipelines.universal` | 17.3.0 | Universal Render Pipeline (URP) |
| `com.unity.xr.openxr` | 1.16.1 | OpenXR support for VR headsets |
| `com.unity.xr.management` | 4.5.4 | XR plugin management |
| `com.meta.xr.sdk.all` | (local) | Meta/Oculus XR SDK |
| `com.google.xr.cardboard` | git | Google Cardboard XR plugin |
| `com.unity.inputsystem` | 1.18.0 | New Input System |
| `com.unity.cinemachine` | 3.1.6 | Camera management |
| `com.unity.ai.navigation` | 2.0.9 | NavMesh / AI navigation |
| `dev.voltstro.unitywebbrowser` | 2.2.8 | Embedded Chromium browser (CEF) |
| `com.unity.postprocessing` | 3.5.1 | Post-processing effects |
| `com.unity.shadergraph` | 17.3.0 | Shader Graph |

---

## Known Issues / TODOs

- [ ] `application.properties` contains a hardcoded password — move to environment variables or a secrets manager before deploying
- [ ] CORS is configured globally via `WebConfig.java` — tighten allowed origins for production
- [ ] Desktop FPS mode and XR mode share the same scene; a build-time flag or separate scene would be cleaner
- [ ] `ConceptFormUI.cs` is currently empty — concept creation from inside VR is not yet implemented
- [ ] `XRConceptInteractable.cs` is no longer in the critical interaction path; remove or re-wire if the Meta XR build is revisited
- [ ] The embedded browser (`BrowserLoader.cs`) requires the CEF engine; Linux x64 binary is included but Windows/Mac builds may need additional setup

---

## License

[MIT LICENSE](LICENSE)
