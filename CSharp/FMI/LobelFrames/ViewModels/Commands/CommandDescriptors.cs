using Deyo.Controls.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace LobelFrames.ViewModels.Commands
{
    public class CommandDescriptors
    {
        private const string HelpText = @"Това приложение позволява моделирането на повърхнини състоящи се от еднакви равностранни триъгълници, наречени още повърхнини на Лобел.";
        private readonly Dictionary<CommandType, CommandDescriptor> descriptors;
        private readonly SurfaceModelingViewModel viewModel;
        private readonly CommandStateEvaluator stateEvaluator;

        public CommandDescriptors(SurfaceModelingViewModel viewModel)
        {
            this.viewModel = viewModel;
            this.stateEvaluator = new CommandStateEvaluator(this.viewModel.Context);
            this.descriptors = new Dictionary<CommandType, CommandDescriptor>();
            this.RegisterCommandDescriptors();
            this.UpdateCommandStates();
        }

        public CommandDescriptor this[CommandType type]
        {
            get
            {
                return this.descriptors[type];
            }
        }

        public void UpdateCommandStates()
        {
            foreach (KeyValuePair<CommandType, CommandDescriptor> commandTypeToDescriptor in this.descriptors)
            {
                commandTypeToDescriptor.Value.IsEnabled = this.stateEvaluator.EvaluateIsEnabled(commandTypeToDescriptor.Key);
            }
        }

        private void RegisterCommandDescriptors()
        {
            this.RegisterCommandDescriptor(CommandType.Open, this.viewModel.Open);
            this.RegisterCommandDescriptor(CommandType.Save, this.viewModel.Save);
            this.RegisterCommandDescriptor(CommandType.Undo, this.viewModel.Undo);
            this.RegisterCommandDescriptor(CommandType.Redo, this.viewModel.Redo);
            this.RegisterCommandDescriptor(CommandType.Settings, this.viewModel.ChangeGeneralSettings);

            this.RegisterCommandDescriptor(CommandType.SelectMesh, this.viewModel.SelectMesh);
            this.RegisterCommandDescriptor(CommandType.DeselectMesh, this.viewModel.Deselect);
            this.RegisterCommandDescriptor(CommandType.MoveMesh, this.viewModel.MoveMesh);
            this.RegisterCommandDescriptor(CommandType.DeleteMesh, this.viewModel.DeleteMesh);

            this.RegisterCommandDescriptor(CommandType.AddLobelMesh, this.viewModel.AddLobelMesh);
            this.RegisterCommandDescriptor(CommandType.CutMesh, null);
            this.RegisterCommandDescriptor(CommandType.FoldMesh, null);
            this.RegisterCommandDescriptor(CommandType.GlueMesh, null);
            this.RegisterCommandDescriptor(CommandType.LobelSettings, this.viewModel.ChangeLobelSettings);

            this.RegisterCommandDescriptor(CommandType.AddBezierSurface, null);
            this.RegisterCommandDescriptor(CommandType.ApproximateWithLobelMesh, null);
            this.RegisterCommandDescriptor(CommandType.BezierSettings, this.viewModel.ChangeBezierSettings);

            this.RegisterCommandDescriptor(CommandType.Test, this.TestAction, false);

            this.RegisterCommandDescriptor(CommandType.Help, this.ShowHelpMessage);
        }

        private void RegisterCommandDescriptor(CommandType type, Action commandAction)
        {
            this.RegisterCommandDescriptor(type, commandAction, true);
        }

        private void RegisterCommandDescriptor(CommandType type, Action commandAction, bool initialIsVisibleState)
        {
            ICommand command = commandAction == null ?
                new DelegateCommand((p) =>
                    {
                        this.viewModel.BeforeCommandExecuted(type);
                        this.ShowNotImplementedCommand(type);
                    }) :
                new DelegateCommand((p) =>
                    {
                        this.viewModel.BeforeCommandExecuted(type);
                        commandAction();
                    });

            this.descriptors.Add(type, new CommandDescriptor(command) { IsVisible = initialIsVisibleState });
        }

        private void ShowHelpMessage()
        {
            MessageBox.Show(HelpText, "Помощ");
        }

        private void TestAction()
        {
            MessageBox.Show("Test action!");
            this.viewModel.HintManager.Hint = "Подсказка сменена!";
            this.viewModel.InputManager.IsEnabled = true;
        }

        private void ShowNotImplementedCommand(CommandType type)
        {
            MessageBox.Show(string.Format("Not Implemented Command: {0}", type));
        }
    }
}
