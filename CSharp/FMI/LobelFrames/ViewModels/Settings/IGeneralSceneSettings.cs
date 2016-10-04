using System;

namespace LobelFrames.ViewModels.Settings
{
    public interface IGeneralSceneSettings
    {
        int HistoryStackSize { get; }
        event EventHandler HistoryStackSizeChanged;
    }
}
