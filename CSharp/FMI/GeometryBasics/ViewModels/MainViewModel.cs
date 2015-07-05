using Deyo.Controls.Common;
using GeometryBasics.Common;
using GeometryBasics.Models;
using GeometryBasics.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GeometryBasics.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private const string ExpandDescriptionHeaderText = "Покажете описанието на алгоритъма...";
        private const string HideDescriptionHeaderText = "Скрийте описанието на алгоритъма...";
        private readonly ObservableCollection<ExampleModelBase> examples;
        private readonly Action<ExampleUserControl> onExampleViewChanged;
        private ExampleModelBase selectedExample;
        private bool isExampleSelected;
        private bool isDescriptionExpanded;
        private string descriptionExpanderHeader;
        private ICommand helpCommand;

        public MainViewModel(Action<ExampleUserControl> onExampleViewChanged)
        {
            this.examples = new ObservableCollection<ExampleModelBase>();
            this.selectedExample = null;
            this.isExampleSelected = false;
            this.isDescriptionExpanded = false;
            this.descriptionExpanderHeader = ExpandDescriptionHeaderText;
            this.onExampleViewChanged = onExampleViewChanged;
            this.helpCommand = new DelegateCommand((parameter) => { this.Help(); });
            this.GenerateExamples();
        }

        public ObservableCollection<ExampleModelBase> Examples
        {
            get
            {
                return this.examples;
            }
        }

        public ExampleModelBase SelectedExample
        {
            get
            {
                return this.selectedExample;
            }
            set
            {
                if (this.selectedExample != value)
                {
                    if (this.selectedExample != null)
                    {
                        this.selectedExample.View.Release();
                    }

                    this.selectedExample = value;

                    if (this.selectedExample != null)
                    {
                        this.selectedExample.View.Initialize();
                    }

                    this.IsExampleSelected = value != null;
                    this.onExampleViewChanged(value == null ? null : value.View);
                    this.OnPropertyChanged();
                }
            }
        }

        public bool IsExampleSelected
        {
            get
            {
                return this.isExampleSelected;
            }
            set
            {
                this.SetProperty(ref this.isExampleSelected, value);
            }
        }

        public bool IsDescriptionExpanded
        {
            get
            {
                return this.isDescriptionExpanded;
            }
            set
            {
                if (this.SetProperty(ref isDescriptionExpanded, value))
                {
                    this.DescriptionExpanderHeader = value ? HideDescriptionHeaderText : ExpandDescriptionHeaderText;   
                }
            }
        }

        public string DescriptionExpanderHeader
        {
            get
            {
                return this.descriptionExpanderHeader;
            }
            set
            {
                this.SetProperty(ref this.descriptionExpanderHeader, value);
            }
        }

        public ICommand HelpCommand
        {
            get
            {
                return this.helpCommand;
            }
            set
            {
                this.SetProperty(ref this.helpCommand, value);
            }
        }

        public void InitializeExamples()
        {
            this.SelectedExample = null;

            foreach (ExampleModelBase example in this.examples)
            {
                example.CanCreateView = true;
            }
        }

        private void GenerateExamples()
        {
            this.examples.Add(new GrahamConvexHullModel());
            this.examples.Add(new ConvexPolygonesIntersectionModel());
            this.examples.Add(new RotatingCalipersModel());
            this.examples.Add(new PointLocalizationModel());
            this.examples.Add(new LineSegmentsIntersectionModel());
            this.examples.Add(new ClippingAlgorithmModel());
            this.examples.Add(new OrthographicVisibilityModel());
            this.examples.Add(new PerspectiveVisibilityModel());
        }

        private void Help()
        {
            MessageBox.Show(@"Може да манипулирате по следните начини демонстрационното поле:
 - ПРЕМЕСТВАНЕ => Може да местите видимата област, задържайки произволен бутон на мишката и движейки мишката.
 - МАЩАБИРАНЕ => Може да променята мащабирането на видимата област, въртейки колелцето на мишката.
 - ДОБАВЯНЕ НА ТОЧКИ => Когато сте в режим ""Селекция"" може да добавяте точки в полето с левия бутон на мишката.",
"Помощ");
        }
    }
}
