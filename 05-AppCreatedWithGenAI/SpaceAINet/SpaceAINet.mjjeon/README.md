# Space.AI.NET() - AI-Powered Console Space Invaders Game

A classic Space Invaders-style console game built with C# and .NET 9, featuring double-buffered rendering, Unicode box-drawing characters, screenshot capabilities, and **AI gameplay using Ollama**.

## Features

- **AI-Powered Gameplay**: Let Ollama AI play the game automatically
- **Flicker-free rendering**: Uses double-buffered console rendering for smooth gameplay
- **Unicode graphics**: Beautiful box-drawing characters (┌ ┐ └ ┘ ─ │) for game borders
- **Screenshot capture**: Press 'S' to save game screenshots as PNG files
- **Three game speeds**: Choose between Slow, Medium, and Fast gameplay
- **Color-coded enemies**: Different enemy types with distinct colors and patterns
- **Bullet system**: Player can fire up to 3 bullets simultaneously
- **Real-time mode switching**: Toggle between manual and AI control during gameplay

## Game Controls

| Key | Action |
|-----|--------|
| ← → | Move player left/right (Manual mode) |
| SPACE | Shoot bullet (Manual mode) |
| O | Toggle AI Mode (Ollama) |
| S | Take screenshot |
| Q | Quit game |

## AI Features

### Ollama Integration
The game integrates with **Ollama** to provide AI-powered gameplay:

- **Real-time decision making**: AI analyzes the current game state and makes strategic decisions
- **Game state analysis**: AI receives simplified view of the game field with player, enemies, and bullets
- **Action prediction**: AI chooses between `move_left`, `move_right`, `fire`, or `stop`
- **Adaptive behavior**: AI responds to changing game conditions

### AI Setup Requirements
1. **Install Ollama**: Download from [ollama.com](https://ollama.com/)
2. **Start Ollama service**: Run `ollama serve` in terminal
3. **Pull a model**: Run `ollama pull phi3` (or any compatible model)
4. **Verify connection**: Game will check Ollama availability at `http://localhost:11434`

### How AI Works
1. **Game State Capture**: Current game field is converted to text representation
2. **AI Prompt**: Game state is sent to Ollama with context about game rules
3. **Decision Making**: AI analyzes the situation and returns next action
4. **Action Execution**: Game executes AI's decision (move, shoot, etc.)
5. **Real-time Display**: UI shows current AI action and mode status

## Game Elements

### Player
- **Symbol**: 'A'
- **Color**: Cyan
- **Position**: Bottom area of screen
- **Bullets**: Can fire up to 3 bullets at once

### Enemies
- **Top row (5 enemies)**: `><`, `oo`, `><`, `oo`, `><` (Red)
- **Bottom row (3 enemies)**: `/O\` (Dark Yellow)
- **Movement**: Left-to-right sweep pattern, moving down after each full sweep
- **Shooting**: Only one enemy shoots at a time

### Bullets
- **Player bullets**: '^' (White, moving up)
- **Enemy bullets**: 'v' (White, moving down)

## How to Build and Run

1. Ensure you have .NET 9 or newer installed
2. Navigate to the project directory
3. Build the project:
   ```bash
   dotnet build
   ```
4. Run the game:
   ```bash
   dotnet run
   ```

## Game Flow

1. **Start Screen**: Displays game title, instructions, and speed selection
2. **Speed Selection**: Choose 1 (Slow), 2 (Medium), 3 (Fast), or ENTER for default
3. **Gameplay**: Control your ship, shoot enemies, avoid enemy bullets
4. **Win Condition**: Destroy all enemies
5. **Lose Condition**: Get hit by enemy bullet

## Technical Implementation

### Double-Buffered Rendering
The game uses two character/color buffers to track changes and only updates console positions that have changed, eliminating flicker and improving performance.

### Unicode Support
The game properly sets `Console.OutputEncoding = System.Text.Encoding.UTF8` to ensure Unicode box-drawing characters display correctly across different systems.

### Screenshot System
Screenshots are automatically saved to a `screenshoots` folder as PNG files using `System.Drawing` with proper font rendering.

## Project Structure

- **Program.cs**: Entry point, handles start screen and user input
- **StartScreen.cs**: Displays game title and instructions
- **GameManager.cs**: Main game loop, input handling, rendering, and AI coordination
- **Player.cs**: Player character logic and movement
- **Enemy.cs**: Enemy behavior, movement patterns, and collision detection
- **Bullet.cs**: Bullet physics and collision detection
- **RenderState.cs**: Frame buffer management for double-buffered rendering
- **ScreenshotService.cs**: Screenshot capture and file management
- **OllamaAIService.cs**: Ollama API integration and AI communication
- **GameStateAnalyzer.cs**: Game state analysis and text representation for AI

## AI Performance Notes

- **Response Time**: AI typically responds within 100-500ms depending on model
- **Model Recommendations**: 
  - `phi3`: Fast, good performance
  - `llama3.2`: More strategic, slower
  - `codellama`: Code-aware, good at following rules
- **Fallback Behavior**: If AI is unavailable, game continues in manual mode
- **Error Handling**: AI errors default to "stop" action for safe gameplay

## Requirements

- .NET 9 or newer
- Windows console with Unicode support
- System.Drawing.Common package for screenshot functionality
- **For AI Mode**: [Ollama](https://ollama.com/) running locally at `http://localhost:11434`

## Quick Start

### Manual Play
1. Build and run: `dotnet run`
2. Select game speed (1, 2, 3, or ENTER)
3. Use arrow keys and SPACE to play

### AI Play
1. Install and start Ollama: `ollama serve`
2. Pull a model: `ollama pull phi3`
3. Build and run the game: `dotnet run`
4. Press 'O' during gameplay to enable AI mode
5. Watch the AI play automatically!

## Technical Implementation

### AI Integration Architecture
- **OllamaAIService**: Handles HTTP communication with Ollama API
- **GameStateAnalyzer**: Converts game state to text representation for AI
- **Async AI Processing**: Non-blocking AI decision making
- **Real-time Mode Switching**: Seamless transition between manual and AI control

### AI Prompt Engineering
The AI receives structured game state information including:
- Player position and bullet count
- Enemy positions and patterns  
- Current bullet locations
- Game field representation using ASCII characters
- Strategic context and game rules

### Game State Representation
```
Score: 120, Time: 45s
Player Position: X=25, Y=22
Player Bullets: 2/3

Game Field:
..................................................
......><....oo....><....oo....><................
............/O\......./O\......./O\..............
..................................................
.........................^........................
..................................................
........................A.........................
```

## Notes

- The game automatically creates a `screenshoots` folder for saved screenshots
- All rendering uses proper Unicode characters - no ASCII fallbacks
- Game speed affects both movement and bullet firing rates
- Screenshots preserve the exact visual state including colors and Unicode characters

Enjoy playing Space.AI.NET()!
