using System;

namespace Task3
{
    public class MyThreadPool
    {
        public MyThreadPool(int numberOfThreads)
        {
            if (numberOfThreads <= 0)
                throw new ArgumentException("Number of threads must be greater than 0.");

        }


    }
}
