# Folder Structure

```
Assests/
├── Art/
│   └── Sprites
├── Audio/
│   ├── Music
│   └── SFX
├── Data/
│   ├── Dialogs
│   ├── Events
│   └── Resources
├── Prefabs
├── Scenes
├── Scrips/
│   ├── Core
│   ├── Data
│   ├── Gameplay
│   ├── UI/
│   │   ├── Controllers
│   │   ├── components
│   │   └── Manipulators
│   └── Utils
└── UI/
    ├── Documents/
    │   ├── Screens
    │   └── Templates
    ├── Styles/
    │   ├── Shared
    │   └── Screens
    ├── Fonts
    ├── Panels
    └── Icons
```

## Data
All content is realised as [Scriptable Objects](https://docs.unity3d.com/6000.3/Documentation/Manual/class-ScriptableObject.html).

## Scrips
### UI/Controllers
Manage specific screens

### UI/Components
Custom VisualElements (inherited C# classes) [Custom C# Element with Inheritance](https://docs.unity3d.com/Packages/com.unity.ui.builder@1.0/manual/uib-structuring-ui-custom-elements.html)

### UI/Manipulators
Reusable logic (DraggableElement.cs, HoverEffect.cs)

## UI
### Documents/Screens
Full-screen layouts (HUD, Pause Menu)

### Shared
Global variables, themes, common styles

### Styles/Screens
Layout-specific styling tied to Documents

### Panels
Panel Settings assets (Define UI scaling and rendering)