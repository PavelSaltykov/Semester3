using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Test3
{
    public class ViewModel
    {
        public ObservableCollection<char> Board { get; set; } = new ObservableCollection<char>();

        public ViewModel() => Initialize();

        private void Initialize()
        {
            Board.Clear();
            for (var i = 0; i < 9; ++i)
            {
                Board.Add(' ');
            }
        }

        private bool firstPlayer = true;
        private MakeMoveCommand makeMoveCommand;
        public MakeMoveCommand MakeMoveCommand
        {
            get
            {
                return makeMoveCommand ??= new MakeMoveCommand(index => MakeMove(index),
                    index => Board[index] == ' ');
            }
        }

        private void MakeMove(int index)
        {
            Board[index] = firstPlayer ? 'X' : 'O';
            firstPlayer = !firstPlayer;

            if (WinnerChecker.Check(Board.ToArray()) == -1)
                return;

            firstPlayer = true;
            if (WinnerChecker.Check(Board.ToArray()) == 0)
            {
                MessageBox.Show("Draw.");
                Initialize();
                return;
            }

            var number = WinnerChecker.Check(Board.ToArray()) == 1 ? "First" : "Second";
            MessageBox.Show($"{number} player wins!");
            Initialize();
        }
    }
}
