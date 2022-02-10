using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Api.Utils.Log;
using wsIntellinx.ViewModels;
using wsIntellinx.ViewModels.Params;

namespace wsIntellinx.BLL
{
    /// <seealso cref="IIntellinxLogic" />
    public class IntellinxLogic : IIntellinxLogic
    {
        private readonly ILogger _log;

        /// <summary>
        /// Constructor for IntellinxLogic
        /// </summary>
        /// <param name="log"></param>
        public IntellinxLogic(ILogger log)
        {
            _log = log;
        }

        /// <seealso cref="IIntellinxLogic.GetKbaMembers(DateTime, DateTime?)" />
        public async Task<List<KbaMember>> GetKbaMembers(DateTime startDate, DateTime? endDate)
        {
            endDate ??= DateTime.Now;
            var kbaMembers = new List<KbaMember>();
            kbaMembers.Add(new KbaMember
            {
                BarId = "1",
                MemberTypeCode = "COMMUNITY_ADMIN",
                MemberStatus = "Active",
                Disbarred = "No",
                FirstName = "FakeFirstName_1",
                MiddleName = "FakeMiddleName_1",
                LastName = "FakeLastName_1",
                MemberNameSuffix = "FakeMemberNameSuffix_1",
                EmailAddress = "FakeEmailAddress_1",
                HomePhone = "FakeHomePhone_1",
                Mobile = "FakeMobile_1",
                AddressLine1 = "111 vandalay",
                AddressLine2 = "1 Drive",
                PostalCode = "40601",
                Country = "USA",
                Location = "FakeLocation_1",
                City = "Frankfort",
                State = "KY"
            });
            kbaMembers.Add(new KbaMember
            {
                BarId = "2",
                MemberTypeCode = "limitedprac",
                MemberStatus = "Active",
                Disbarred = "Yes",
                FirstName = "FakeFirstName_2",
                MiddleName = "FakeMiddleName_2",
                LastName = "FakeLastName_2",
                MemberNameSuffix = "FakeMemberNameSuffix_2",
                EmailAddress = "FakeEmailAddress_2",
                HomePhone = "FakeHomePhone_2",
                Mobile = "FakeMobile_2",
                AddressLine1 = "222 vandalay",
                AddressLine2 = "2 Drive",
                PostalCode = "40601",
                Country = "USA",
                Location = "FakeLocation_2",
                City = "Frankfort",
                State = "KY"
            });
            return await Task.FromResult(kbaMembers);
        }

        /// <seealso cref="IIntellinxLogic.GetKbaMember(KbaMemberParam)"/>
        public async Task<KbaMember> GetKbaMember(KbaMemberParam kbaMemberParam)
        {
            if (kbaMemberParam.BarId == "KBANumber123")
            {
                var kbaMember = new KbaMember
                {
                    BarId = "KBANumber123",
                    MemberTypeCode = "limitedpracla1",
                    MemberStatus = "Limited Practice",
                    Disbarred = "No",
                    FirstName = "John",
                    MiddleName = "FakeMiddleName_123",
                    LastName = "Doe",
                    MemberNameSuffix = "FakeMemberNameSuffix_123",
                    EmailAddress = "JohnDoe@Hometown.net",
                    HomePhone = "FakeHomePhone_123",
                    Mobile = "FakeMobile_123",
                    AddressLine1 = "222 vandalay",
                    AddressLine2 = "2 Drive",
                    PostalCode = "40601",
                    Country = "USA",
                    Location = "FakeLocation_123",
                    City = "Frankfort",
                    State = "KY"
                };
                return await Task.FromResult(kbaMember);
            }
            return null;
        }
    }
}
