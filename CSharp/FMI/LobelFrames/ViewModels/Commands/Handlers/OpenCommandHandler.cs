using LobelFrames.DataStructures.Surfaces;
using LobelFrames.FormatProviders;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;

namespace LobelFrames.ViewModels.Commands.Handlers
{
    public class OpenCommandHandler : CommandHandlerBase
    {
        public OpenCommandHandler(ILobelSceneEditor editor, ISceneElementsManager elementsManager)
            : base(editor, elementsManager)
        {
        }

        public override CommandType Type
        {
            get
            {
                return CommandType.Open;
            }
        }

        public override void BeginCommand()
        {
            base.Editor.ShowHint(Hints.OpenLobelScene);
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = LobelFormatProviders.DialogsFilter;

            if (dialog.ShowDialog() == true)
            {
                string extension = Path.GetExtension(dialog.FileName);

                LobelSceneFormatProviderBase formatProvider;
                if (LobelFormatProviders.TryGetFormatProvider(extension, out formatProvider))
                {
                    LobelScene scene = formatProvider.Import(File.ReadAllBytes(dialog.FileName));
                    base.Editor.LoadScene(scene);
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
