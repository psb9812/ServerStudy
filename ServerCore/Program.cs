using System;

namespace ServerCore
{
    // 랜덤 메타 구현
    class SpinLock
    {
        volatile int _locked = 0;

        public void Acquier()
        {
            while (true)
            {
                int expected = 0;
                int desired = 1;
                if (Interlocked.CompareExchange(ref _locked, desired, expected) == expected)
                {
                    break;
                }

                //락을 취득하지 못했을 때 어떻게 처리하느냐에 따라 구현 방법이 나뉜다.
                //랜덤 메타의 경우 쉬다올게 ~하는 느낌의 세가지 방법
                //Thread.Sleep(0);
                //Thread.Sleep(1);
                Thread.Yield();
            }
        }

        public void Release()
        {
            _locked = 0;
        }
    }

    internal class Program
    {
        static int _num = 0;
        static SpinLock spinLock = new SpinLock();

        static void Thread1()
        {
            for (int i = 0; i < 10000; i++)
            {
                spinLock.Acquier();
                _num++;
                spinLock.Release();
            }
        }

        static void Thread2()
        {
            for (int i = 0; i < 10000; i++)
            {
                spinLock.Acquier();
                _num--;
                spinLock.Release();
            }
        }

        static void Main(string[] args)
        {
            Task task1 = new Task(Thread1);
            Task task2 = new Task(Thread2);
            
            task1.Start();
            task2.Start();
            
            Task.WaitAll(task1, task2);

            Console.WriteLine(_num);

            Console.ReadLine();
        }
    }
}