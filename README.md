# GeneticAlgNetControl
 
## Introduction

Welcome to the GeneticAlgNetControl repository! This is a C# / .Net Core application which aims to solve the target network controllability problem using genetic algorithms. The application is cross-platform, working on all modern operating systems (Windows, MacOS, Linux) and can be run through a web-based GUI (graphical user interface), or via CLI (command-line interface).

## Download

You can download the application as a single, runnable JAR file, from the [releases page](https://github.com/vicbgdn/GeneticAlgNetControl/releases). No prerequisites are needed in order to run it, although administrator rights (namely, rights to create files in the current working directory) might be needed if running the application via CLI. After downloading the ZIP archive for your operating system, simply unarchive it and it is ready to run, no installation being required.

## Usage

Several launch options (such as CLI) require adding several arguments to the application executable. The easiest way to do this would be to launch the application from the OS' terminal (either CMD or Powershell for Windows, or Terminal for MacOS and Linux).

### Usage (GUI)

To run the application via GUI, it is enough to launch the executable (either by double clicking on it, or launching it from the command line). You will get several information messages, notifying that a local web server was started on your computer, and your default browser will open to the home page of the application. The application was tested on Chrome, but it should render and work nicely in any browser.

To close the application, simply select it (and not the browser!) again and press `CTRL + C` (or `CMD + C` on MacOS). It is recommended to wait for it to gracefully shut down. If you force it to end, you will lose all progress on currently running algorithms (although none of the other ones will be affected), but they will automatically start running again on the next application launch.

*Note*: If you have another server using *localhost* on the default ports `5000` and `5001`, the application may encounter an error and fail to start. You can configure the ports that it will use by launching it using the `--Urls` argument. For example, omitting this argument entirely is equivalent with launching the program using:

```
--Urls "http://localhost:5000;https://localhost:5001"
```

### Usage (CLI)

To run the application via CLI, you need to launch it using the `--UseCli` argument, for example:

```
--UseCli "true"
```

If you would like to know the required arguments, you can also add the `--Help` argument>

```
--UseCli "true" --Help "true"
```

Either way, there are three arguments that always need to be specified (in addition to one optional). Omitting any of the mandatory ones will result into the application encountering an error. The mandatory ones are `--Edges`, specifying the path to a file containing the network edges, `--Targets`, specifying the path to a file containing the list of target nodes, and `--Parameters`, specifying the path to a file containing the parameters to be used by the algorithm. The optional argument is `--Preferred`, specifying the path to a file containing a list of preferred nodes. Thus, a complete set of arguments for an algorithm run via the CLI could be:

```
--UseCli "true" --Edges "path/to/edges/file.extension" --Targets "/path/to/targets/file.extension" --Parameters "/path/to/parameters/file.extension"
```

The required formats for each of these files will be presented next.
