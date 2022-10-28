using AzSearchLab.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq; 

namespace AzSearchLab.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public string QueryComposerButtonClick(string searchName, string apiKey, string loadedindex, string queryParameters, string httpMethod, string apiVersion)
        {
            JObject indexJson = new JObject();

            var queryRsp = SearchIndexModel.QueryIndexDocumentWithQueryComposer(searchName, apiKey, loadedindex, queryParameters, httpMethod, apiVersion); 
            indexJson["result"] = queryRsp;

            return indexJson.ToString();
        }


        [HttpPost]
        public string WebAPISkillURLQueryButtonClick(string url, string method, string headers, string payload)
        {
            try
            { 
                JObject indexJson = new JObject();

                var queryRsp = SearchIndexModel.WebAPISkillURLQuery(url, method, headers, payload);
                indexJson["result"] = queryRsp;

                return indexJson.ToString();
            } 
            catch (Exception e) 
            {
                JObject indexJson = new JObject();
                indexJson["result"] = e.Message; 
                return indexJson.ToString();
            }

        }

        [HttpPost]
        public string LoadIndexDataButtonClick(string searchName, string apiKey, string selectedIndex, int pageNum)
        {
            try
            {
                JObject indexJson = new JObject();
                var doc = SearchIndexModel.GetIndexDocument(searchName, apiKey, selectedIndex, pageNum);
                var sug = SearchIndexModel.GetIndexSuggesters(searchName, apiKey, selectedIndex);
                var alz = SearchIndexModel.GetIndexAnalyzers(searchName, apiKey, selectedIndex);
                string synmapName = "";
                var synmaps = SearchIndexModel.GetSynonymMaps(searchName, apiKey, ref synmapName);
                var synmapRule = SearchIndexModel.GetSynonymMapRule(searchName, apiKey, synmaps);
                var idxJs = SearchIndexModel.GetIndexDefinition(searchName, apiKey, selectedIndex);
                indexJson["IndexDocument"] = doc;
                indexJson["Suggesters"] = sug;
                indexJson["Analyzers"] = alz;
                indexJson["SynonymMaps"] = synmaps;
                indexJson["IndexJsonDefinition"] = idxJs;

                List<string> searchableFieldlist = null;
                List<string> facetableFieldlist = null;
                List<string> retrievableFieldlist = null;
                List<string> sortableFieldlist = null;
                List<string> scoringProfileList = null;
                List<string> geographyPointList = null;
                SearchIndexModel.GetIndexFieldsProperty(searchName, apiKey, selectedIndex, ref searchableFieldlist, ref facetableFieldlist,
                                                        ref retrievableFieldlist, ref sortableFieldlist, ref scoringProfileList, ref geographyPointList);
                 
                if (searchableFieldlist != null)
                {
                    JArray searchableFieldArray = new JArray();
                    foreach (string item in searchableFieldlist)
                    {
                        searchableFieldArray.Add(item);
                    }
                    indexJson["SearchableFieldlist"] = searchableFieldArray;
                }

                if (facetableFieldlist != null)
                {
                    JArray facetableFieldArray = new JArray();
                    foreach (string item in facetableFieldlist)
                    {
                        facetableFieldArray.Add(item);
                    }
                    indexJson["FacetableFieldlist"] = facetableFieldArray;
                }

                if (retrievableFieldlist != null)
                {
                    JArray retrievableFieldArray = new JArray();
                    foreach (string item in retrievableFieldlist)
                    {
                        retrievableFieldArray.Add(item);
                    }
                    indexJson["RetrievableFieldlist"] = retrievableFieldArray;
                }

                if (sortableFieldlist != null)
                {
                    JArray SortableFieldArray = new JArray();
                    foreach (string item in sortableFieldlist)
                    {
                        SortableFieldArray.Add(item);
                    }
                    indexJson["SortableFieldlist"] = SortableFieldArray;
                }

                if (scoringProfileList != null)
                {
                    JArray scoringProfileArray = new JArray();
                    foreach (string item in scoringProfileList)
                    {
                        scoringProfileArray.Add(item);
                    }
                    indexJson["ScoringProfileList"] = scoringProfileArray;
                }

                if (geographyPointList != null)
                {
                    JArray geographyPointArray = new JArray();
                    foreach (string item in geographyPointList)
                    {
                        geographyPointArray.Add(item);
                    }
                    indexJson["GeographyPointList"] = geographyPointArray;
                }

                return indexJson.ToString(); 
            }
            catch (Exception e)
            {
                return null;
            }
        }


        [HttpPost]
        public string AnalyzeTextIntoToken(string searchName, string apiKey, string loadedindex, string analyzerName, string txtToBreak)
        {
            try
            {
                var doc = SearchIndexModel.AnalyzeTextIntoToken(searchName, apiKey, loadedindex, analyzerName, txtToBreak);
                return doc;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        [HttpPost]
        public string AnalyzeTextIntoTokenWithTokenizer(string searchName, string apiKey, string loadedindex, 
                                                        string tokenizerName, string tokenizerFilter, string charFilter, string txtToBreak)
        {
            try
            {
                var doc = SearchIndexModel.AnalyzeTextIntoTokenWithTokenizer(searchName, apiKey, loadedindex, tokenizerName, tokenizerFilter, charFilter, txtToBreak);
                return doc;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        [HttpPost]
        public string QueryIndexDataButtonClick(string searchName, string apiKey, string loadedindex, string keyword, int pageNum)
        {
            try
            {
                var doc = SearchIndexModel.QueryIndexDocument(searchName, apiKey, loadedindex, keyword, pageNum);
                return doc;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        [HttpPost]
        public string LoadSuggestionData(string searchName, string apiKey, string loadedindex, string sg, string suggestText)
        {
            try
            {
                var result = SearchIndexModel.QuerySuggestionsInIndex(searchName, apiKey, loadedindex, sg, suggestText);
                return result;
            }
            catch (Exception e)
            {
                return null;
            }
        }
         
        [HttpPost]
        public string GetSynonymMapRule(string searchName, string apiKey, string synonyMapName)
        {
            try
            {
                JObject indexJson = new JObject();
                var synmapRule = SearchIndexModel.GetSynonymMapRule(searchName, apiKey, synonyMapName);
                indexJson["SynmapRule"] = synmapRule;

                var indexfiekds = SearchIndexModel.ListIndexsFieldsSynonyms(searchName, apiKey, synonyMapName);
                if (indexfiekds != null && indexfiekds.Count<string>() > 0)
                {
                    JArray hubsArray = new JArray();
                    foreach (string item in indexfiekds)
                    {
                        hubsArray.Add(item);
                    }
                    indexJson["IndexFields"] = hubsArray;  
                }
                 
                return indexJson.ToString();  
            }
            catch (Exception e)
            {
                return null;
            }
        }


        [HttpPost]
        public string DeleteSynonymMapButtonClick(string searchName, string apiKey, string loadedindex, string synonymMapName)
        {
            try
            {
                JObject indexJson = new JObject();

                int respcode = SearchIndexModel.DeleteSynonymMap(searchName, apiKey, loadedindex, synonymMapName);

                string synmapName = "";
                var synmaps = SearchIndexModel.GetSynonymMaps(searchName, apiKey, ref synmapName);
                var synmapRule = SearchIndexModel.GetSynonymMapRule(searchName, apiKey, synmapName);
                var indexfiekds = SearchIndexModel.ListIndexsFieldsSynonyms(searchName, apiKey, synmapName);
                indexJson["SynonymMaps"] = synmaps;
                indexJson["SynmapRule"] = synmapRule;
                if (respcode < 300)
                {
                    indexJson["ResponseMsg"] = "Delete the synonym map successfully";
                    indexJson["Success"] = "true";
                }
                else
                {
                    indexJson["ResponseMsg"] = "Fail to delete the synonym map. Response code: " + respcode.ToString();
                    indexJson["Success"] = "false";
                }

                if (indexfiekds != null && indexfiekds.Count<string>() > 0)
                {
                    JArray hubsArray = new JArray();
                    foreach (string item in indexfiekds)
                    {
                        hubsArray.Add(item);
                    }
                    indexJson["IndexFields"] = hubsArray; 
                }
                return indexJson.ToString();
            }
            catch (Exception e)
            {
                JObject indexJson = new JObject();
                indexJson["ResponseMsg"] = e.ToString();
                indexJson["Success"] = "false";
                return indexJson.ToString(); 
            }
        }

        [HttpPost]
        public string CreateUpdateSynonymsButtonClick(string searchName, string apiKey, string indexFields, string synonymMapName, string synonymMapRule)
        {
            try
            { 
                JObject indexJson = new JObject();
                var respcode = SearchIndexModel.CreateOrUpdateSynonymMap(searchName, apiKey, indexFields, synonymMapName, synonymMapRule);
                if (respcode < 300)
                {
                    indexJson["ResponseMsg"] = "Create or update the synonym map successfully";
                    indexJson["Success"] = "true";
                }
                else
                {
                    indexJson["ResponseMsg"] = "Fail to create or update the synonym map. Response code: " + respcode.ToString();
                    indexJson["Success"] = "false";
                }

                string synmapName = "";
                var synmaps = SearchIndexModel.GetSynonymMaps(searchName, apiKey, ref synmapName);  
                indexJson["synonymMapName"] = synonymMapName;
                indexJson["SynonymMaps"] = synmaps;
                 
                return indexJson.ToString();
            }
            catch (Exception e)
            {
                JObject indexJson = new JObject();
                indexJson["ResponseMsg"] = e.ToString();
                indexJson["Success"] = "false";
                return indexJson.ToString(); 
            }
        }

        [HttpPost]
        public string QueryIndexAutocompleteButtonClick(string searchName, string apiKey, string loadedindex, string sg, string term, string autocompleteInput)
        {
            try
            {
                var result = SearchIndexModel.QueryAutocompleteInIndex(searchName, apiKey, loadedindex, sg, term, autocompleteInput);
                return result;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        [HttpPost]
        public string UpdateIndexDataButtonClick(string searchName, string apiKey, string loadedindex, string indexDocument)
        {
            try
            {
                var result = SearchIndexModel.UpdateIndexDocument(searchName, apiKey, loadedindex, indexDocument);
                return result;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        [HttpPost]
        public string UpdateIndexDefinitionBtnClick(string searchName, string apiKey, string indexName, string indexDefinition)
        {
            try
            {
                var result = SearchIndexModel.UpdateIndexDefinition(searchName, apiKey, indexName, indexDefinition);
                return result;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        [HttpPost]
        public string LoadSearchIndexInfo(string searchName, string apiKey, string synonymMapName)
        {
            try
            {    
                JObject indexJson = new JObject();
                //var indexs = SearchIndexModel.ListIndexs(searchName, apiKey);
                string synmapName = "";
                var synmaps = SearchIndexModel.GetSynonymMaps(searchName, apiKey, ref synmapName);
                var synmapRule = SearchIndexModel.GetSynonymMapRule(searchName, apiKey, synmapName);
                var indexfiekds = SearchIndexModel.ListIndexsFieldsSynonyms(searchName, apiKey, synmapName);
                if (indexfiekds != null && indexfiekds.Count<string>() > 0)
                {
                    JArray hubsArray = new JArray(); 
                    foreach (string item in indexfiekds)
                    {
                        hubsArray.Add(item);
                    }
                    indexJson["IndexFields"] = hubsArray;
                     
                    var doc = SearchIndexModel.GetIndexDocument(searchName, apiKey, indexfiekds.First());
                    var sug = SearchIndexModel.GetIndexSuggesters(searchName, apiKey, indexfiekds.First());
                    var alz = SearchIndexModel.GetIndexAnalyzers(searchName, apiKey, indexfiekds.First());
                    var idxJs = SearchIndexModel.GetIndexDefinition(searchName, apiKey, indexfiekds.First()); 
                    indexJson["IndexDocument"] = doc;
                    indexJson["Suggesters"] = sug;
                    indexJson["Analyzers"] = alz;
                    indexJson["SynonymMaps"] = synmaps;
                    indexJson["SynmapRule"] = synmapRule;
                    indexJson["IndexJsonDefinition"] = idxJs;

                    List<string> searchableFieldlist = null;
                    List<string> facetableFieldlist = null;
                    List<string> retrievableFieldlist = null;
                    List<string> sortableFieldlist = null;
                    List<string> scoringProfileList = null; 
                    List<string> geographyPointList = null;
                    SearchIndexModel.GetIndexFieldsProperty(searchName, apiKey, indexfiekds.First(), ref searchableFieldlist, ref facetableFieldlist, 
                                                            ref retrievableFieldlist, ref sortableFieldlist, ref scoringProfileList, ref geographyPointList);

                    if (searchableFieldlist != null)
                    {
                        JArray searchableFieldArray = new JArray();
                        foreach (string item in searchableFieldlist)
                        {
                            searchableFieldArray.Add(item);
                        }
                        indexJson["SearchableFieldlist"] = searchableFieldArray;
                    }

                    if (facetableFieldlist != null)
                    {
                        JArray facetableFieldArray = new JArray();
                        foreach (string item in facetableFieldlist)
                        {
                            facetableFieldArray.Add(item);
                        }
                        indexJson["FacetableFieldlist"] = facetableFieldArray; 
                    }

                    if (retrievableFieldlist != null)
                    {
                        JArray retrievableFieldArray = new JArray();
                        foreach (string item in retrievableFieldlist)
                        {
                            retrievableFieldArray.Add(item);
                        }
                        indexJson["RetrievableFieldlist"] = retrievableFieldArray;
                    }

                    if (sortableFieldlist != null)
                    {
                        JArray SortableFieldArray = new JArray();
                        foreach (string item in sortableFieldlist)
                        {
                            SortableFieldArray.Add(item);
                        }
                        indexJson["SortableFieldlist"] = SortableFieldArray;
                    }
                     
                    if (scoringProfileList != null)
                    {
                        JArray scoringProfileArray = new JArray();
                        foreach (string item in scoringProfileList)
                        {
                            scoringProfileArray.Add(item);
                        }
                        indexJson["ScoringProfileList"] = scoringProfileArray;
                    }

                    if (geographyPointList != null)
                    {
                        JArray geographyPointArray = new JArray();
                        foreach (string item in geographyPointList)
                        {
                            geographyPointArray.Add(item);
                        }
                        indexJson["GeographyPointList"] = geographyPointArray;
                    }
                }

                //webapi skillset 
                var apiskills = SearchIndexModel.GetSkillSet(searchName, apiKey);
                JArray skillArray = new JArray(); 
                if (null != apiskills)
                {
                    foreach (string item in apiskills)
                    {
                        skillArray.Add(item);
                    }
                    indexJson["WebAPISkill"] = skillArray;
                }

                //semantic 
                var semantics = SearchIndexModel.GetSemanticConfigurations(searchName, apiKey, indexfiekds.First());
                JArray semanticArray = new JArray(); 
                if (null != semantics)
                {
                    foreach (string item in semantics)
                    {
                        semanticArray.Add(item);
                    }
                    indexJson["SemanticsName"] = semanticArray;
                }

                return indexJson.ToString(); 
            }
            catch (Exception e)
            { 
                return null;
            }
        }
    }
}
