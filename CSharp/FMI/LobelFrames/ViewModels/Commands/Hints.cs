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

        // Lobel commands common
        public const string NextPointCannotBeColinearWithPreviousPointsCouple = "Точката не трябва да е колинеарна с предните две точки от селекцията!";
        public const string NeighbouringPointsShouldBeOnColinearEdges = "Съседните точки трябва да са свързани с колинеарни ръбове!";

        // Cut command
        public const string SelectCutPoint = "Избери точка на рязане!";
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
        public const string SwitchBetweenPossibleRotationAngles = "Сменяйте с кликане възможните позиции на ротация или натиснете Enter за завършване!";
        public const string RotationPlanePointMustBeConnectedWithColinearEdgesWithAxisFirstRotationPoint = "Точката от равнината за ротация и първата точка от оста трябва да са свързани с колинеарни ръбове!";
        public const string RotationPlanePointCannotBeColinearWithRotationAxis = "Точката от равнината на ротация не може да е колинеарна с оста на ротация!";
        public const string RotationAxisPointsMustBeConnectedWithColinearEdges = "Точките от оста на ротация трябва да са свързани с колинеарни ръбове!";
        public const string InvalidRotationAngleParameterValue = "Невалиден параметър! Ъгълът на ротация трябва да е десетично число!";
        public const string FoldMeshPatchesCannotIntersect = "Двата региона на огъване не могат да се пресичат взаимно!";
        public const string RotationAxisesCannotBeColinear = "Двете оси на огъване не може да са колинеарни!";
        public const string NoPossibleFoldingPositions = "С текущата селекция няма възможни ъгли на огъване. Сменете селекцията с Escape.";

        // Glue command
        public const string SelectFirstGluePoint = "Избери първата точка на лепене!";
        public const string SelectSecondGluePoint = "Избери втората точка на лепене!";
        public const string SelectGlueDirectionPoint = "Избери точка от полуравнината на новодобавените триъгълници!";
        public const string InputNumberOfGlueRowsToAddAndPressEnter = "Въведи различен брой на редове с триъгълници или потвърди текущия брой с Enter!";
        public const string InvalidGlueRowsParameterValue = "Невалиден параметър! Броят на редове с триъгълници трябва да е цяло положително число!";

        // Approximate command
        public const string ApproximatingPleaseWait = "Апроксимиране на провърхнината. Моля изчакайте...";
        public const string FinishOrChooseOtherApproximation = "Завърши апроксимирането с Enter или започни ново, въвеждайки страна на триъгълника!";
        public const string StartApproximation = "Започни апроксимиране, въвеждайки страна на триъгълника.";
        public const string TriangleSideMustBePositive = "Страната на триъгълника трябва да бъде положително число!";
    }
}
