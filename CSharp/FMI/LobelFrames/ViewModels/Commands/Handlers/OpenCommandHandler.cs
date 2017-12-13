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
            base.Editor.ShowHint(Hints.OpenLobelScene, HintType.Info);
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = LobelFormatProviders.DialogsFilter;
            dialog.InitialDirectory = LobelFormatProviders.GetFullPath(@"Resources\Sample files\");

            if (dialog.ShowDialog() == true)
            {
                string extension = Path.GetExtension(dialog.FileName);

                LobelSceneFormatProviderBase formatProvider;
                if (LobelFormatProviders.TryGetFormatProvider(extension, out formatProvider))
                {
                    try
                    {
                        LobelScene scene = formatProvider.Import(File.ReadAllBytes(dialog.FileName));
                        base.Editor.LoadScene(scene);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(string.Format("Грешка по време на отварянето на файлa: \"{0}\"", e.Message), "Грешка!");
                    }
                }
                else
                {
                    MessageBox.Show("Невалидно файлово разширение: " + extension, "Грешка!");
                }
            }

            base.Editor.CloseCommandContext();
        }
    }
}
