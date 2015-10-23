using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;
using RestSharp;

namespace JustGiving.Film.Modules
{
    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            Get["/home"] = x =>
                {
                    return View["home", GetVotes()];
                };
        }

        public FundraisingPageDonations GetFundraisingPageDonations()
        {
            RestClient client = new RestClient("https://api.justgiving.com/0f938d22/v1/fundraising/pages/justmovies/donations");
            var request = new RestRequest(Method.GET);
            request.AddHeader("Accept", "application/json");
            var result = client.Get<FundraisingPageDonations>(request);
            return result.Data;
        }

        public class FundraisingPageDonations
        {
            public List<FundraisingPageDonation> Donations { get; set; }

            public FundraisingPageDonations()
            {
                Donations = new List<FundraisingPageDonation>();
            }
        }

        public class FundraisingPageDonation
        {
            public DateTime DonationDate { get; set; }
            public string Message { get; set; }
        }

        public List<string> GetMovies()
        {
            var result = new List<string> { "Dead Man's Shoes", "Elf", "Somers Town" };
            return result;
        }

        public IEnumerable<Vote> GetVotes()
        {
            var votes = GetFundraisingPageDonations().Donations;
            var candidates = GetMovies();
            var result = new List<string>();
            foreach (var vote in votes)
            {
                if (!string.IsNullOrWhiteSpace(vote.Message))
                {
                    var movie = IsMovie(vote.Message);
                    result.Add(movie);
                }
            }
            var votesResults = new List<Vote>();
            foreach (var group in  result.GroupBy(x => x))
            {
                votesResults.Add(new Vote()
                    {
                        Name = group.First(),
                        Votes = group.Count()
                    });
            }
            return votesResults;
        }

        public string IsMovie(string vote)
        {
            var candidates = GetMovies();
            foreach (var candidate in candidates)
            {
                if (vote.Contains(candidate))
                {
                    return candidate;
                }
            }
            return "Empty";
        }

        public class Vote
        {
            public string Name { get; set; }
            public int Votes { get; set; }
        }
    }
}