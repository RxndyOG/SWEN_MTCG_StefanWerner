

using Newtonsoft.Json;

namespace TCPserverClasses
{
    public class Body
    {
        public Body()
        {

        }
        public (string, string) getBodyHeader(string request, StreamReader reader)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            while (!string.IsNullOrWhiteSpace(request = reader.ReadLine()))
            {
                string[] headerParts = request.Split(": ", 2, StringSplitOptions.None);
                if (headerParts.Length == 2)
                {
                    headers[headerParts[0]] = headerParts[1];
                }
            }

            string body = null;
            if (headers.ContainsKey("Content-Length"))
            {
                int contentLength = int.Parse(headers["Content-Length"]);
                char[] buffer = new char[contentLength];
                reader.Read(buffer, 0, contentLength);
                body = new string(buffer);
            }

            if (headers.ContainsKey("Authorization"))
            {
                return (body, headers["Authorization"]);
            }

            return (body, null);
        }

        public List<Dictionary<string, object>> getDeserialBody(string body)
        {

            List<Dictionary<string, object>> deserialBody = null;
            if (body != null)
            {
                if (body.TrimStart().StartsWith("["))
                {
                    if (body.TrimStart().StartsWith("[{"))
                    {
                        // parsed beginnt mit [{
                        deserialBody = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(body);
                    }
                    else
                    {
                        //parsed begint mit [
                        var deserializedArray = JsonConvert.DeserializeObject<List<object>>(body);
                        deserialBody = new List<Dictionary<string, object>>();

                        foreach (var item in deserializedArray)
                        {
                            deserialBody.Add(new Dictionary<string, object> { { "Value", item } });
                        }
                    }
                    
                }
                else
                {
                    // parsed beginnt mit {
                    if (body.TrimStart().StartsWith("{"))
                    {
                        deserialBody = new List<Dictionary<string, object>> { JsonConvert.DeserializeObject<Dictionary<string, object>>(body) };
                    }
                    else if (body.StartsWith("\"") && body.EndsWith("\""))
                    {
                        // parsed beginnt mit \"
                        string simpleString = JsonConvert.DeserializeObject<string>(body);
                        deserialBody = new List<Dictionary<string, object>>{ new Dictionary<string, object> { { "ID", simpleString } }};
                    }
                }
            }

            return deserialBody;
        }
    }

    
}
