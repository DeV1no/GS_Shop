using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace GS_Shop_UserManagement.Infrastructure.Policy
{
    public class AuthorizationPolicyLoader
    {
        public static Dictionary<string, List<string>> LoadPolicies(IConfiguration configuration)
        {
            var policyConfig = configuration.GetSection("AuthorizationPolicies").Get<List<PolicyConfiguration>>();

            var policies = new Dictionary<string, List<string>>();

            if (policyConfig != null)
            {
                foreach (var policy in policyConfig)
                {
                    policies[policy.PolicyName] = policy.RequiredClaims;
                }
            }

            return policies;
        }
    }

    public class PolicyConfiguration
    {
        public string PolicyName { get; set; }
        public List<string> RequiredClaims { get; set; }
    }
}