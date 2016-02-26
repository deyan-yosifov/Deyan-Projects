using LobelFrames.DataStructures.Surfaces;
using System;
using System.Collections.Generic;

namespace LobelFrames.ViewModels.Commands
{
    public class CommandStateEvaluator
    {
        private readonly ILobelSceneContext context;
        private readonly Dictionary<CommandType, Func<ILobelSceneContext, bool>> commandTypesIsEnabledEvaluators;

        public CommandStateEvaluator(ILobelSceneContext context)
        {
            this.context = context;
            this.commandTypesIsEnabledEvaluators = new Dictionary<CommandType, Func<ILobelSceneContext, bool>>();

            this.RegisterEvaluator(CommandType.Open, CommandStateEvaluator.IsAlwaysEnabled);
            this.RegisterEvaluator(CommandType.Save, CommandStateEvaluator.IsAlwaysEnabled);
            this.RegisterEvaluator(CommandType.Undo, CommandStateEvaluator.IsUndoEnabled);
            this.RegisterEvaluator(CommandType.Redo, CommandStateEvaluator.IsRedoEnabled);
            this.RegisterEvaluator(CommandType.Settings, CommandStateEvaluator.IsAlwaysEnabled);
            
            this.RegisterEvaluator(CommandType.SelectMesh, CommandStateEvaluator.AreMeshesDeselected);
            this.RegisterEvaluator(CommandType.DeselectMesh, CommandStateEvaluator.IsMeshSelected);
            this.RegisterEvaluator(CommandType.MoveMesh, CommandStateEvaluator.IsMeshSelected);
            this.RegisterEvaluator(CommandType.DeleteMesh, CommandStateEvaluator.IsMeshSelected);
            
            this.RegisterEvaluator(CommandType.AddLobelMesh, CommandStateEvaluator.IsAlwaysEnabled);
            this.RegisterEvaluator(CommandType.CutMesh, CommandStateEvaluator.IsLobelSurfaceSelected);
            this.RegisterEvaluator(CommandType.FoldMesh, CommandStateEvaluator.IsLobelSurfaceSelected);
            this.RegisterEvaluator(CommandType.GlueMesh, CommandStateEvaluator.IsLobelSurfaceSelected);
            this.RegisterEvaluator(CommandType.LobelSettings, CommandStateEvaluator.IsAlwaysEnabled);
            
            this.RegisterEvaluator(CommandType.AddBezierSurface, CommandStateEvaluator.IsAlwaysEnabled);
            this.RegisterEvaluator(CommandType.ApproximateWithLobelMesh, CommandStateEvaluator.IsBezierSurfaceSelected);
            this.RegisterEvaluator(CommandType.BezierSettings, CommandStateEvaluator.IsAlwaysEnabled);
            
            this.RegisterEvaluator(CommandType.Test, CommandStateEvaluator.IsAlwaysEnabled);
            this.RegisterEvaluator(CommandType.Help, CommandStateEvaluator.IsAlwaysEnabled);
        }

        private ILobelSceneContext Context
        {
            get
            {
                return this.context;
            }
        }

        public bool EvaluateIsEnabled(CommandType type)
        {
            if (this.Context.HasActiveCommand)
            {
                return type == CommandType.Help;
            }
            else
            {
                return this.commandTypesIsEnabledEvaluators[type](this.Context);
            }
        }

        private void RegisterEvaluator(CommandType type, Func<ILobelSceneContext, bool> evaluateIsEnabled)
        {
            this.commandTypesIsEnabledEvaluators.Add(type, evaluateIsEnabled);
        }

        private static bool IsAlwaysEnabled(ILobelSceneContext context)
        {
            return true;
        }

        private static bool IsUndoEnabled(ILobelSceneContext context)
        {
            return context.HasActionToUndo;
        }

        private static bool IsRedoEnabled(ILobelSceneContext context)
        {
            return context.HasActionToRedo;
        }

        private static bool AreMeshesDeselected(ILobelSceneContext context)
        {
            return context.SelectedSurface == null && context.HasSurfaces;
        }

        private static bool IsMeshSelected(ILobelSceneContext context)
        {
            return context.SelectedSurface != null;
        }

        private static bool IsLobelSurfaceSelected(ILobelSceneContext context)
        {
            return context.SelectedSurface != null && context.SelectedSurface.Type == SurfaceType.Lobel;
        }

        private static bool IsBezierSurfaceSelected(ILobelSceneContext context)
        {
            return context.SelectedSurface != null && context.SelectedSurface.Type == SurfaceType.Bezier;
        }
    }
}
