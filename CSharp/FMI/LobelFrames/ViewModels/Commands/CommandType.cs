using System;

namespace LobelFrames.ViewModels.Commands
{
    public enum CommandType
    {
        // Basic files and commands.
        Open,
        Save,
        Undo,
        Redo,

        // Common mesh commands
        SelectMesh,
        DeselectMesh,
        MoveMesh,
        DeleteMesh,

        // Lobel mesh commands
        AddLobelMesh,
        CutMesh,
        FoldMesh,
        GlueMesh,
        LobelSettings,

        // Bezier surface commands
        AddBezierSurface,
        ApproximateWithLobelMesh,
        BezierSettings,
        
        // General settings command
        Settings,
        
        // Test command.
        Test,

        // Help command.
        Help,
    }
}
