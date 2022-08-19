using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Models;
using Azure.Search.Documents.Indexes.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Reflection;

namespace AzSearchLab.Models
{
    public class SearchIndexModel
    {
        // The default search pagination size
        private const int SearchPageSize = 10;

        // Get all indexes under the search service
        public static IEnumerable<string> ListIndexs(string searchName, string apiKey)
        { 
            List<string> indexs = new List<string>();

            var indexClient = new SearchIndexClient(new Uri("https://" + searchName+ ".search.windows.net"), new Azure.AzureKeyCredential(apiKey));
            var idx = indexClient.GetIndexNames();
            foreach (var item in idx)
            {
                indexs.Add(item); 
            }
              
            return indexs;
        }

        // Get all skillsets under the search service
        public static IEnumerable<string> GetSkillSet(string searchName, string apiKey)
        {
            string searchServiceUri = "https://" + searchName + ".search.windows.net";
            var indexerClient = new SearchIndexerClient(new Uri(searchServiceUri), new AzureKeyCredential(apiKey));

            List<string> apiSkills = new List<string>();
            var skillsets = indexerClient.GetSkillsets();
            foreach (var skillset in skillsets.Value)
            {
                foreach (SearchIndexerSkill skill in skillset.Skills)
                {
                    //Check the skill property to see whether it contains the ODataType. This property will contain the web api skill
                    var allProps = skill.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).OrderBy(pi => pi.Name).ToList();
                    foreach (PropertyInfo propertyInfo in allProps)
                    {
                        if (propertyInfo.Name.Equals("ODataType"))
                        {
                            string skillsetType = (string)propertyInfo.GetValue(skill);
                             
                            // Extract each web api skill's property, like headers, url. 
                            if ("#Microsoft.Skills.Custom.WebApiSkill" == skillsetType)
                            {
                                JObject skillJson = new JObject();
                                skillJson["@odata.type"] = "#Microsoft.Skills.Custom.WebApiSkill";

                                apiSkills.Add(skill.Name);
                                apiSkills.Add(skill.Description);
                                skillJson["name"] = skill.Name;
                                skillJson["description"] = skill.Description;

                                List<string> inputNames = new List<string>();
                                JArray inputArray = new JArray();
                                string input = "";
                                foreach (var item in skill.Inputs)
                                {
                                    input += item.Name + ":" + item.Source + ",";

                                    JObject inputJson = new JObject();
                                    inputJson["name"] = item.Name;
                                    inputJson["source"] = item.Source;
                                    inputArray.Add(inputJson);
                                    inputNames.Add(item.Name);
                                }
                                apiSkills.Add(input);
                                skillJson["inputs"] = inputArray;

                                JArray outArray = new JArray();
                                string output = "";
                                foreach (var item in skill.Outputs)
                                {
                                    output += item.Name + ":" + item.TargetName + ",";

                                    JObject outJson = new JObject();
                                    outJson["name"] = item.Name;
                                    outJson["targetName"] = item.TargetName;
                                    outArray.Add(outJson);
                                }
                                apiSkills.Add(output);
                                skillJson["outputs"] = outArray;

                                apiSkills.Add(skill.Context.ToString());
                                apiSkills.Add(((WebApiSkill)skill).Uri.ToString());
                                apiSkills.Add(((WebApiSkill)skill).Timeout.ToString());
                                apiSkills.Add(((WebApiSkill)skill).HttpMethod.ToString()); 
                                skillJson["context"] = ((WebApiSkill)skill).Context.ToString();
                                skillJson["uri"] = ((WebApiSkill)skill).Uri.ToString();
                                skillJson["timeout"] = ((WebApiSkill)skill).Timeout.ToString();
                                skillJson["httpMethod"] = ((WebApiSkill)skill).HttpMethod.ToString();

                                JObject headerJson = new JObject();
                                string headers = "";
                                foreach (var header in ((WebApiSkill)skill).HttpHeaders)
                                {
                                    headers += header.Key + ":" + header.Value + ",";
                                    headerJson[header.Key] = header.Value;
                                } 
                                apiSkills.Add(headers);
                                skillJson["httpHeaders"] = headerJson;

                                apiSkills.Add(((WebApiSkill)skill).DegreeOfParallelism.ToString());
                                apiSkills.Add(((WebApiSkill)skill).BatchSize.ToString());
                                skillJson["degreeOfParallelism"] = ((WebApiSkill)skill).DegreeOfParallelism.ToString();
                                skillJson["batchSize"] = ((WebApiSkill)skill).BatchSize.ToString();

                                apiSkills.Add(skillJson.ToString());

                                // Add 3 groups of input data
                                JObject valuesJson = new JObject();
                                JArray valuesArray = new JArray();
                                int inputDataNum = 3; 
                                for (int i = 0; i < inputDataNum; i++)
                                {
                                    JObject valueJson = new JObject();
                                    valueJson["recordId"] = i.ToString();
                                    JObject dataJson = new JObject();
                                    foreach (var nm in inputNames)
                                    {
                                        dataJson[nm] = "Test <"+ nm + "> input, please change it to real data!!!";
                                    }
                                    valueJson["data"] = dataJson;
                                    valuesArray.Add(valueJson);
                                }
                                valuesJson["values"] = valuesArray;
                                apiSkills.Add(valuesJson.ToString());
                            }
                        }
                    } 
                }
            }

