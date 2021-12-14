# Deep Learning Visualization
### CprE 482x Bounty Points Project
### Ethan Ruchotzke, Fall 2021

![A model trained with 10 epochs](https://github.com/Ruchotzke/DNN-Visualizer/blob/main/Images/10-epochs.png)
## Description
The Deep Learning Visualizer is a simple Unity application intended to teach individuals about neural networks. Specifically, this application is intended to help users gain intuition of the backpropagation algorithm, and how repeated backpropagation is used to train a model.
## Build Instructions
### Technical Details
Engine: `Unity 2021.2.6f1`
Modules: `WebGL Build`
### Build Details
If you would like to rebuild this project from source (not necessary), you will need to download the correct version of Unity, along with the `WebGL build` module from Unity Hub (download link: https://unity.com/download)
Then, once in Unity, follow these instructions (https://www.instructables.com/How-to-BuildExport-Your-Game-in-Unity-to-Windows/) but select `WebGL` instead of `Windows` in the build settings. Ensure `use compression` is DISABLED.
Once again, this is NOT the recommended option for using the program.
### To Use the Program
A WebGL build has already been provided for you in the repository (`WebGL_Build`). To use this build:
1. Clone the repository (if you want everything) or download the `WebGL_Build` folder
2. Launch a web server, targeting the `index.html` file in the build folder.
	1. I would personally recommend using python's built in HTTP server for this. Simply navigate to that directory and run the command `python -m http.server`to start a local web server. Then, navigate to `localhost:PORT` to open the application in your browser.
3. If you open the corresponding web page in your browser, you should be presented with the application.
4. NOTE: YOUR BROWSER MUST SUPPORT WEB GL
![Browser Application Running](https://github.com/Ruchotzke/DNN-Visualizer/blob/main/Images/Browser.png)
## Features
The chosen application for this visualizer is a neural network which can learn to sum two real values in the range [-1, 1]. The network <b>IS REQUIRED</b> to have 2 input neurons and 1 output neuron. See backlog for more information about this limitation. Each neuron in the network uses a Sigmoid activation function, except for the outputs who use a linear (no) activation.

![GUI](https://github.com/Ruchotzke/DNN-Visualizer/blob/main/Images/10-epochs.png)

The application is divided into several regions:
### Top Bar - Network Editing

### Bottom Bar - Visualization

### Right Panel - Training and Results
## Feature Backlog


