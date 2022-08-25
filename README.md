# Azure Cognitive Search Lab

Azure Cognitive Search Lab (also known as AzSearchLab) is a web-based debugging tool for [Azure Cognitive Search](https://docs.microsoft.com/azure/search/search-what-is-azure-search) troubleshooting. It provides a centralized interface for designing and testing indexing and query operations. 

## Features

This project framework is designed to run locally. It connects to your Azure Cognitive Search service, with the ability to test the following features:

* Queries
* Index definitions
* Documents
* Suggestions 
* Analyzers
* Autocomplete 
* Synonyms 
* Built-in analyzers
* Web api skills

### Prerequisites

This project assumes a Windows development environment.

- [.Net 5.0 or greater](https://dotnet.microsoft.com/download/dotnet)
- [Vistual Studio 2019 or greater](https://visualstudio.microsoft.com/downloads/)
- Azure Cognitive Search in your Azure subscription

## Getting Started

Download the source code, open the solution in Visual Studio, and press F5 to build the project. After it builds, you can run the website locally with IIS Express. 

Alternatively, you can also deploy this website to your app service. 
  ![image](Image/Deploy-this-website.jpg)

1. Download the package and extract the source files. 
   ![image](Image/Download-the-package.jpg)

2. Open the source code with Visual Studio 2019 or higher version. 

3. Press F5 to build the project:
 
   ![image](Image/Press-F5-to-build.jpg)

4. Run the source code with IIS Express. 

   ![image](Image/Run-the-source-code.jpg)

5. Enter the search service name and its key. Then load the service information. You should see all of the indexes that exist in your service.

6. There are GET and POST type query. With this tool, you don't need to check the document for each parameter and its format. You can select the parameter on UI， then test the GET or POST query and analyze its query result.

   ![image](Image/Test-the-query-parameter.jpg)

   ![image](Image/Query-result.jpg)

7. Update an index definition.

   Updates to an index definition isn't allowed on the Azure portal and can only be changed programmatically. Azure Search Lab supports direct updates to an index JSON definition.

   ![image](Image/Update-index-definition.jpg)

8. Query and update document.

   You can also change and update a search document after it's been indexed. For this task, query the documents to find one you want to change, then make your update and save the changes to index.
   
   ![image](Image/Query-and-update-document.jpg)

9. Test suggestions.

   This tool provide a UI to simulate suggestions.
   
   ![image](Image/Test-suggestion.jpg)

10. Test analyzer behaviors.

    You can load a search index and test its analyzer configurations. This step shows you how the analyzer processes text into tokens.
    
    ![image](Image/Test-analyzer-behavior.jpg)

11. Test autocomplete.

    You can test the auto-complete rule of the index and confirm its behavior.
   ![image](Image/Test-autocomplete-feature.jpg)

12. Create, update and delete synonyms.

    You can create, update or delete the synonym on index fields.
    ![image](Image/Create-update-and-delete-synonyms.jpg)

    You can create new synonym by specify the synonym name, its rule and its applied index fields. You can use Ctrl+Click the fields to multiple select the fields you want to apply.
    ![image](Image/We-can-create-new-synonym.jpg)

    You can also delete an existing synonym.
    ![image](Image/We-can-also-delete-an-existing-synonym.jpg)

    You can also update field's synonym by select or un-select them. Notice that each field can ONLY contain 1 synonym. So if you update the field with a new synonym, the previous synonym configured on this field will be removed automatically.
    ![image](Image/We-can-also-update-field.jpg)

13. Test built-in analyzer, tokenizer, token filter and char filters.

    You can also test different built-in analyzer, tokenizer, token filter and char filter to select the one that fit for our project. It will show you how analyzer, tokenizer, token filter and char filter break text into tokens.
    
    ![image](Image/Test-built-in-analyzer.jpg)
  

14. Test Custom Web Api Skill.

    We can also test the custom web api skill's behavior. The URL, method, headers and payload format will be extracted from the web api skill automatically. Then we need to change the payload to the real data or test data. Make POST or PUT request to web api server. And the response from web api server will show up on UI.
    
    ![image](Image/Test-custom-web-api.jpg)
