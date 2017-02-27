using System;
using System.Linq;
using System.Threading.Tasks;
using NServiceBus;

class Program
{
    static void Main()
    {
        AsyncMain().GetAwaiter().GetResult();
    }

    static async Task AsyncMain()
    {
        var endpointConfiguration = new EndpointConfiguration("Voter");

        var transportConfig = endpointConfiguration.ApplyCommonConfiguration();

        #region ConfigureSenderSideRouting-Voter

        var remotePartitions = new[] { "John", "Abby" };

        var distributionConfig = transportConfig.Routing()
            .RegisterPartitionedDestinationEndpoint("CandidateVoteCount", remotePartitions);

        distributionConfig.AddPartitionMappingForMessageType<CloseElection>(message => message.Candidate);

        #endregion

        if (MessageDrivenPubSub.Enabled)
        {
            #region ConfigureSenderSideRouting-MessageDrivenPubSub

            distributionConfig.AddPartitionMappingForMessageType<VotePlaced>(message => message.Candidate);

            #endregion
        }

        var endpointInstance = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);

        Console.WriteLine("Press Enter to start election");
        Console.WriteLine("Press Esc to stop election");
        Console.ReadLine();

        var random = new Random();
        var votedZipCode = Enumerable.Range(1, 10).Select(x => random.Next(0, 99001).ToString("d5")).ToArray();

        do
        {
            while (!Console.KeyAvailable)
            {
                var choice = random.Next(0, 2);
                var candidate = remotePartitions[choice % 2];
                var zipcode = votedZipCode[random.Next(0, 10)];

                Console.WriteLine($"Voted for {candidate} from zip code {zipcode}");

                await endpointInstance.Publish(new VotePlaced
                {
                    Candidate = candidate,
                    ZipCode = zipcode
                }).ConfigureAwait(false);

                await Task.Delay(1000).ConfigureAwait(false);
            }
        } while (Console.ReadKey(true).Key != ConsoleKey.Escape);

        Console.WriteLine("Closing election");

        await endpointInstance.Send(new CloseElection { Candidate = remotePartitions[0] }).ConfigureAwait(false);
        await endpointInstance.Send(new CloseElection { Candidate = remotePartitions[1] }).ConfigureAwait(false);

        await endpointInstance.Stop().ConfigureAwait(false);
    }
}