public class Script : ScriptBase
{
    public override async Task<HttpResponseMessage> ExecuteAsync()
    {
        var accessTokenJson = await this.getAccessToken().ConfigureAwait(false);
        //this.Context.Request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessTokenJson);
        
        var finalRequest = new HttpRequestMessage(HttpMethod.Get, this.Context.Request.RequestUri.ToString());
        finalRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessTokenJson);
        var finalResponse = await this.Context.SendAsync(finalRequest, this.CancellationToken).ConfigureAwait(false);
        
        return finalResponse;
    }

    private async Task<string> getAccessToken()
    {
        string content = string.Empty;


        IEnumerable<string> clientIDValues = this.Context.Request.Headers.GetValues("clientId");
        var clientIdVal = clientIDValues.FirstOrDefault();

        IEnumerable<string> clientSecretValues = this.Context.Request.Headers.GetValues("clientSecret");
        var clientSecretVal = clientSecretValues.FirstOrDefault();

        IEnumerable<string> orgIdValues = this.Context.Request.Headers.GetValues("orgId");
        var orgIdVal = orgIdValues.FirstOrDefault();

        IEnumerable<string> imsValues = this.Context.Request.Headers.GetValues("ims");
        var imsVal = imsValues.FirstOrDefault();

        IEnumerable<string> technicalAccountIdValues = this.Context.Request.Headers.GetValues("technicalAccountId");
        var technicalAccountIdVal = technicalAccountIdValues.FirstOrDefault();

        IEnumerable<string> metaScopesValues = this.Context.Request.Headers.GetValues("metaScopes");
        var metaScopesVal = metaScopesValues.FirstOrDefault();

        IEnumerable<string> privateKeyValues = this.Context.Request.Headers.GetValues("privateKey");
        var privateKeyVal = privateKeyValues.FirstOrDefault();

       // var requestContent = JsonConvert.SerializeObject(new { clientId = clientIdVal, clientSecret = clientSecretVal, orgId = orgIdVal, ims = imsVal, technicalAccountId = technicalAccountIdVal, metaScopes = metaScopesVal, privateKey = privateKeyVal });

        JObject requestContent = new JObject
        {
            ["clientId"] = clientIdVal,
            ["clientSecret"] = clientSecretVal,
            ["orgId"] = orgIdVal,
            ["ims"] = imsVal,
            ["technicalAccountId"] = technicalAccountIdVal,
            ["metaScopes"] = metaScopesVal,
            ["privateKey"] = privateKeyVal
        };

        var accessTokenRequest = new HttpRequestMessage(HttpMethod.Post, "https://adobeioruntime.net/api/v1/web/283250-powerautomate/pa/jwt");

        accessTokenRequest.Content = CreateJsonContent("{\"clientId\": \"" + clientIdVal + "\", \"clientSecret\": \"" + clientSecretVal + "\", \"orgId\": \"" + orgIdVal + "\", \"ims\": \"" + imsVal + "\", \"technicalAccountId\": \"" + technicalAccountIdVal + "\", \"metaScopes\": \"" + metaScopesVal + "\", \"privateKey\": \"" + privateKeyVal + "\"}");
        accessTokenRequest.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        try
        {
            var accessTokenResponse = await this.Context.SendAsync(accessTokenRequest, this.CancellationToken).ConfigureAwait(false);
            content = await accessTokenResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (accessTokenResponse.IsSuccessStatusCode)
            {
                var jsonContent = JObject.Parse(content);
                return jsonContent["access_token"].ToString();
            }
            else
            {
                return accessTokenResponse.StatusCode.ToString();
            }
        }
        catch (HttpRequestException ex)
        {
            return ex.Message ;
        }
        catch (JsonReaderException ex)
        {
            return ex.Message;
        }
    }
}
