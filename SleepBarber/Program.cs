using System.Text;

namespace SleepingBarber
{
    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            const int maxCustomers = 5;
            const int numChairs = 3;
            var waitingRoom = new Semaphore(numChairs, numChairs);
            var barberChair = new Semaphore(1, 3);
            var barberSleepChair = new Semaphore(0, 3);
            var seatBelt = new Semaphore(3, 3);
            var rnd = new Random();
            bool allDone = false;

            void Barber()
            {
                while (!allDone)
                {
                    Console.WriteLine("Barber sleep...");
                    barberSleepChair.WaitOne();
                    if (!allDone)
                    {
                        Console.WriteLine("Barber working...");
                        Thread.Sleep(rnd.Next(1, 3) * 1000);
                        Console.WriteLine("Barber finished haircut!");
                        seatBelt.Release();
                    }
                    else
                    {
                        Console.WriteLine("Barber sleep...");
                    }
                }
            }

            void Customer(object number)
            {
                int num = (int)number;
                Console.WriteLine("Клиент {0} идет в парикмахерскую...", num);
                Thread.Sleep(rnd.Next(1, 5) * 1000);
                Console.WriteLine("Клиент {0} пришел!", num);
                waitingRoom.WaitOne();
                Console.WriteLine("Клиент {0} заходит в комнату ожидания...", num);
                barberChair.WaitOne();
                waitingRoom.Release();
                Console.WriteLine("Клиент {0} будит парикмахера...", num);
                barberSleepChair.Release();
                seatBelt.WaitOne();
                barberChair.Release();
                Console.WriteLine("Клиент {0} покидает парикмахерскую...", num);
            }

            var barberThread = new Thread(Barber);
            barberThread.Start();
            var customers = new Thread[maxCustomers];

            for (int i = 0; i < maxCustomers; i++)
            {
                customers[i] = new Thread(new ParameterizedThreadStart(Customer));
                customers[i].Start(i + 1);
            }
            for (int i = 0; i < maxCustomers; i++)
            {
                customers[i].Join();
            }

            allDone = true;
            barberSleepChair.Release();
            barberThread.Join();
            Console.WriteLine("End!");
        }
    }
}