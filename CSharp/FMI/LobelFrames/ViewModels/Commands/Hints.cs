using System;

namespace LobelFrames.ViewModels.Commands
{
    internal static class Hints
    {
        // No command
        public const string Default = "Избери команда!";

        //Select and Move commands
        public const string SelectMesh = "Избери повърхнина!";
        public const string SelectFirstMovePoint = "Избери начална точка за преместване!";
        public const string SelectSecondMovePoint = "Избери финална точка на преместване!";

        // Open and Save commands
        public const string OpenLobelScene = "Отвори сцена от файл!";
        public const string SaveLobelScene = "Запиши сцената в предпочитан файлов формат!";

        // Cut command
        public const string SelectCutPoint = "Избери точка на рязане!";
        public const string NeighbouringCutPointsShouldBeOnColinearEdges = "Съседните точки на рязане трябва да са свързани с колинеарни ръбове!";
        public const string NextCutPointCannotBeColinearWithPreviousCutPointsCouple = "Точката не трябва да е колинеарна с предните две точки от селекцията!";
        public const string CutSelectionMustBePlanarPolyline = "Селекцията за рязане трябва да бъде равнинна начупена линия!";
        public const string CutSelectionMustBeConvexPolyline = "Селекцията за рязане трябва да бъде изпъкнала начупена линия!";
        public const string SpecifySemiplaneToCut = "Избери точка от полуравнината на рязане!";
        public const string ConfirmOrRejectCutSelection = "Потвърди с Enter или отмени с Escape селекцията за рязане?";
        public const string ThereIsNothingToDeleteWithCurrentSelection = "Няма нищо за рязане с текущата селекция! Променете селекциятa, връщайки назад с Escape!";

        // Fold command
        public const string SelectFirstAxisFirstRotationPoint = "Избери първата точка от първата ос на ротация!";
        public const string SelectFirstAxisSecondRotationPoint = "Избери втората точка от първата ос на ротация!";
        public const string SelectPointFromFirstRotationPlane = "Избери точка от първата равнина на ротация!";
        public const string SelectSecondAxisSecondRotationPointOrPressEnterToRotate = "Избери втора точка от втората ос на ротация или натисни Enter за започване на ротация!";
        public const string SelectPointFromSecondRotationPlane = "Избери точка от втората равнина на ротация!";
        public const string ClickOrInputRotationValue = "Избери ъгъла на ротация с кликане с мишката или въвеждане с клавиатурата!";
        public const string SwitchBetweenPossibleRotationAngles = "Изберете със стрелките една от възможните ротации и натиснете Enter за завършване!";
        public const string RotationPlanePointMustBeConnectedWithColinearEdgesWithAxisFirstRotationPoint = "Точката от равнината за ротация и първата точка от оста трябва да са свързани с колинеарни ръбове!";
        public const string RotationPlanePointCannotBeColinearWithRotationAxis = "Точката от равнината на ротация не може да е колинеарна с оста на ротация!";
        public const string RotationAxisPointsMustBeConnectedWithColinearEdges = "Точките от оста на ротация трябва да са свързани с колинеарни ръбове!";
    }
}
