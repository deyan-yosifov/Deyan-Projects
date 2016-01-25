using System;
using System.Windows;

namespace LobelFrames.DataStructures.Surfaces.IteractionHandling
{
    public interface IIteractionHandler
    {
        IteractionHandlingType IteractionType { get; }
        bool TryHandleClick(Point viewportPosition);
        bool TryHandleMove(Point viewportPosition);
    }
}
