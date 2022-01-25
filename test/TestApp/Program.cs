using System;
using System.Threading.Tasks;
using ProtoBuf.Grpc.Client;
using Service.UserReward.Client;
using Service.UserReward.Grpc.Models;

namespace TestApp
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            GrpcClientFactory.AllowUnencryptedHttp2 = true;

            Console.Write("Press enter to start");
            Console.ReadLine();

            var factory = new UserRewardClientFactory("http://localhost:5001");
            var client = factory.GetUserRewardService();

            Console.WriteLine("End");
            Console.ReadLine();
        }
    }
}
