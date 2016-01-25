using LobelFrames.DataStructures.Surfaces;
using System;
using System.Collections.Generic;

namespace LobelFrames.ViewModels.Commands
{
    public class CommandStateEvaluator
    {
        private readonly SurfaceModelingContext context;
        private readonly Dictionary<CommandType, Func<SurfaceModelingContext, bool>> commandTypesIsEnabledEvaluators;

        public CommandStateEvaluator(SurfaceModelingContext context)
        {
            this.context = context;
            this.commandTypesIsEnabledEvaluators = new Dictionary<CommandType, Func<SurfaceModelingContext, bool>>();

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

        private SurfaceModelingContext Context
        {
            get
            {
                return this.context;
            }
        }

        public bool EvaluateIsEnabled(CommandType type)
        {
            if (this.Context.HistoryManager.IsCreatingUndoGroup)
            {
                return type == CommandType.Help;
            }
            else
            {
                return this.commandTypesIsEnabledEvaluators[type](this.Context);
            }
        }

        private void RegisterEvaluator(CommandType type, Func<SurfaceModelingContext, bool> evaluateIsEnabled)
        {
            this.commandTypesIsEnabledEvaluators.Add(type, evaluateIsEnabled);
        }

        private static bool IsAlwaysEnabled(SurfaceModelingContext context)
        {
            return true;
        }

        private static bool IsUndoEnabled(SurfaceModelingContext context)
        {
            return context.HistoryManager.CanUndo;
        }

        private static bool IsRedoEnabled(SurfaceModelingContext context)
        {
            return context.HistoryManager.CanRedo;
        }

        private static bool AreMeshesDeselected(SurfaceModelingContext context)
        {
            return context.SelectedSurface == null && context.HasSurfaces;
        }

        private static bool IsMeshSelected(SurfaceModelingContext context)
        {
            return context.SelectedSurface != null;
        }

        private static bool IsLobelSurfaceSelected(SurfaceModelingContext context)
        {
            return context.SelectedSurface != null && context.SelectedSurface.Type == SurfaceType.Lobel;
        }

        private static bool IsBezierSurfaceSelected(SurfaceModelingContext context)
        {
            return context.SelectedSurface != null && context.SelectedSurface.Type == SurfaceType.Bezier;
        }
    }
}
