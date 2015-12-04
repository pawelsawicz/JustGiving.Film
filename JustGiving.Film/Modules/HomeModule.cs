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

            Get["/"] = x =>
                {
                    return View["Index.cshtml", GetVotes()];
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
            var result = new List<string> { "Home Alone", "Die Hard", "It a wonderful life" };
            return result;
        }

        public IEnumerable<Vote> GetVotes()
        {
            var votes =
                GetFundraisingPageDonations().Donations.Where(x => x.DonationDate >= new DateTime(2015, 04, 1)).ToList();
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
            return votesResults.Where(x => x.Name != "Empty");
        }

        public string IsMovie(string vote)
        {
            var candidates = GetMovies();
            foreach (var candidate in candidates)
            {
                if (vote.ToLower().Contains(candidate.ToLower()))
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