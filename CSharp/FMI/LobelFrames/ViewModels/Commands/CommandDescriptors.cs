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
        private const bool DefaultIsEnabledInitialState = true;
        private const bool DefaultIsVisibleInitialState = true;
        private readonly Dictionary<CommandType, CommandDescriptor> descriptors;
        private readonly SurfaceModelingViewModel viewModel;

        public CommandDescriptors(SurfaceModelingViewModel viewModel)
        {
            this.viewModel = viewModel;
            this.descriptors = new Dictionary<CommandType, CommandDescriptor>();

            this.RegisterCommandDescriptor(CommandType.Open, null);
            this.RegisterCommandDescriptor(CommandType.Save, null);
            this.RegisterCommandDescriptor(CommandType.Undo, null, false);
            this.RegisterCommandDescriptor(CommandType.Redo, null, false);

            this.RegisterCommandDescriptor(CommandType.SelectMesh, null);
            this.RegisterCommandDescriptor(CommandType.DeselectMesh, null, false);
            this.RegisterCommandDescriptor(CommandType.MoveMesh, null, false);
            this.RegisterCommandDescriptor(CommandType.DeleteMesh, null, false);

            this.RegisterCommandDescriptor(CommandType.AddLobelMesh, new DelegateCommand((p) => this.viewModel.AddLobelMesh()));
            this.RegisterCommandDescriptor(CommandType.CutMesh, null, false);
            this.RegisterCommandDescriptor(CommandType.FoldMesh, null, false);
            this.RegisterCommandDescriptor(CommandType.GlueMesh, null, false);
            this.RegisterCommandDescriptor(CommandType.LobelSettings, null);

            this.RegisterCommandDescriptor(CommandType.AddBezierSurface, null);
            this.RegisterCommandDescriptor(CommandType.ApproximateWithLobelMesh, null, false);
            this.RegisterCommandDescriptor(CommandType.BezierSettings, null);

            this.RegisterCommandDescriptor(CommandType.Settings, null);

            this.RegisterCommandDescriptor(CommandType.Test, new DelegateCommand(TestAction), true, false); 

            this.RegisterCommandDescriptor(CommandType.Help, new DelegateCommand(ShowHelpMessage));
        }

        public CommandDescriptor this[CommandType type]
        {
            get
            {
                return this.descriptors[type];
            }
        }

        private void RegisterCommandDescriptor(CommandType type, ICommand command)
        {
            this.RegisterCommandDescriptor(type, command, DefaultIsEnabledInitialState, DefaultIsVisibleInitialState);
        }

        private void RegisterCommandDescriptor(CommandType type, ICommand command, bool initialIsEnabledState)
        {
            this.RegisterCommandDescriptor(type, command, initialIsEnabledState, DefaultIsVisibleInitialState);
        }

        private void RegisterCommandDescriptor(CommandType type, ICommand command, bool initialIsEnabledState, bool initialIsVisibleState)
        {
            command = command ?? new DelegateCommand((p) => this.ShowNotImplementedCommand(type));
            this.descriptors.Add(type, new CommandDescriptor(command, initialIsEnabledState, initialIsVisibleState));
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
            this.viewModel.HintManager.Hint = "Подсказка сменена!";
            this.viewModel.InputManager.IsEnabled = true;
        }
    }
}
