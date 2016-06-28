using System;

namespace LobelFrames.ViewModels.Commands
{
    internal static class Hints
    {
        public const string Default = "Избери команда!";
        public const string SelectMesh = "Селектирай повърхнина!";
        public const string SelectFirstMovePoint = "Селектирай начална точка за преместване!";
        public const string SelectSecondMovePoint = "Селектирай финална точка на преместване!";
        public const string OpenLobelScene = "Отвори сцена от файл!";
        public const string SaveLobelScene = "Запиши сцената в предпочитан файлов формат!";
        public const string SelectCutPoint = "Селектирай точка на рязане!";
        public const string NeighbouringCutPointsShouldBeOnColinearEdges = "Съседните точки на рязане трябва да са свързани с колинеарни ръбове!";
        public const string NextCutPointCannotBeColinearWithPreviousCutPointsCouple = "Точката не трябва да е колинеарна с предните две точки от селекцията!";
        public const string CutSelectionMustBePlanarPolyline = "Селекцията за рязане трябва да бъде равнинна начупена линия!";
        public const string CutSelectionMustBeConvexPolyline = "Селекцията за рязане трябва да бъде изпъкнала начупена линия!";
    }
}
