using System;

namespace ServerCore
{
    // 랜덤 메타 구현
    class Lock
    {
        //톨게이트 같은 방식으로 작동한다. 생성자의 매개변수로 true를 넣으면 초기 상태가 열린 상태이다. false는 반대이다.
        //커널 단의 bool 변수라고 생각하면 된다.
        AutoResetEvent _available = new AutoResetEvent(true);

        public void Acquier()
        {
            //입장을 시도한다.
            _available.WaitOne();
            //_available.Reset(); -> _available을 false로 만들어 주는 메서드이다.
            //WaitOne()메서드에 내부적으로 사용되어 있으므로 구지 안 해도 된다.
        }

        public void Release()
        {
            _available.Set(); //_available을 true로 만들어주는 메서드
        }
    }

    internal class Program
    {
        static int _num = 0;
        static Lock _lock = new Lock();

        static void Thread1()
        {
            for (int i = 0; i < 10000000; i++)
            {
                _lock.Acquier();
                _num++;
                _lock.Release();
            }
        }

        static void Thread2()
        {
            for (int i = 0; i < 10000000; i++)
            {
                _lock.Acquier();
                _num--;
                _lock.Release();
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