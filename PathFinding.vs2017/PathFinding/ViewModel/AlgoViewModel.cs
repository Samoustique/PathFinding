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

        private char[] _bestIndividualMap;
        public char[] BestIndividualMap
        {
            get
            {
                return _bestIndividualMap;
            }
            set
            {
                _bestIndividualMap = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(BestIndividualMap)));
            }
        }

        private string _generationNumber;
        public string GenerationNumber
        {
            get
            {
                return _generationNumber;
            }
            set
            {
                _generationNumber = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(GenerationNumber)));
            }
        }

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

        public int GenerationCount
        {
            get
            {
                return _geneticAlgorithm.GenerationCount;
            }
            set
            {
                _geneticAlgorithm.GenerationCount = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(GenerationCount)));
            }
        }
        #endregion

        private IGenetic _genetic;
        private GeneticAlgorithm _geneticAlgorithm;
        public event PropertyChangedEventHandler PropertyChanged;

        public AlgoViewModel(IGenetic genetic)
        {
            _genetic = genetic;
            _geneticAlgorithm = new GeneticAlgorithm(_genetic);
        }

        private ICommand _launchCommand;
        public ICommand LaunchCommand
        {
            get
            {
                return _launchCommand ?? (_launchCommand = new CommandHandler(() => Launch(), true));
            }
        }

        public void Launch()
        {
            _geneticAlgorithm.GenerationChanged += HandleGenerationChanged;
            _geneticAlgorithm.Launch();
        }

        internal void HandleBestIndividualMapChanged(object sender, BestIndividualMapArgs e)
        {
            BestIndividualMap = Map2DToMap1D(e.BestIndividualMap);
        }

        internal void HandleGenerationChanged(object sender, GenerationChangedArgs e)
        {
            GenerationNumber = e.GenerationNumber;
        }

        public static char[] Map2DToMap1D(char[,] map)
        {
            return (from char x in map select x).ToArray();
        }
    }

    public class BestIndividualMapArgs : EventArgs
    {
        private char[,] _bestIndividualMap;
        public char[,] BestIndividualMap
        {
            get { return _bestIndividualMap; }
        }

        public BestIndividualMapArgs(char[,] bestIndividualMap)
        {
            this._bestIndividualMap = bestIndividualMap;
        }
    }

    public class GenerationChangedArgs : EventArgs
    {
        private string _generationNumber;
        public string GenerationNumber
        {
            get { return _generationNumber; }
        }

        public GenerationChangedArgs(string generationNumber)
        {
            this._generationNumber = generationNumber;
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