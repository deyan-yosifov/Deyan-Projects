using Deyo.Controls.Common;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace LobelFrames.ViewModels.Commands
{
    public class CommandDescriptors
    {
        private const string HelpText = @"Това приложение позволява моделирането на повърхнини състоящи се от еднакви равностранни триъгълници, наречени още повърхнини на Лобел.";
        private readonly Dictionary<CommandType, CommandDescriptor> descriptors;

        public CommandDescriptors()
        {
            this.descriptors = new Dictionary<CommandType, CommandDescriptor>();

            this.RegisterCommandDescriptor(CommandType.Open, null, true);
            this.RegisterCommandDescriptor(CommandType.Save, null, true);
            this.RegisterCommandDescriptor(CommandType.Undo, null, false);
            this.RegisterCommandDescriptor(CommandType.Redo, null, false);
            this.RegisterCommandDescriptor(CommandType.Help, new DelegateCommand(ShowHelpMessage), true);
            this.RegisterCommandDescriptor(CommandType.Test, new DelegateCommand(TestAction), true); 
        }

        public CommandDescriptor this[CommandType type]
        {
            get
            {
                return this.descriptors[type];
            }
        }

        private void RegisterCommandDescriptor(CommandType type, ICommand command, bool isEnabled)
        {
            command = command ?? new DelegateCommand((p) => this.ShowNotImplementedCommand(type));
            this.descriptors.Add(type, new CommandDescriptor(command, isEnabled));
        }

        private void ShowHelpMessage(object obj)
        {
            MessageBox.Show(HelpText, "Помощ");
        }

        private void ShowNotImplementedCommand(CommandType type)
        {
            MessageBox.Show(string.Format("Not Implemented Command: {0}", type));
        }

        private void TestAction(object obj)
        {
            MessageBox.Show("Test action!");
        }
    }
}
