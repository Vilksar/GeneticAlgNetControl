# GeneticAlgNetControl
 
## Introduction

Welcome to the GeneticAlgNetControl repository! This is a C# / .Net Core application which aims to solve the target network controllability problem using genetic algorithms. The application is cross-platform, working on all modern operating systems (Windows, MacOS, Linux) and can be run through a web-based GUI (graphical user interface), or via CLI (command-line interface).

## Download

You can download the application as a single, runnable JAR file, from the [releases page](https://github.com/vicbgdn/GeneticAlgNetControl/releases). No prerequisites are needed in order to run it, although administrator rights (namely, rights to create files in the current working directory) might be needed. After downloading the ZIP archive for your operating system, simply unarchive it and it is ready to run, no installation being required.

## Usage

Several launch options (such as CLI) require adding several arguments to the application executable. The easiest way to do this would be to launch the application from the OS' terminal (either CMD or Powershell for Windows, or Terminal for MacOS and Linux).

### Usage (GUI)

To run the application via GUI, it is enough to launch the executable (either by double clicking on it, or launching it from the command line). You will get several information messages, notifying that a local web server was started on your computer, and your default browser will open to the home page of the application. Afterwards, it can be used just as any other website / web application (with the difference that everything runs locally, so no internet connection is needed). The application was tested on Chrome, but it should render and work nicely in any browser. Once the web browser is opened on the 

If there exists another local server using `localhost` on the default ports `5000` and `5001`, the application may encounter an error and fail to start. You can configure the ports that it will use by launching it using the `--Urls` argument. For example, omitting this argument entirely is equivalent with launching the program using:

```
--Urls "http://localhost:5000;https://localhost:5001"
```

Upon the first launch, your browser might issue an warning that the certificate of the website could not be verified, thus the connection might not be secure. Considering that everything happens locally (the web server is hosted on your computer, and the browsar connects locally to it), no certificate is actually needed, as no data gets transfered, and the warning can be safely ignored (by selecting the options "I understand the risks." or "I want to continue anyway.").

To close the application, simply select it (and not the browser!) again and press `CTRL + C` (or `CMD + C` on MacOS). It is recommended to wait for it to gracefully shut down. If you close the application, you will lose all progress on currently running algorithms (although none of the other ones will be affected), but they will automatically start running again on the next application launch. This is the reason why it is recommended to stop beforehand all ongoing algorithms.

### Usage (CLI)

To run the application via CLI, you need to launch it using the `--UseCli` argument set to `true`, for example:

```
--UseCli "true"
```

If you would like to know the possible and required arguments to run it using the CLI, you can also add the `--Help` argument.

```
--UseCli "true" --Help "true"
```

Either way, there are three arguments that always need to be specified (in addition to an optional one). Omitting any of the mandatory arguments will result into the application returning an error. The mandatory ones are `--Edges`, specifying the path to a file containing the network edges, `--Targets`, specifying the path to a file containing the list of target nodes, and `--Parameters`, specifying the path to a file containing the parameters to be used by the algorithm. The optional argument is `--Preferred`, specifying the path to a file containing a list of preferred nodes. Thus, a complete set of arguments for an algorithm run via the CLI could be:

```
--UseCli "true" --Edges "path/to/edges/file.extension" --Targets "/path/to/targets/file.extension" --Parameters "/path/to/parameters/file.extension"
```

The required format of these files, as well as several more details about them are presented below.

* The "**Edges**" file must containg ordered pairs of nodes, each on a separate line. The source and target node of an edge must be separated by a tab character. For example:

```
Node A    Node B
Node A    Node C
Node A    Node D
Node C    Node D
```

  If the file is in a different format, or no nodes or edges could be found, an error will be returned. The order of the nodes is important, as the network is directed. Thus, `Node A    Node B` is not the same as `Node B    Node A`, and they can both appear in a network. Any duplicate edges will be ignored. The set of nodes of the network will be automatically taken from the set of edges.

* The "**Targets**" file must contain a list of nodes, each on a separate line. For example:

```
Node C
Node D
```

  If the file is in a different format, or no nodes could be found in the network, an error will be returned. The node names should match the ones used when defining the edges. Additionally, any given target nodes that do not appear in the network will simply be ignored.

* (*optional*) The "**Preferred**" file must contain a list of nodes, each on a separate line. For example:

```
Node A
Node C
```

  If the file is in a different format, an error will be returned. The node names should match the ones used when defining the edges. A node can be both a target node and a preferred node. Additionally, the given preferred nodes that do not appear in the network will simply be ignored.

* The "**Parameters**" file must contain a JSON object, properly formatted. If any of the parameters are missing, then their default values will be used. Upon launching the application for the first time via the CLI with the argument `--Help "true"`, a new file, named `DefaultParameters.json`, will be created in the working directory. This file provides an editable model of the required type of parameters file and contains the default values of all parameters.

```
{
  "RandomSeed": -1,
  "MaximumIterations": 10000,
  "MaximumIterationsWithoutImprovement": 1000,
  "MaximumPathLength": 5,
  "PopulationSize": 80,
  "RandomGenesPerChromosome": 15,
  "PercentageRandom": 0.25,
  "PercentageElite": 0.25,
  "ProbabilityMutation": 0.01,
  "CrossoverType": 0,
  "MutationType": 0
}
```

  The possible parameters, their meaning, as well as their restrictions, are presented below.
  
  * **Random seed**. Represents the random seed for the current algorithm run. It must be a positive integer. However, if it is `-1`, then it will be randomly generated. Its default value is `-1`.
  * **Number of iterations**. Represents the total number of generations for which the algorithm will run. It must be a positive integer, and its default value is `10000`.
  * **Number of iterations without improvement**. Represents the total number of generations without an improvement in the fitness of the best chromosome in the population. It must be a positive integer, and its default value is `1000`.
  * **Population size**. Represents the total number of chromosomes in a generation. It must be a positive integer greater than `1`, and its default value is `80`.
  * **Maximum path length**. Represents the maximum number of interactions in a control path. It must be a positive integer, and its default value is `5`.
  * **Random elements per step**. Represents the maximum number of randomly generated genes in a chromosome. It must be a positive integer, and its default value is `15`.
  * **Probability of mutation**. Represents the probability of mutation for a chromosome. It must be a real number in the `[0, 1]` interval, and its default value is `0.01`.
  * **Percentage of elite individuals**. Represents the maximum percentage of elite chromosomes in a generation. It must be a real number in the `[0, 1]` interval, and its default value is `0.25`.
  * **Percentage of random individuals**. Represents the maximum percentage of randomly generated chromosomes in a generation. It must be a real number in the `[0, 1]` interval, and its default value is `0.25`.
  * **Crossover algorithm**. Represents the crossover algorithm to be used by the genetic algorithm. It must be an integer in the set `{0, 1, 2, 3}`, and its default value is `0`.
  * **Mutation algorithm**. Represents the mutation algorithm to be used by the genetic algorithm. It must be an integer in the set `{0, 1, 2, 3}`, and its default value is `0`.

If all the files have been successfully read and loaded, a confirmation message will appear in the console and the algorithm will start running, providing constant feedback on its progress.
