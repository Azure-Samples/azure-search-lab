# Azure Cognitive Search Lab

Azure Cognitive Search Lab (refers as AzSearchLab) is a web based debugging tool for [Azure Cognitive Search](https://docs.microsoft.com/en-us/azure/search/search-what-is-azure-search) troubleshooting. It provides centralized interface to check the azure search documents, status and other features. It can also help engineer to analyze Search operation. 

## Features

This project framework provides the following features:

* Query composer 
* Index definition
* Document 
* Suggestions 
* Analyze text 
* Auto complete 
* Synonyms 
* Built-in analyzer 
* Web api skill  

## Getting Started

### Prerequisites

- .Net 5.0 or greater on your development machine. 
- Vistual Studio 2019 or greater version.
- Windows machine.

### Installation

- Download the source code and build it. 
- After finish building, you can run the website locally with IIS Express. 
- You can also deploy this website onto your app service. 
  ![image](https://user-images.githubusercontent.com/39817657/185533251-562d37d8-7d1b-4574-98fa-b731ff10eefa.png)

### Quickstart

1. Download the source code.
2. Build the project. 
3. Run the project with IIS express. 


## Demo

To run the demo, follow these steps:

1. Download the package and extract the source files. 
   ![image](https://user-images.githubusercontent.com/39817657/185562030-4cd4cc3c-d3fc-4c8b-b7db-587bc7371799.png)
2. Open the source code with Vistual Studio 2019 or higher version. 
3. Build the source code
 
   ![image](https://user-images.githubusercontent.com/39817657/185562237-c1c3a896-d171-42f2-8dcc-d611f7cb2fcd.png)
4. Run the source code with IIS Express. 

   ![image](https://user-images.githubusercontent.com/39817657/185562344-9561dc77-fbb6-4203-8c3f-ca0ed7eba981.png)
5. Enter the search service name and its key. Then load the data. 
   ![image](https://user-images.githubusercontent.com/39817657/185562784-0ee79f60-615b-437d-8b57-19ac4a7e93ff.png)


6. There are GET and POST type query. With this tool, we don't need to check the document for each parameter and its format. We can select the parameter on UI， then test the GET or POST query and analyze its query result.
   ![image](https://user-images.githubusercontent.com/39817657/185872176-abe48403-d002-43ab-827a-0de394e22aa7.png)
   ![image](https://user-images.githubusercontent.com/39817657/185872373-8b1a55b4-9776-436f-88e0-910921ed3d47.png)

7. Update index definition

   Some time, we need to update the index definition. But it is not allowed on portal and can only be changed via Rest API. We can change the index Json definition with this tool and update the index directly.
   ![image](https://user-images.githubusercontent.com/39817657/185872479-cdba35b9-f152-4750-ae8e-ab3c8a4e5a04.png)

8. Query and update document

   We also need to change and update the document after finish indexing. We can simply query the documents to find out the one that we want to change, then adjust it and update the changes to index.
   ![image](https://user-images.githubusercontent.com/39817657/185872835-2962cb74-7ad4-4cd4-9291-4c75eee8f992.png)

9. Test suggestion

   This tool provide a UI to simulate the suggestion scenario. So we can test the suggestion and implement similar scenario in our production environment.
   ![image](https://user-images.githubusercontent.com/39817657/185872930-0126a272-54fe-425f-bd68-ebcd77c2e508.png)

10. Test analyzer behavior

    We can load the search index and test its analyzers configured in the index. It will show us how the analyzer broke the texts into tokens.
   ![image](https://user-images.githubusercontent.com/39817657/185873041-4b6be693-3587-49d7-88d6-f042a38b8a46.png)

11. Test autocomplete feature

    We can test the auto-complete rule of the index and confirm its behavior.
   ![image](https://user-images.githubusercontent.com/39817657/185873110-9ca0736d-0bc3-4793-956b-c2454fd2061d.png)

12. Create, update and delete synonyms

    We can create, update or delete the synonym on index fields.
    ![image](https://user-images.githubusercontent.com/39817657/185873285-8120225f-f48d-4b0c-9383-07b2146088d8.png)

    We can create new synonym by specify the synonym name, its rule and its applied index fields. We can use Ctrl+Click the fields to multiple select the fields we want to apply.
    ![image](https://user-images.githubusercontent.com/39817657/185873411-cb021a9b-e663-4dd9-a1b2-1615c1c71ca0.png)

    We can also delete an existing synonym.
    ![image](https://user-images.githubusercontent.com/39817657/185873512-24c62554-d48f-4d92-8f0c-34f5329a4333.png)

    We can also update field's synonym by select or un-select them. Please notice that each field can ONLY contains 1 synonym. So if we update the field with a new synonym, the previous synonym configured on this field will be removed automatically.
    ![image](https://user-images.githubusercontent.com/39817657/185873603-0d01be10-9b3b-4abe-8336-3d23a0955b09.png)

13. Test built-in analyzer, tokenizer, token filter and char filter

    We can also test different built-in analyzer, tokenizer, token filter and char filter to select the one that fit for our project. It will show us how analyzer, tokenizer, token filter and char filter break text into tokens.
    ![image](https://user-images.githubusercontent.com/39817657/185873724-aee72089-b279-4a3b-8b9c-2b6e303d4443.png)
