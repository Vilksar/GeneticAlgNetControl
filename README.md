# GeneticAlgNetControl

## Table of contents

* [Introduction](#introduction)
* [Download](#download)
  * [Build](#build)
  * [Release](#release)
* [Usage](#usage)
  * [Usage (GUI)](#usage-gui)
  * [Usage (CLI)](#usage-cli)
* [Examples](#examples)
 
## Introduction

Welcome to the GeneticAlgNetControl repository!

This is a C# / .Net Core application which aims to solve the target network controllability problem using genetic algorithms. The application is cross-platform, working on all modern operating systems (Windows, MacOS, Linux), and can be run through a web-based GUI (graphical user interface), or via CLI (command-line interface).

## Download

You can either download the repository or the already compiled application.

### Build

The repository consists of a Visual Studio 2022 project. You can download it to run or build the application yourself. You need to have [.NET 6](https://dotnet.microsoft.com/download/dotnet/6.0) installed on your computer in order to run it, or the corresponding SDK in order to also be able to build it.

### Release

You can directly download the application (corresponding to your operating system) from the [releases page](../../releases). No prerequisites are needed in order to run it, although permission to execute the application and permission to create files might be required.

After downloading the ZIP archive, simply unarchive it and it is ready to run, without any installation. Please note that on MacOS and Linux it might be needed to manually mark the file `GeneticAlgNetControl` as executable. One way to achieve that can be to navigate to the unarchived directory and execute the following command in the terminal.

```
chmod 755 GeneticAlgNetControl
```

## Usage

Some options (such as CLI) require adding several command-line arguments when launching the application. This can be achieved by launching the application from your operating system's terminal or console. You can view several examples of launch commands in the [examples](#examples) section. In order to find out more about the usage and possible arguments which can be used, you can launch the application with the `--Mode` argument set to `Help`, for example:

```
--Mode "Help"
```

### Usage (GUI)

To run the application via GUI, it is enough to double click the `GeneticAlgNetControl` file or launch it from the terminal either with no arguments (default), or with the `--Mode` argument set to `Web`, for example:

```
--Mode "Web"
```

This mode has one optional argument:

* `--Urls`. (optional) Use this argument to specify the local address on which to run the web server. It can also be configured in the `appsettings.json` file. The default value is `http://localhost:5000`.

Logging is enabled by default, so you will get several information messages throughout the execution of the application, firstly notifying that a local web server was started on your computer. If there is another local server that uses `localhost` on the default ports `5000` and `5001`, the application may encounter an error and fail to start. You can change the ports that are used by launching the application with the `--Urls` argument.

Your default browser should open automatically to the home page of the application. If not, you should copy the local web server's address and paste it into the address bar of your favorite browser. Afterwards, it can be used just as any other website / web application (with the difference that everything runs locally, so no internet connection is needed). The application was tested on Chrome, but it should render and work nicely in any browser.

If the `https` protocol is used (thus, requesting a secure connection), your browser might issue a warning that the certificate of the website could not be verified, thus the connection might not be secure. Considering that everything happens locally (the web server is hosted on your computer, and the browser connects locally to it), no certificate is actually needed, as no data gets transfered, and the warning can be safely ignored (by selecting the options similar to "I understand the risks." or "I want to continue anyway.").

To close the application, you can navigate to the `Quit` page, or you can simply select the terminal where the application is running (and not the browser!) again and press `CTRL + C` (or `CMD + C` on MacOS). It is recommended to wait for it to gracefully shut down, as this will automatically stop and save the progress of all currently running analyses. If you forcefully close the application, you will lose all progress on the currently running analyses, however they will automatically restart on the next application launch.

### Usage (CLI)

To run the application via CLI, you need to launch it from the terminal with the `--Mode` argument set to `Cli`, for example:

```
--Mode "Cli"
```

This mode has three mandatory arguments (omitting any of them will return an error) and two optional ones:

* `--Edges`. Use this argument to specify the path to the file containing the edges of the network. Each edge should be on a new line, with its source and target nodes being separated by a semicolon character, for example:
  
  ```
  Node A;Node B
  Node A;Node C
  Node A;Node D
  Node C;Node D
  ```
  
  If the file is in a different format, or no nodes or edges could be found, an error will be returned. The order of the nodes is important, as the network is directed. Thus, `Node A;Node B` is not the same as `Node B;Node A`, and they can both appear in the network. Any duplicate edges will be ignored. The set of nodes in the network will be automatically inferred from the set of edges. This argument does not have a default value.

* `--Targets`. Use this argument to specify the path to the file containing the target nodes of the network. Each node should be on a new line.
  
  ```
  Node C
  Node D
  ```
  
  If the file is in a different format, or no nodes could be found in the network, an error will be returned. Only the nodes which already appear in the network will be considered. Any duplicate nodes will be ignored. This argument does not have a default value.

* `--Preferred`. (optional) Use this argument to specify the path to the file containing the preferred nodes of the network. Each node should be on a new line.
  
  ```
  Node A
  Node C
  ```
  
  If the file is in a different format, or no nodes could be found in the network, an error will be returned. Only the nodes which already appear in the network will be considered. Any duplicate nodes will be ignored. This argument does not have a default value.

* `--Parameters`. Use this argument to specify the path to the file containing the parameter values for the analysis. The file should be in JSON format, using the same model as the `DefaultParameters.json` file, which contains the default parameter values.
  
  ```
  {
    "RandomSeed": -1,
    "MaximumIterations": 100,
    "MaximumIterationsWithoutImprovement": 25,
    "MaximumRunningTime": 0,
    "MaximumPathLength": 5,
    "PopulationSize": 20,
    "RandomGenesPerChromosome": 25,
    "PercentageRandom": 0.25,
    "PercentageElite": 0.25,
    "ProbabilityMutation": 0.01,
    "CrossoverType": 0,
    "MutationType": 0
  }
  ```
  
  The parameters are presented below.
  
  * `RandomSeed`. Represents the random seed for the current algorithm run. A value of `-1` will generate a random value for the seed. It must be a positive integer, and its default value is `-1`.
  * `MaximumIterations`. Represents the maximum number of generations for which the algorithm will run. It must be a positive integer, and its default value is `100`.
  * `MaximumIterationsWithoutImprovement`. Represents the maximum number of generations without an improvement in the fitness of the best chromosome in the population for which the algorithm will run. It must be a positive integer, and its default value is `25`.
  * `MaximumRunningTime`. Represents the maximum number of seconds for which the algorithm will run. A value of `0` will not restrict the running time. It must be a positive integer, and its default value is `0`.
  * `MaximumPathLength`. Represents the maximum number of edges in a control path. It must be a positive integer, and its default value is `5`.
  * `PopulationSize`. Represents the total number of chromosomes in a generation. It must be a positive integer greater than `1`, and its default value is `20`.
  * `RandomGenesPerChromosome`. Represents the maximum number of randomly generated genes in a chromosome. It must be a positive integer, and its default value is `25`.
  * `PercentageElite`. Represents the maximum percentage of elite chromosomes in a generation. It must be a real number in the `[0, 1]` interval, and its default value is `0.25`.
  * `PercentageRandom`. Represents the maximum percentage of randomly generated chromosomes in a generation. It must be a real number in the `[0, 1]` interval, and its default value is `0.25`.
  * `ProbabilityMutation`. Represents the probability of mutation for a chromosome. It must be a real number in the `[0, 1]` interval, and its default value is `0.01`.
  * `CrossoverType`. Represents the crossover algorithm to be used by the genetic algorithm. It must be an integer in the set `{0, 1, 2, 3}`, and its default value is `0`.
  * `MutationType`. Represents the mutation algorithm to be used by the genetic algorithm. It must be an integer in the set `{0, 1, 2, 3}`, and its default value is `0`.
  
  If the file is in a different format, an error will be returned. Additionally, if any of the parameters are missing, their default values will be used. This argument does not have a default value.

* `--Output`. (optional) Use this argument to specify the path to the output file where the solutions of the analysis will be written. Permission to write is needed for the corresponding folder. If a file with the same name already exists, it will be automatically overwritten. The default value is the name of the file containing the edges, followed by the current date and time.

If all the files have been successfully read and loaded, a confirmation message will be logged to the terminal and the algorithm will start running, providing constant feedback on its progress. Upon completion, all of the solutions will be written to the JSON file specified by the `--Output` argument.

## Examples

These are a few examples of possible command-line parameters for running the application.

* Help
  
  ```
  ./GeneticAlgNetControl --Mode "Help"
  ```
  
* GUI
  
  ```
  ./GeneticAlgNetControl
  ```
  
  ```
  ./GeneticAlgNetControl --Mode "Web"
  ```
  
  ```
  ./GeneticAlgNetControl --Mode "Web" --Urls "http://localhost:5000"
  ```
  
* CLI
  
  ```
  ./GeneticAlgNetControl --Mode "Cli" --Edges "DataSets/Social_Interaction_3.txt" --Targets "DataSets/Social_Interaction_3_Target.txt" --Parameters "DefaultParameters.json"
  ```
  
  ```
  ./GeneticAlgNetControl --Mode "Cli" --Edges "DataSets/Breast_MDA-MB-361.txt" --Targets "DataSets/Breast_MDA-MB-361_Target.txt" --Preferred "samples/Breast_MDA-MB-361_Preferred.txt" --Parameters "DefaultParameters.json"
  ```
  
  ```
  ./GeneticAlgNetControl --Mode "Cli" --Edges "path/to/file.extension" --Targets "path/to/file.extension" --Parameters "path/to/file.extension"
  ```
  
  ```
  ./GeneticAlgNetControl --Mode "Cli" --Edges "path/to/file.extension" --Targets "path/to/file.extension" --Preferred "path/to/file.extension" --Parameters "path/to/file.extension"
  ```
