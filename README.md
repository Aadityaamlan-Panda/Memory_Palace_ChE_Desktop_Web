# 🏛️ Memory Palace — Chemical Engineering Edition

> **A Unity 6 application applying the Method of Loci to technical education.**  
> Developed at the Department of Chemical Engineering, IIT Kanpur.  
> Authors: Aaditya Amlan Panda (220007) · Abhijit Dalai (220030)

> **Note:** This is the **desktop / web build** of the project — it runs in the Unity Editor or as a standalone desktop application using a keyboard+mouse FPS rig, without requiring a VR headset. The full Meta Quest XR build lives in a separate private repository.

---

## 📖 Table of Contents

- [Overview](#overview)
- [Research Background](#research-background)
- [Pilot Study Results](#pilot-study-results)
- [Project Structure](#project-structure)
- [Tech Stack](#tech-stack)
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
  - [Backend Setup](#backend-setup)
  - [Unity Frontend Setup](#unity-frontend-setup)
- [API Reference](#api-reference)
- [Concept Data Model](#concept-data-model)
- [Spatial Mnemonic Design](#spatial-mnemonic-design)
- [Desktop Controls](#desktop-controls)
- [Web Review Dashboard](#web-review-dashboard)
- [Key Scripts](#key-scripts)
- [Unity Packages](#unity-packages)
- [Known Issues / TODOs](#known-issues--todos)
- [Future Work](#future-work)
- [Contributors](#contributors)
- [Participants](#participants)

---

## Overview

Memory Palace places learning content inside a navigable 3D apartment. Each room object (e.g. a bathtub, lamp, door) is bound to one or more **Chemical Engineering concepts** stored in a PostgreSQL database via a REST API. Learners walk through the space, aim at objects, and are shown floating concept text labels and slide image panels — reinforcing spatial memory encoding without needing to imagine a palace from scratch.

After exploring, a companion **web review dashboard** (served by the same backend) lets users rate each concept's recall difficulty using a traffic-light system, which feeds a spaced-repetition strength score.

The dataset covers **114 Chemical Engineering concepts** across material balances, thermodynamics, phase equilibria, equations of state, reaction engineering, and heat transfer.

---

## Research Background

The **Method of Loci** exploits the well-established superiority of spatial over verbal memory encoding. Neuroimaging studies show that navigating a familiar environment activates the hippocampus and parahippocampal place area — regions central to spatial and episodic memory consolidation. Dresler et al. (2017) demonstrated that six weeks of Method-of-Loci training produced durable changes in brain connectivity and recall performance persisting at a four-month follow-up.

VR and 3D environments extend this by providing a persistent, richly navigable space that the learner genuinely *inhabits* rather than merely imagines, adding a proprioceptive and navigational layer on top of visual instruction.

**Gap addressed:** Very few prior studies have combined an explicit Method-of-Loci protocol with a fully interactive 3D environment for abstract technical undergraduate content, and compared trained participants against a retention baseline on identical quiz items in a controlled two-day setting.

### References

1. Legge et al. (2012). Building a memory palace in minutes: Equivalent memory performance using virtual versus conventional environments. *Acta Psychologica*, 141(3), 380–390.
2. Dresler et al. (2017). Mnemonic training reshapes brain networks to support superior memory. *Neuron*, 93(5), 1227–1235.
3. Parong & Mayer (2018). Learning science in immersive virtual reality. *Journal of Educational Psychology*, 110(6), 785–797.
4. Maguire et al. (1998). Knowing where and getting there: a human navigation network. *Science*, 280(5365), 921–924.

---

## Pilot Study Results

A six-participant pilot study was conducted with undergraduate students from IIT Kanpur (five non-ChE disciplines + one final-year ChE student). Participants completed two rounds of exploration followed by a multiple-choice quiz each round. One participant (benchmark) answered the quiz 2.5 hours after the session ended, serving as a delayed-recall reference.

### Accuracy

| Participant | Dept. | Q-Set | Round 1 (%) | Round 2 (%) | Δ (pp) |
|---|---|---|---|---|---|
| Akarsh | Economics | 2 | 78.0 | 90.2 | +12.2 |
| Arjit | Civil Engg. | 1 | 70.2 | 57.9 | −12.3 |
| Dhruv | Aerospace Engg. | 2 | 66.7 | 80.6 | +13.9 |
| Jagdeesh | Mechanical Engg. | 2 | 76.0 | 96.0 | +20.0 |
| Snehil | Chemical Engg. | 1 | 73.2 | 78.0 | +4.9 |
| **Group Mean** | | | **72.8** | **80.5** | **+7.7** |
| Amit *(benchmark)* | Mat. Sci. & Engg. | 2 | — | 73.3 | — |

### Response Time

Average per-question response time dropped **60.8%** across all five gameplay participants — from 20.2 s (Round 1) to 7.9 s (Round 2). The benchmark participant averaged 11.6 s, slower than every Round 2 gameplay participant.

### Hint Usage

Hint dependency collapsed between rounds. Three of five participants reached **0% hint usage** in Round 2, indicating that spatial associations replaced textual scaffolding as the primary retrieval cue.

### Feedback

Post-session Likert ratings averaged **8.5 / 10** for overall learning effectiveness. The highest-rated item (9.0 / 10) was "how well exploration aided quiz performance." The lowest-rated item (7.2 / 10) was topic locatability — identified as the key UX improvement target.

---

## Project Structure

```
.
├── Assets/                     # Unity project assets
│   ├── Scripts/                # All C# game scripts
│   │   ├── XR/                 # FPS rig, door interaction, concept manager
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
│           ├── static/index.html   # Web review dashboard
│           └── data/concepts.csv
├── Packages/
│   └── manifest.json           # Unity package dependencies
└── ProjectSettings/
```

---

## Tech Stack

| Layer | Technology |
|---|---|
| Game Engine | Unity 6 (URP 17.3.0) |
| XR / Input | OpenXR 1.16.1, New Input System |
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
- No VR headset required for the desktop build

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

4. **Seed initial data**

   On first run, `CsvLoaderService` automatically imports 114 concepts from `src/main/resources/data/concepts.csv` if the database is empty. Image URLs follow the pattern:

   ```
   https://raw.githubusercontent.com/Aadityaamlan-Panda/CSE-Topics/main/ChemE_Enhanced/Slide{N}.jpg
   ```

---

### Unity Frontend Setup

1. Open the project root in **Unity Hub** and select the Unity 6 editor version.
2. Let the Package Manager resolve all dependencies from `Packages/manifest.json`.
3. Open the scene: `Assets/MainSceneNew.unity`
4. In **Project Settings → XR Plug-in Management**, enable **OpenXR** (or leave it unchecked to run in pure desktop mode).
5. Ensure the backend is running, then press **Play**.

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

| Field | Type | Description |
|---|---|---|
| `id` | Long | Auto-generated primary key |
| `title` | String | Concept name |
| `description` | String | Full description (up to 1000 chars) |
| `mediaUrl` | String | GitHub-hosted slide image URL |
| `memoryObject` | String | **Key linking concept to scene object** — must exactly match the Unity GameObject name |
| `location` | String | Room/area in the Memory Palace |
| `visualCue` | String | Short mnemonic hint |
| `strength` | int | Recall strength score (adjusted by review) |
| `repetitions` | int | Total number of review sessions |
| `lastReviewed` | DateTime | Timestamp of the most recent review |
| `createdAt` | DateTime | Auto-set on create |
| `updatedAt` | DateTime | Auto-updated on save |

**Example CSV row:**

```
815,Manometry,"Explains pressure measurement using manometers...",https://...Slide3.jpg,BathTub,,,1,,
```

---

## Spatial Mnemonic Design

Each concept is anchored to a scene object chosen for a *meaningful* mnemonic relationship — objects whose function or appearance bears a logical connection to the concept:

| Scene Object | Concept | Mnemonic Rationale |
|---|---|---|
| `BlenderContainer` | Extent of Reaction | Blender and reactor both transform raw inputs into products |
| `CoffeeMaker_Apt_01` | Reactive Material Balances | Coffee extraction consumes reactants and produces a product stream |
| `Fridge_01` | Vapour-Compression Refrigeration Cycle | Fridge *is* the refrigeration cycle |
| `Stove_Top` | Combustion Reactions | Direct functional match |
| `Toilet_Apt_01` | Water-Air VLE & Psychrometric Chart | Water vapour flow analogy |
| `BathTub` | Manometry | Fluid-level / pressure analogy |
| `Trash_apt_01` | Recycle Mass Balances | Recycle loop analogy |
| `Range_Hood` | Ideal Gas Law | Gas extraction / flow |
| `Mech_apt_01` | Mechanical Energy Balance | Mechanical → mechanical energy |

---

## Desktop Controls

The project ships an FPS desktop rig (`XRSimpleRig.cs`) as a drop-in replacement for the Meta XR controller rig.

| Input | Action |
|---|---|
| `WASD` | Move |
| `Mouse` | Look around (cursor auto-locked; `Esc` to release, `RMB` to re-lock) |
| `Q / E` | Fly up / down |
| `F` | Interact with door, sliding panel, or teleport trigger |
| `G` | Show concept **text labels and image panel** for the object in the crosshair |

**Crosshair feedback:**

- Left half **green** → a door/slide/teleport is in view
- Right half **green** → a concept-linked object is in view (press `G` to reveal)
- Both halves dim → nothing interactive in range

---

## Web Review Dashboard

Served at `http://localhost:8080/index.html` (or the deployed backend URL).

The dashboard presents all 114+ concepts as **priority-sorted review cards**. The priority algorithm is:

```
priority(item) = item.strength − item.repetitions × 0.5
```

A concept with low or negative strength and few repetitions rises to the top. A well-learned concept sinks to the bottom.

**Features:**
- Traffic-light recall buttons: 🟢 Easy / 🟡 Medium / 🔴 Hard — each click calls `PATCH /concepts/{id}/review`
- Concept images loaded from GitHub at runtime
- Shared database with the Unity client — review state updated here is reflected the next time Unity fetches concepts

---

## Key Scripts

| Script | Location | Purpose |
|---|---|---|
| `XRSimpleRig.cs` | `Scripts/XR/` | FPS movement, mouse look, raycasting, crosshair feedback, key bindings |
| `XRConceptManager.cs` | `Scripts/XR/` | Spawns / destroys concept text labels and image panels in world space |
| `XRConceptInteractable.cs` | `Scripts/XR/` | Per-object interaction bridge; fires `OnSelected()` to notify the manager |
| `XRDoorInteract.cs` | `Scripts/XR/` | Smooth door open/close with side-detection and collider management |
| `ConceptItem.cs` | `Scripts/Models/` | Populates text label prefabs; applies traffic-light strength colour indicator |
| `ConceptImagePanel.cs` | `Scripts/Models/` | Downloads `mediaUrl` image via `UnityWebRequestTexture`; falls back to red texture on failure |
| `CSVLoader.cs` | `Scripts/UI/` | Offline fallback: loads concepts directly from a local CSV in `Resources/` |
| `ConceptScrollerController.cs` | `Scripts/Models/` | Scrollable concept list UI in-world |
| `BrowserLoader.cs` | `Scripts/` | Loads concept `mediaUrl` in the embedded CEF browser panel |
| `AppManager.cs` | `Scripts/Managers/` | App-level initialisation and scene management |

---

## Unity Packages

| Package | Version | Purpose |
|---|---|---|
| `com.unity.render-pipelines.universal` | 17.3.0 | Universal Render Pipeline (URP) |
| `com.unity.xr.openxr` | 1.16.1 | OpenXR support |
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

- [ ] `application.properties` contains a hardcoded password — move to environment variables before deploying
- [ ] CORS is configured globally via `WebConfig.java` — tighten allowed origins for production
- [ ] Desktop FPS mode and XR mode share the same scene; a build-time flag or separate scene would be cleaner
- [ ] `ConceptFormUI.cs` is currently empty — concept creation from inside the app is not yet implemented
- [ ] `XRConceptInteractable.cs` is no longer in the critical interaction path (G key calls manager directly); remove or re-wire if the Meta XR build is revisited
- [ ] The embedded browser (`BrowserLoader.cs`) requires the CEF engine; Linux x64 binary is included but Windows/Mac builds may need additional setup

---

## Future Work

1. **Larger participant pool** — current n=5 (complete data) is insufficient for statistical inference; a study with 20+ participants per condition is needed.
2. **True no-VR control condition** — participants who review the same concept slides without spatial encoding, to isolate the Method-of-Loci effect from repeated concept-image exposure.
3. **Longitudinal spaced-repetition testing** — the `strength`/`repetitions` infrastructure is already in place; follow-up quizzes at 1-day, 1-week, and 1-month intervals would test durability of spatial encoding.
4. **Progressive hint fading** — automatically reduce hint availability across rounds to force deeper spatial encoding.
5. **Mini-map and waypoint markers** — address the lowest-rated feedback item (topic locatability, 7.2/10) with a HUD showing unvisited concept locations.
6. **Controlled anchor quality variation** — compare semantically coherent anchors (blender → reaction) against arbitrary placements to quantify how much effect is due to mnemonic quality vs. spatial encoding alone.
7. **Uniform question set** — eliminate the Set 1 / Set 2 confound by administering the same question type to all participants.

---

## Contributors

| Name | Roll No. | Department |
|---|---|---|
| Aaditya Amlan Panda | 220007 | Chemical Engineering, IIT Kanpur |
| Abhijit Dalai | 220030 | Chemical Engineering, IIT Kanpur |

Additional contributions: **Shorya Agarwal** (resource contributions).

---

## Participants

The following students from IIT Kanpur volunteered to participate in the pilot study. All provided informed consent prior to the session.

| Name | Department | Question Set | Role |
|---|---|---|---|
| Akarsh V. | Economics | Set 2 | Gameplay participant |
| Arjit Verma | Civil Engineering | Set 1 | Gameplay participant |
| Dhruv Bansal | Aerospace Engineering | Set 2 | Gameplay participant |
| Jagdeesh | Mechanical Engineering | Set 2 | Gameplay participant |
| Snehil Tripathi | Chemical Engineering | Set 1 | Gameplay participant |
| Amit Kumar | Materials Science & Engineering | Set 2 | Retention benchmark participant |

> The benchmark participant (Amit Kumar) completed two full gameplay rounds but answered the quiz 2.5 hours after the session ended due to a system failure. His data serves as a delayed-recall reference rather than a standard gameplay round.

---

## License

[MIT LICENSE](LICENSE)
