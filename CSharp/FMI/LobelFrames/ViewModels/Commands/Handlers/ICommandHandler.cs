using LobelFrames.DataStructures.Surfaces.IteractionHandling;
using System;

namespace LobelFrames.ViewModels.Commands.Handlers
{
    public interface ICommandHandler
    {
        CommandType Type { get; }

        void BeginCommand();

        void EndCommand();

        void HandleSurfaceSelected(SurfaceSelectedEventArgs e);

        void HandlePointClicked(PointClickEventArgs e);

        void HandlePointMove(PointEventArgs e);
    }
}
