using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace PathFinding
{
    public class AlgoViewModel: INotifyPropertyChanged
    {
        #region Properties
        public char[] Map1D => Map2DToMap1D(_genetic.Map);
        public char[] BestIndividualMap => Map2DToMap1D(_genetic.BestIndividualMap);

        public int PopulationSize
        {
            get
            {
                return _genetic.PopulationSize;
            }
            set
            {
                _genetic.PopulationSize = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PopulationSize)));
            }
        }

        public int IndividualMoveCount
        {
            get
            {
                return _genetic.IndividualMoveCount;
            }
            set
            {
                _genetic.IndividualMoveCount = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IndividualMoveCount)));
            }
        }

        public float SurvivorRate
        {
            get
            {
                return _genetic.SurvivorRate;
            }
            set
            {
                _genetic.SurvivorRate = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SurvivorRate)));
            }
        }

        public float MutationRate
        {
            get
            {
                return _genetic.MutationRate;
            }
            set
            {
                _genetic.MutationRate = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MutationRate)));
            }
        }
        #endregion
        
        private IGenetic _genetic;

        public AlgoViewModel(IGenetic genetic)
        {
            _genetic = genetic;
        }

        private ICommand _clickCommand;

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand ClickCommand
        {
            get
            {
                return _clickCommand ?? (_clickCommand = new CommandHandler(() => MyAction(), true));
            }
        }

        public void MyAction()
        {
            GeneticAlgorithm algo = new GeneticAlgorithm(_genetic);
            algo.Launch();
        }

        public char[] Map2DToMap1D(char[,] map)
        {
            return (from char x in map select x).ToArray();
        }
    }

    public class CommandHandler : ICommand
    {
        private Action _action;
        private bool _canExecute;
        public CommandHandler(Action action, bool canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _action();
        }
    }
}