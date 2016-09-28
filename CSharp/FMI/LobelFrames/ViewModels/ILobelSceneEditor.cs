using Deyo.Controls.Controls3D.Visuals.Overlays2D;
using Deyo.Core.Common.History;
using LobelFrames.FormatProviders;
using LobelFrames.IteractionHandling;
using LobelFrames.ViewModels.Commands;
using System;

namespace LobelFrames.ViewModels
{
    public interface ILobelSceneEditor : IUndoableActionDoer
    {
        InputManager InputManager { get; }
        ILobelSceneContext Context { get; }
        ISurfaceModelingPointerHandler SurfacePointerHandler { get; }
        void EnableSurfacePointerHandler(IteractionHandlingType iteractionType);
        void DisableSurfacePointerHandler();
        void ShowHint(string hint, HintType hintType);
        void CloseCommandContext();
        LobelScene SaveScene();
        void LoadScene(LobelScene scene);
    }
}
