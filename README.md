# Moorhase

A simple Whack-a-Mole game for Windows in WPF/C#.

## Installation

```bash
dotnet build
dotnet run
```

## Setup - Images

Place your images in the `Assets` folder:

```
Moorhase/Assets/
├── Grass.png
├── Hase.png
└── Jäger.png (optional)
```

If images are missing, the game will use fallback colors.

## Game Modes

**Task Mode**
- Wrong clicks are allowed
- Rabbits appear for 2 seconds
- Interval decreases by 10% per hit (min. 500ms)

**Expert Mode**
- One wrong click = Game Over
- Too slow = Game Over
- Interval decreases by 15% per hit + 5% every 5 seconds (min. 300ms)
- Sometimes a hunter (Jäger) appears - clicking it = Game Over

## Controls

- **Mouse click** on the rabbit
- **ESC** to abort

## Full Window Controls

- Maximize
- Fullscreen
- Reset to 800x600

## Highscores

Automatically saved to:
`%APPDATA%\Moorhase\highscores.json`
- MVVM-ähnliche Architektur
- Event-basierte Timer
- JSON-Serialisierung für Highscores