            return apiSkills;
        }

        // Get the web api skill's URL, headers, method and body. Make the PUT/POST request. 
        public static string WebAPISkillURLQuery(string url, string method, string headers, string payload)
        {
            HttpClient webClient = new HttpClient();
            HttpMethod httpMethod = HttpMethod.Post;
            if (method == "PUT")
            {
                httpMethod = HttpMethod.Put;
            }

            var hds = headers.Split("\n");
            foreach (var item in hds)
            {
                if (item != null && item.Length > 0)
                {
                    var keyval = item.Split(':');
                    webClient.DefaultRequestHeaders.Add(keyval[0], keyval[1]);
                } 
            }

            HttpRequestMessage request = new HttpRequestMessage(httpMethod, url);
            request.Content = new StringContent(payload, Encoding.UTF8, "application/json");
             
            var result = webClient.SendAsync(request);
            var response = result.Result.Content.ReadAsStringAsync();

            return response.Result; 
        }
         
        // Get the index Json definition by index name.
        public static string GetIndexDefinition(string searchName, string apiKey, string indexName)
        {  
            string searchURL = "https://" + searchName + ".search.windows.net/indexes/"+ indexName+ "?api-version=2020-06-30";
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, searchURL);
            request.Content = new StringContent("", Encoding.UTF8, "application/json");

            HttpClient webClient = new HttpClient();
            webClient.DefaultRequestHeaders.Add("api-key", apiKey); 

            var result = webClient.SendAsync(request);
            var response = result.Result.Content.ReadAsStringAsync();
              
            JToken parsedJson = JToken.Parse(response.Result);
            var beautified = parsedJson.ToString(Formatting.Indented); 

