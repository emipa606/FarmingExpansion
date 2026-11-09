# GitHub Copilot Instructions for Farming Expansion

Welcome to the Farming Expansion mod for RimWorld! This mod adds events and chemicals to enhance the farming experience for players. This guide provides an overview of the mod's purpose, key features, coding patterns, XML integration, and harmony patching.

## Mod Overview and Purpose
The Farming Expansion mod introduces various events and chemicals to the farming aspects of RimWorld. Players will encounter pests and weeds that threaten their crops, and they can use different chemicals to prevent these events and boost their farm productivity.

## Key Features and Systems
- **Events**:
  - Pests: Insects that consume crops.
  - Weeds: Fast-growing plants that destroy crops.
  
- **Chemicals**:
  - Pesticide (Tribal): Prevents Pests events.
  - Herbicide (Tribal): Prevents Weeds events.
  - Fungicide (Tribal): Prevents Blight events.
  - DDT (Industrial): Prevents all events but lowers yield.
  - Chemical Spray (Spacer): Prevents all events, increases yield, and growth speed.

- **Integration**:
  - The mod integrates with several other mods to enhance functionality and compatibility.
  
## Coding Patterns and Conventions
The mod follows standard RimWorld coding practices with a focus on clarity and maintainability.

- **C# Files**:
  - `ChemicalAddedCheck.cs`: Manages chemical effects on game components.
  - `Zone_Growing_GetGizmos.cs`: Modifies the growing zone gizmos to include chemical usage.
  - `IncidentWorker_CropEaters.cs`: Handles the Pests event logic.
  - `IncidentWorker_Weeds.cs`: Manages the Weeds event logic.
  - `UseChemicalJobDriver.cs`: Manages the chemical application job.

- **XML Files**:
  - `IncidentsDefs.xml`, `JobDriverDef.xml`, `RecipeDefs.xml`, and others: Define mod-specific game elements such as incidents, job drivers, and recipes.

## XML Integration
XML files are used to define game elements and integrate with existing RimWorld systems. Key XML files include:

- `IncidentsDefs.xml`: Defines new incident types like Pests and Weeds.
- `JobDriverDef.xml`: Defines new job drivers for chemical application.
- `RecipeDefs.xml`: Defines new recipes for chemicals and their effects.

## Harmony Patching
The mod uses Harmony to patch existing RimWorld methods to integrate new features seamlessly. Key patches include:

- `Zone_Growing_GetGizmos.cs`: Modifies the `GetGizmos` method in the `Zone_Growing` class to add chemical application options.

## Suggestions for Copilot
To help with contributing to the Farming Expansion mod, here are some suggestions for using GitHub Copilot:

1. **Understand the Codebase**: Familiarize yourself with the existing code and XML files to understand how the mod works.
2. **Follow Coding Conventions**: Use the established coding patterns and conventions to ensure consistency.
3. **Test Changes**: Always test changes in a safe environment to ensure they do not break existing functionality.
4. **Contribute Documentation**: Document new features and changes to help other contributors.
5. **Collaborate with Other Developers**: Work with other modders to ensure compatibility and integration.

Feel free to reach out to the mod author or other contributors if you have any questions or need assistance with your contributions. Happy coding!

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.


## Hard rules (must follow)
- Do NOT run commands that modify the repo (no git commit, git apply, dotnet format) unless explicitly asked.
- Prefer minimal reads: read only the smallest code region needed (around the suspicious lines).
- When mentioning SonarQube issues, automatically use the SonarQube MCP service to fetch and address issues instead of making inferred fixes without querying SonarQube first.
- When mentioning the rimworld log, automatically use the Rimworld MCP service to fetch the log.

