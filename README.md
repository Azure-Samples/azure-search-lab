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
  ![image](https://user-images.githubusercontent.com/39817657/185533251-562d37d8-7d1b-4574-98fa-b731ff10eefa.png)

1. Download the package and extract the source files. 
   ![image](https://user-images.githubusercontent.com/39817657/185562030-4cd4cc3c-d3fc-4c8b-b7db-587bc7371799.png)

2. Open the source code with Visual Studio 2019 or higher version. 

3. Press F5 to build the project:
 
   ![image](https://user-images.githubusercontent.com/39817657/185562237-c1c3a896-d171-42f2-8dcc-d611f7cb2fcd.png)

4. Run the source code with IIS Express. 

   ![image](https://user-images.githubusercontent.com/39817657/185562344-9561dc77-fbb6-4203-8c3f-ca0ed7eba981.png)

5. Enter the search service name and its key. Then load the service information. You should see all of the indexes that exist in your service.

6. There are GET and POST type query. With this tool, you don't need to check the document for each parameter and its format. You can select the parameter on UI， then test the GET or POST query and analyze its query result.

7. Update an index definition.

   Updates to an index definition isn't allowed on the Azure portal and can only be changed programmatically. Azure Search Lab supports direct updates to an index JSON definition.

8. Query and update document.

   You can also change and update a search document after it's been indexed. For this task, query the documents to find one you want to change, then make your update and save the changes to index.

9. Test suggestions.

   This tool provide a UI to simulate suggestions.

10. Test analyzer behaviors.

    You can load a search index and test its analyzer configurations. This step shows you how the analyzer processes text into tokens.

11. Test autocomplete.

    You can test the auto-complete rule of the index and confirm its behavior.
   ![image](https://user-images.githubusercontent.com/39817657/185873110-9ca0736d-0bc3-4793-956b-c2454fd2061d.png)

12. Create, update and delete synonyms.

    You can create, update or delete the synonym on index fields.
    ![image](https://user-images.githubusercontent.com/39817657/185873285-8120225f-f48d-4b0c-9383-07b2146088d8.png)

    You can create new synonym by specify the synonym name, its rule and its applied index fields. You can use Ctrl+Click the fields to multiple select the fields you want to apply.
    ![image](https://user-images.githubusercontent.com/39817657/185873411-cb021a9b-e663-4dd9-a1b2-1615c1c71ca0.png)

    You can also delete an existing synonym.
    ![image](https://user-images.githubusercontent.com/39817657/185873512-24c62554-d48f-4d92-8f0c-34f5329a4333.png)

    You can also update field's synonym by select or un-select them. Notice that each field can ONLY contain 1 synonym. So if you update the field with a new synonym, the previous synonym configured on this field will be removed automatically.
    ![image](https://user-images.githubusercontent.com/39817657/185873603-0d01be10-9b3b-4abe-8336-3d23a0955b09.png)

13. Test built-in analyzer, tokenizer, token filter and char filters.

    You can also test different built-in analyzer, tokenizer, token filter and char filter to select the one that fit for our project. It will show you how analyzer, tokenizer, token filter and char filter break text into tokens.
  
