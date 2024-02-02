namespace HangFireLearn.api
{
    public static class HangfireMocs
    {
        private const int MillisecondsTimeout = 30000;

        public static async Task HelloWorldFireAndForget()
        {
            await Task.Run(() =>
            {
                Thread.Sleep(MillisecondsTimeout);
                Console.WriteLine("Aeeeee o Fire e Forget rodando!!");
            });
        }

        public static async Task HelloWorldFireAndForgetWithError()
        {
            await Task.Run(() =>
            {
                throw new Exception("Simulando um Erro aqui no Job");
            });
        }

        public static async Task HelloWorldRecurringJob()
        {
            await Task.Run(() =>
            {
                Console.WriteLine($"HelloWorldRecurringJob");
            });
        }

        public static async Task HelloWorldDelayedJob()
        {
            await Task.Run(() =>
            {
                Console.WriteLine($"HelloWorldDelayedJob");
            });
        }

        public static async Task<int> HelloWorldContinuationJobPai()
        {
            Random rand = new Random(); 
            int randomNumber = rand.Next(1, 1001);
            await Task.Run(() =>
            {
                Console.WriteLine($"HelloWorldDelayedJob com ID {randomNumber}");
            });
                        
            return randomNumber;
        }

        public static async Task HelloWorldContinuationJobFilha(string jobId)
        {
            await Task.Run(() =>
            {
                Console.WriteLine($"HelloWorldDelayedJob com o ID anterior {jobId}");
            });
        }
    }
}
