# GeneticAlgNetControl

## Table of contents

* [Introduction](#introduction)
* [Input](#input)
* [Output](#output)

## Introduction

This directory contains the data sets which have been used in testing the application. The source of each network is presented in the table below.

Networks | Source
:--- | :---:
Breast DEF, Ovarian DEF, Pancreatic DEF, SIGNOR BrOvPa DEF | [Link](https://doi.org/10.1038/s41598-017-10491-y)
Breast HCC1428, MDA-MB-361, Ovarian O1946, OVCA8, Pancreatic AsPC-1, KP-3 | [Link](https://doi.org/10.1093/nar/gkr959)
Social Interaction 1, 3 | [Link](https://doi.org/10.1126/science.1089167)
Electronic Circuit 208, 420, 838 | [Link](https://doi.org/10.1126/science.298.5594.824)
Random Erdos-Renyi 100, 500, 1000, 1500, 2000, 2500, 300 | [Link](https://networkx.github.io/documentation/stable/reference/generated/networkx.generators.random_graphs.fast_gnp_random_graph.html).
Random Scale Free 100, 500, 1000, 1500, 2000, 2500, 300 | [Link](https://networkx.github.io/documentation/stable/reference/generated/networkx.generators.directed.scale_free_graph.html?highlight=scale_free#networkx.generators.directed.scale_free_graph).
Random Small World 100, 500, 1000, 1500, 2000, 2500, 300 | [Link](https://networkx.github.io/documentation/stable/reference/generated/networkx.generators.random_graphs.watts_strogatz_graph.html?highlight=watts_st#networkx.generators.random_graphs.watts_strogatz_graph).

## Input

The ``Input`` directory contains examples of input data that can be read by the application. The actual networks can be found in the text files named ``Network_Name``. Each network file has a corresponding target nodes file named ``Network_Name_Target``. Both such files are needed for one run of the application. Furthermore, the files corresponding to the protein-protein interaction networks have an additional drug-target nodes file named ``Network_Name_Preferred``, which can be used as supplementary input for the application.

## Output

The ``Output`` directory contains the results obtained after running the application once on each of the networks in the ``Input`` directory. Each output consists of a JSON file named ``Network_Name_Output``. In addition, the application has been run once more for all the protein-protein interaction networks while using the additional preferred nodes. The files named ``Network_Name_Output_Preffered`` contain these results.

Given the stochastic nature of the algorithm, the results obtained after each run of the application might vary. Each file represents only one possible output.
