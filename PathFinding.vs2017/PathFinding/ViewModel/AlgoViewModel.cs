using System;
using System.Windows.Input;

namespace PathFinding
{
    public class AlgoViewModel
    {
        public static int PopCount = 10;

        public AlgoViewModel()
        {
        }

        private ICommand _clickCommand;
        public ICommand ClickCommand
        {
            get
            {
                return _clickCommand ?? (_clickCommand = new CommandHandler(() => MyAction(), true));
            }
        }

        public void MyAction()
        {
            GeneticAlgorithm algo = new GeneticAlgorithm(new PathFindingGenetic());
            algo.Launch();
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