            return beautified;
            //return response.Result;
        }
         
        // Update index definition by allow index down time.
        public static string UpdateIndexDefinition(string searchName, string apiKey, string indexName, string indexDefinition)
        {  
            HttpClient webClient = new HttpClient();
            string searchURL = "https://" + searchName + ".search.windows.net/indexes/" + indexName + "?api-version=2020-06-30&allowIndexDowntime=true";
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, searchURL); 

            request.Content = new StringContent(indexDefinition, Encoding.UTF8, "application/json");
            webClient.DefaultRequestHeaders.Add("api-key", apiKey);

            var result = webClient.SendAsync(request);
            var response = result.Result.Content.ReadAsStringAsync();
            return response.Result;
             
        }
         
        // Get the index fields properties. The properties will be saved to the in/out List<string> parameters. 
        public static void GetIndexFieldsProperty(string searchName, string apiKey, string indexName, 
                                                                 ref List<string> searchableFieldlist, ref List<string> facetableFieldlist, 
                                                                 ref List<string> retrievableFieldlist, ref List<string> sortableFieldlist,
                                                                 ref List<string> scoringProfileList, ref List<string> geographyPointList)
        { 
            var indexClient = new SearchIndexClient(new Uri("https://" + searchName + ".search.windows.net"), new Azure.AzureKeyCredential(apiKey));
            var idx = indexClient.GetIndex(indexName).Value;

            scoringProfileList = new List<string>();
            foreach (var profile in idx.ScoringProfiles)
            {
                scoringProfileList.Add(profile.Name);
            }

            searchableFieldlist = new List<string>(); //Search Fields , High light
            facetableFieldlist = new List<string>(); // Facet
            retrievableFieldlist = new List<string>(); //Select Fields
            sortableFieldlist = new List<string>(); //Order by 
            geographyPointList = new List<string>(); //geography Point 
            foreach (var field in idx.Fields)
            {
                if (field.IsSearchable == true)
                {
                    searchableFieldlist.Add(field.Name);
                }
                 
                if (field.IsFacetable == true)
                {
                    facetableFieldlist.Add(field.Name);
                }
                 
                if (field.IsHidden == false)
                {
                    retrievableFieldlist.Add(field.Name);
                }

                if (field.IsSortable == true)
                {
                    sortableFieldlist.Add(field.Name);
                }

                if (field.Type == SearchFieldDataType.GeographyPoint)
                {
                    geographyPointList.Add(field.Name);
                }
            }
   
            return;
        }

        // List the index field synonyms. 
        public static IEnumerable<string> ListIndexsFieldsSynonyms(string searchName, string apiKey, string synonymName)
        {
            List<string> indexfields = new List<string>();

            var indexClient = new SearchIndexClient(new Uri("https://" + searchName + ".search.windows.net"), new Azure.AzureKeyCredential(apiKey));
            var idxNames = indexClient.GetIndexNames();

            Azure.Response<SearchIndex> idxRsp;
            SearchIndex idx;
            foreach (var item in idxNames)
            {
                indexfields.Add(item);

                idxRsp = indexClient.GetIndex(item);
                idx = idxRsp.Value;
                List<string> fieldlist = new List<string>();
                foreach (var field in idx.Fields)
                {
                    if (field.IsSearchable == true)
                    {
                        if (field.SynonymMapNames.Contains(synonymName))
                        {
                            fieldlist.Add(field.Name + ":" + synonymName);
                        }
                        else
                        {
                            fieldlist.Add(field.Name);
                        }
                    }
                }
                var fieldliststr = string.Join(",", fieldlist.ToArray());
                indexfields.Add(fieldliststr);

            }
               
            return indexfields;
        }

        public static string GetSynonymMaps(string searchName, string apiKey, ref string synmapName)
        { 
            var indexClient = new SearchIndexClient(new Uri("https://" + searchName + ".search.windows.net"), new Azure.AzureKeyCredential(apiKey));
            var rp = indexClient.GetSynonymMaps();
            var synonyMaps = rp.Value;
            string synonyMapsStr = "";
            bool isFirst = false;
            foreach (var alz in synonyMaps)
            {
                if (!isFirst)
                {
                    synmapName = alz.Name;
                    isFirst = true;
                }
                synonyMapsStr += alz.Name + ","; 
            }
            if (synonyMapsStr != "")
            {
                synonyMapsStr = synonyMapsStr.Remove(synonyMapsStr.Length - 1, 1);
            }
            return synonyMapsStr;
        }

        public static string GetSynonymMapRule(string searchName, string apiKey, string synonyMapName)
        {
            var indexClient = new SearchIndexClient(new Uri("https://" + searchName + ".search.windows.net"), new Azure.AzureKeyCredential(apiKey));
            var rp = indexClient.GetSynonymMaps();
            var synonyMaps = rp.Value;

            foreach (var alz in synonyMaps)
            {
                if (alz.Name == synonyMapName)
                {
                    return alz.Synonyms;
                } 
            } 
            return null;
        }

        public static int DeleteSynonymMap(string searchName, string apiKey, string loadedindex, string synonymMapName)
        {
            var indexClient = new SearchIndexClient(new Uri("https://" + searchName + ".search.windows.net"), new Azure.AzureKeyCredential(apiKey));
            var rp = indexClient.DeleteSynonymMap(synonymMapName); 

            // remove this synonymMapName from all index fields 
            var allindexes = indexClient.GetIndexes();
            foreach (var idx in allindexes)
            {
                foreach (var field in idx.Fields)
                {
                    field.SynonymMapNames.Remove(synonymMapName);
                }
                indexClient.CreateOrUpdateIndex(idx);
            }
             
            return rp.Status;
        }

        public static int CreateOrUpdateSynonymMap(string searchName, string apiKey, string indexFields, string synonymMapName, string synonymRule)
        {
            var indexClient = new SearchIndexClient(new Uri("https://" + searchName + ".search.windows.net"), new Azure.AzureKeyCredential(apiKey));
            var synMap = new SynonymMap(synonymMapName, synonymRule);
            var rp = indexClient.CreateOrUpdateSynonymMap(synMap); 

            // remove this synonymMapName from all index fields 
            var allindexes = indexClient.GetIndexes();
            foreach (var idx in allindexes)
            {
                foreach (var field in idx.Fields)
                { 
                    field.SynonymMapNames.Remove(synonymMapName); 
                }
                indexClient.CreateOrUpdateIndex(idx);
            } 

            //add the SynonymMapNames to fields
            if (indexFields != null && indexFields.Contains(","))
            {
                var indexAndFields = indexFields.Split(";");
                Dictionary<string, SearchIndex> indexObjs = new Dictionary<string, SearchIndex>();
                foreach (var item in indexAndFields)
                {
                    if (item.Contains(","))
                    {
                        var oneIndexField = item.Split(",");
                        if (!indexObjs.ContainsKey(oneIndexField[1]))
                        {
                            var currentIndex = indexClient.GetIndex(oneIndexField[1]).Value;
                            indexObjs[oneIndexField[1]] = currentIndex;
                            // Only a single synonym map configuration is currently supported.
                            currentIndex.Fields.First(f => f.Name == oneIndexField[0]).SynonymMapNames.Clear();
                            currentIndex.Fields.First(f => f.Name == oneIndexField[0]).SynonymMapNames.Add(synonymMapName);
                            indexClient.CreateOrUpdateIndex(currentIndex);
                        }
                        else
                        {
                            // Only a single synonym map configuration is currently supported.
                            indexObjs[oneIndexField[1]].Fields.First(f => f.Name == oneIndexField[0]).SynonymMapNames.Clear();
                            indexObjs[oneIndexField[1]].Fields.First(f => f.Name == oneIndexField[0]).SynonymMapNames.Add(synonymMapName);
                            indexClient.CreateOrUpdateIndex(indexObjs[oneIndexField[1]]);
                        }
                    }
                }
            }

            return rp.GetRawResponse().Status;
        }

        public static string GetIndexAnalyzers(string searchName, string apiKey, string indexName)
        {
            var indexClient = new SearchIndexClient(new Uri("https://" + searchName + ".search.windows.net"), new Azure.AzureKeyCredential(apiKey));

            //Get index definition
            var indexdefResult = indexClient.GetIndex(indexName);
            var indexdef = indexdefResult.Value;
            string analyzers = "";
            foreach (var alz in indexdef.Analyzers)
            {
                analyzers += alz.Name + ",";
            }
            if (analyzers != "")
            {
                analyzers = analyzers.Remove(analyzers.Length - 1, 1);
            }

            return analyzers;
        }
         
        public static string AnalyzeTextIntoTokenWithTokenizer(string searchName, string apiKey, string selectedIndex, 
                                                               string tokenizerName, string tokenizerFilterName, string charFilterName, string txtToBreak)
        {
            var indexClient = new SearchIndexClient(new Uri("https://" + searchName + ".search.windows.net"), new Azure.AzureKeyCredential(apiKey));
            var searchClient = indexClient.GetSearchClient(selectedIndex); 
            AnalyzeTextOptions opt = new AnalyzeTextOptions(txtToBreak, new LexicalTokenizerName(tokenizerName));
            if (tokenizerFilterName != null && tokenizerFilterName.Length > 0)
            {
                opt.TokenFilters.Add(tokenizerFilterName);
            }
            if (charFilterName != null && charFilterName.Length > 0)
            {
                opt.CharFilters.Add(charFilterName); 
            }

            var analyzeResponse = indexClient.AnalyzeText(selectedIndex, opt);

            JArray jarrayObj = new JArray();
            foreach (var item in analyzeResponse.Value)
            {
                JObject sugJson = new JObject();
                sugJson["Token"] = item.Token;
                sugJson["Position"] = item.Position;
                sugJson["StartOffset"] = item.StartOffset;
                sugJson["EndOffset"] = item.EndOffset;
                jarrayObj.Add(sugJson);
            }

            JObject sugJsonResult = new JObject();
            sugJsonResult["tokens"] = jarrayObj;
            return sugJsonResult.ToString();
        }

        public static string AnalyzeTextIntoToken(string searchName, string apiKey, string selectedIndex, string analyzerName, string txtToBreak)
        {
            var indexClient = new SearchIndexClient(new Uri("https://" + searchName + ".search.windows.net"), new Azure.AzureKeyCredential(apiKey));
            var searchClient = indexClient.GetSearchClient(selectedIndex);
            AnalyzeTextOptions opt = new AnalyzeTextOptions(txtToBreak, new LexicalAnalyzerName(analyzerName));

            var analyzeResponse = indexClient.AnalyzeText(selectedIndex, opt);
             
            JArray jarrayObj = new JArray();
            foreach (var item in analyzeResponse.Value)
            {
                JObject sugJson = new JObject();
                sugJson["Token"] = item.Token;
                sugJson["Position"] = item.Position;
                sugJson["StartOffset"] = item.StartOffset;
                sugJson["EndOffset"] = item.EndOffset; 
                jarrayObj.Add(sugJson);
            } 

            JObject sugJsonResult = new JObject();
            sugJsonResult["tokens"] = jarrayObj;
            return sugJsonResult.ToString();
        }
 
        public static string QuerySuggestionsInIndex(string searchName, string apiKey, string selectedIndex, string sg, string suggestText)
        {
            var indexClient = new SearchIndexClient(new Uri("https://" + searchName + ".search.windows.net"), new Azure.AzureKeyCredential(apiKey));
            var searchClient = indexClient.GetSearchClient(selectedIndex);
            var searchResponse = searchClient.Suggest<SearchDocument>(suggestText, sg);
            var docs = searchResponse.Value.Results;
              
            JArray jarrayObj = new JArray(); 
            foreach (var item in docs)
            {
                JObject sugJson = new JObject();
                sugJson["searchText"] = item.Text;
                if (item.Document.Keys.Contains("listingId"))
                {
                    sugJson["listingId"] = item.Document.GetString("listingId");
                } 
                else if (item.Document.Keys.Contains("fieldkey"))
                {
                    sugJson["fieldkey"] = item.Document.GetString("fieldkey");
                }
                jarrayObj.Add(sugJson);
            }

            JObject sugJsonResult = new JObject();
            sugJsonResult["value"] = jarrayObj;
            return sugJsonResult.ToString();
        }
         
        public static string QueryAutocompleteInIndex(string searchName, string apiKey, string selectedIndex, string sg, string term, string autocompleteInput)
        {
            var indexClient = new SearchIndexClient(new Uri("https://" + searchName + ".search.windows.net"), new Azure.AzureKeyCredential(apiKey));
            var searchClient = indexClient.GetSearchClient(selectedIndex);
            AutocompleteOptions opt = new AutocompleteOptions();
            if (term == "oneTerm")
            {
                opt.Mode = AutocompleteMode.OneTerm;
            } 
            else if (term == "twoTerms")
            {
                opt.Mode = AutocompleteMode.TwoTerms;
            }
            else if (term == "oneTermWithContext")
            {
                opt.Mode = AutocompleteMode.OneTermWithContext;
            }
            var searchResponse = searchClient.Autocomplete(autocompleteInput, sg, opt); 
            var docs = searchResponse.Value.Results;
              
            JArray jarrayObj = new JArray();
            foreach (var item in docs)
            {
                JObject sugJson = new JObject();
                sugJson["text"] = item.Text;
                sugJson["queryPlusText"] = item.QueryPlusText;
                jarrayObj.Add(sugJson);
            }

            JObject sugJsonResult = new JObject();
            sugJsonResult["value"] = jarrayObj;
            return sugJsonResult.ToString();
        }

        public static string GetIndexDocument(string searchName, string apiKey, string indexName, int pageNum = 0)
        { 
            var indexClient = new SearchIndexClient(new Uri("https://" + searchName + ".search.windows.net"), new Azure.AzureKeyCredential(apiKey));
            var searchClient = indexClient.GetSearchClient(indexName);
            long docCount  = searchClient.GetDocumentCount();

            SearchOptions opt = new SearchOptions();
            opt.Size = SearchPageSize;
            opt.Skip = SearchPageSize * pageNum; //page number 
            var allDoc = searchClient.Search<SearchDocument>("*", opt);
            var docs = allDoc.Value.GetResults();

            string result = "";
            foreach (var item in docs)
            {
                result += item.Document.ToString() + ",";
            }
            if (result != null && result != "")
            {
                result = result.Remove(result.Length - 1, 1);
            }
              
            return result;
        }

        public static string GetIndexSuggesters(string searchName, string apiKey, string indexName)
        {
            var indexClient = new SearchIndexClient(new Uri("https://" + searchName + ".search.windows.net"), new Azure.AzureKeyCredential(apiKey)); 

            //Get index definition
            var indexdefResult = indexClient.GetIndex(indexName);
            var indexdef = indexdefResult.Value;
            string suggesters = "";
            foreach (var sug in indexdef.Suggesters)
            {
                suggesters += sug.Name + ",";
            }
            if (suggesters != "")
            {
                suggesters = suggesters.Remove(suggesters.Length - 1, 1);
            }

            return suggesters;
        }

        public static string QueryIndexDocumentWithQueryComposer(string searchName, string apiKey, string indexName, string parameters, string httpMethod, string apiVersion)
        {  
            HttpClient webClient = new HttpClient();

            if ("GET" == httpMethod)
            {
                string searchUpdateDocumentURL = "https://" + searchName + ".search.windows.net/indexes/" + indexName + "/docs?" + parameters /*+ "&api-version=2020-06-30"*/;
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, searchUpdateDocumentURL);

                webClient.DefaultRequestHeaders.Add("api-key", apiKey);

                var result = webClient.SendAsync(request);
                var response = result.Result.Content.ReadAsStringAsync();

                JToken parsedJson = JToken.Parse(response.Result);
                var beautified = parsedJson.ToString(Formatting.Indented);

                return beautified;
            }
            else if ("POST" == httpMethod)
            {
                string searchUpdateDocumentURL = "https://" + searchName + ".search.windows.net/indexes/" + indexName + "/docs/search?api-version=" + apiVersion /*+ "&api-version=2020-06-30"*/;
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, searchUpdateDocumentURL);
                 
                request.Content = new StringContent(parameters, Encoding.UTF8, "application/json");
                webClient.DefaultRequestHeaders.Add("api-key", apiKey);

                var result = webClient.SendAsync(request);
                var response = result.Result.Content.ReadAsStringAsync();

                JToken parsedJson = JToken.Parse(response.Result);
                var beautified = parsedJson.ToString(Formatting.Indented);

                return beautified;
            } 
            else
            {
                return "";
            } 
        }

        public static string QueryIndexDocument(string searchName, string apiKey, string indexName, string keyword, int pageNum=0)
        {
            var indexClient = new SearchIndexClient(new Uri("https://" + searchName + ".search.windows.net"), new Azure.AzureKeyCredential(apiKey));
            var searchClient = indexClient.GetSearchClient(indexName);
            long docCount = searchClient.GetDocumentCount();

            SearchOptions opt = new SearchOptions();
            opt.Size = SearchPageSize;
            opt.Skip = SearchPageSize * pageNum; //page number 
            //opt.QueryType = SearchQueryType.Full;
            opt.SearchMode = SearchMode.All;
            var allDoc = searchClient.Search<SearchDocument>(keyword, opt);
            var docs = allDoc.Value.GetResults();

            string result = "";
            foreach (var item in docs)
            {
                result += item.Document.ToString() + ",";
            }
            if (result != "")
            {
                result = result.Remove(result.Length - 1, 1);
            }

            return result;
        }

        public static string UpdateIndexDocument(string searchName, string apiKey, string indexName, string indexDocument)
        { 
            string jsonDocument = "[" + indexDocument + "]";
            string payloadJson = "{\"value\": [";
            dynamic jsArray = JsonConvert.DeserializeObject(jsonDocument);
            foreach (var obj in jsArray)
            { 
                string item = Convert.ToString(obj);  
                item = item.Insert(1, "\"@search.action\": \"mergeOrUpload\",");
                payloadJson += item.ToString() + ",";
                  
            }
            payloadJson = payloadJson.Remove(payloadJson.Length - 1, 1); 
            payloadJson += "]}"; 
              
            HttpClient webClient = new HttpClient();
            string searchUpdateDocumentURL = "https://" + searchName + ".search.windows.net/indexes/" + indexName + "/docs/index?api-version=2020-06-30"; 
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, searchUpdateDocumentURL);
 
            request.Content = new StringContent(payloadJson, Encoding.UTF8, "application/json"); 
            webClient.DefaultRequestHeaders.Add("api-key", apiKey); 

            var result = webClient.SendAsync(request);
            var response = result.Result.Content.ReadAsStringAsync(); 
            return response.Result; 
        }
         
    }
}
