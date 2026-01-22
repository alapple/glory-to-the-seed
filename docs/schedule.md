# 4-Day Development Schedule: Glory-to-the-seed

**Team Focus:** This schedule prioritizes **Priority 1** features to ensure a finished, playable game by the deadline.

---

## Day 1: The Skeleton (Logic & Layout)
**Goal:** By evening, the math must work in the console. The game loop (`Start` -> `5 Years` -> `End`) exists. No final graphics needed yet.

### Game Designer (GD)
* **Data Balancing (Excel/Sheets):** Define the core numbers.
    * Potato Quota (e.g., 1000).
    * Region production rates per second.
    * Cost of a fire (e.g., -10 potatoes/sec).
* **Event Writing:** Write the flavor text for Priority 1 events (Fire, Strike, Broken Machine).

### Artist (Art)
* **UI Wireframes:** Sketch exactly where buttons, text, and bars go. **Crucial:** Since this is a UI game, the layout is the most important part.
* **Asset List:** Create a checklist of all needed icons (Vodka, Water, Hammer, Potato).
* **Style Mockup:** Draw one "Region Panel" in the final style so programmers know the dimensions.

### Programmer 1 (Backend/Logic)
* **GameManager:** Create the state machine: `Start -> Year Loop -> End`.
* **Data Models:** Create C# classes for `Region`, `Resource`, and `Modifier`.
* **Math Logic:** Implement the production formula: `Output = BaseValue * Happiness * Modifiers`.

### Programmer 2 (Frontend/UI)
* **Canvas Setup:** Build the layout in Unity using grey boxes (Placeholders) based on the Artist's sketches.
* **Input Stub:** Make buttons clickable (even if they don't do much yet).

---

## Day 2: The Body (Playable Loop)
**Goal:** The systems are connected. You can see what you are doing. Modifiers appear and can be fixed.

### Game Designer
* **Content Entry:** Put the texts into the game.
* **Scenario Testing:** Calculate if the game is winnable. Adjust the quota numbers.
* **Sound Scouting:** Find free SFX (Typewriter, Stamping, Alarm, Soviet Anthem).

### Artist
* **UI Assets:** Paint the actual panels, buttons, and backgrounds. **Priority:** Must be finished today for Prog 2 to implement.
* **Icons:** Draw Vodka, Water, Spare Parts, Fire icon.

### Programmer 1 (Events)
* **Event Spawner:** Program the logic to spawn random events (Fire/Strike) at random times.
* **Fix Mechanics:** Logic: `If Resource "Water" is applied to Region -> Remove "Fire"`.

### Programmer 2 (Integration)
* **Display:** Connect Logic to UI. Show real Potato count and Happiness bars.
* **Interaction:** Implement Drag & Drop (or Clicking) to move resources to regions.

---

## Day 3: The Heart (The End Screen & Atmosphere)
**Goal:** The "Lying" mechanic is implemented. The game starts to feel "juicy" (sound, effects).

### Game Designer
* **Playtesting:** Play the game all day. Is it too hard? Too easy? Tell Prog 1 which numbers to tweak.
* **Endings:** Write text for the different outcomes (Win, Gulag via Quota, Gulag via Lie).

### Artist
* **The Report:** Paint the End-Screen Form (Paper, Stamps).
* **Characters:** Paint the Inspector (Stern look) or Worker portraits.
* **Feedback:** Create visual effects (e.g., Red vignette when fire is active, smoke particles).

### Programmer 1 (The End)
* **Report Logic:** Implement the End-of-Game screen.
* **Calculation:** Compare `Actual Potatoes` vs. `Quota`.
* **Lying Math:** Implement the probability logic (Risk % based on how big the lie is).

### Programmer 2 (Juice)
* **Feedback:** Add Screen Shake when bad things happen.
* **Audio:** Implement SFX and Music.
* **Visuals:** Swap the grey placeholders with the Artist's final assets.

---

## Day 4: The Polish (Fix & Ship)
**Goal:** Bug fixing, building, uploading. **NO NEW FEATURES.**

### Everyone
* **Playtest:** Find bugs. Try to break the game.

### Game Designer
* **itch.io Page:** Write description, upload screenshots.
* **Tutorial:** Write a short "How To Play" (or a text popup inside the game).

### Artist
* **Marketing:** Create the Cover Art and the Game Icon.

### Programmer 1 & 2
* **Bugfixing:** Fix everything that is broken.
* **Build:** Create a WebGL build (for browser play) and a Windows Build.
* **Bonus (Only if 100% finished):** If, and only if, the game works perfectly by 12:00 PM, try adding **Priority 2** (Bribing). Otherwise, skip it.
