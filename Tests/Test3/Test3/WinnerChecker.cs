namespace Test3
{
    public static class WinnerChecker
    {
        public static int Check(char[] field)
        {
            return -1;
        }

        private static int CheckRow(char[] field, int rowNumber)
        {
            int firstPlayerCount = 0;
            int secondPlayerCount = 0;
            for (var i = rowNumber; i < 3; ++i)
            {
                if (field[rowNumber * 3 + i] == 'X')
                {
                    firstPlayerCount++;
                }
                if(field[rowNumber * 3 + i] == 'O')
                {
                    secondPlayerCount++;
                }
            }

            if (firstPlayerCount == 3)
                return 1;
            if (secondPlayerCount == 3)
                return 2;

            return -1;
        }
    }
}
