using System.ComponentModel;

namespace PathFinding.ViewModel
{
    public class GenerationViewModel: INotifyPropertyChanged
    {
        public static int MAP_WIDTH = 10;
        public static int MAP_HEIGHT = 10;

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

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
