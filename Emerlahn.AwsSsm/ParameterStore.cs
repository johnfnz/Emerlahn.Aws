using Amazon;
using Amazon.SimpleSystemsManagement.Model;
using System.Linq;
using System.Threading.Tasks;
using SsmClient = Amazon.SimpleSystemsManagement.AmazonSimpleSystemsManagementClient;

namespace Emerlahn.AwsSsm
{
    public static class ParameterStore
    {
        public static async Task<T> GetParameters<T>(string awsRegion, string prefix = "") where T : new()
        {
            var properties = typeof(T).GetProperties().ToDictionary(p => prefix + p.Name);

            using (var client = new SsmClient(RegionEndpoint.GetBySystemName(awsRegion)))
            {
                var request = new GetParametersRequest
                {
                    Names = properties.Keys.ToList(),
                    WithDecryption = true,
                };
                var response = await client.GetParametersAsync(request).ConfigureAwait(false);
                var result = new T();
                foreach (var responseParameter in response.Parameters)
                {
                    properties[responseParameter.Name].SetValue(result, responseParameter.Value);
                }
                // TODO: Handle response.InvalidParameters
                return result;
            }
        }
    }
}
