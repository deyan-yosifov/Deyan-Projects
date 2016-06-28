using LobelFrames.DataStructures.Surfaces;
using LobelFrames.FormatProviders;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;

namespace LobelFrames.ViewModels.Commands.Handlers
{
    public class SaveCommandHandler : CommandHandlerBase
    {
        public SaveCommandHandler(ILobelSceneEditor editor, ISceneElementsManager elementsManager)
            : base(editor, elementsManager)
        {
        }

        public override CommandType Type
        {
            get
            {
                return CommandType.Save;
            }
        }

        public override void BeginCommand()
        {
            base.Editor.ShowHint(Hints.SaveLobelScene, HintType.Info);
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = LobelFormatProviders.DialogsFilter;

            if (dialog.ShowDialog() == true)
            {
                string extension = Path.GetExtension(dialog.FileName);

                LobelSceneFormatProviderBase formatProvider;
                if (LobelFormatProviders.TryGetFormatProvider(extension, out formatProvider))
                {
                    LobelScene scene = base.Editor.SaveScene();
                    File.WriteAllBytes(dialog.FileName, formatProvider.Export(scene));
                }
                else
                {
                    MessageBox.Show("Невалидно файлово разширение: " + extension);
                }
            }

            base.Editor.CloseCommandContext();
        }
    }
}
