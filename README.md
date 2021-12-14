# Deep Learning Visualization
### CprE 482x Bounty Points Project
### Ethan Ruchotzke, Fall 2021

![Construct a Network](https://github.com/Ruchotzke/DNN-Visualizer/blob/main/Images/construct_network.gif)
Constructing a network.

![Both propagations](https://github.com/Ruchotzke/DNN-Visualizer/blob/main/Images/propagation.gif)
Propagating values through the network.

![Training](https://github.com/Ruchotzke/DNN-Visualizer/blob/main/Images/training.gif)
Training the network.

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
- `Add Neuron`
	- Left click to add a new neuron to the model.
- `Remove Neuron`
	- Left click the a neuron to remove it from the model. Occasionally buggy if you remove an output.
- `Connect Neuron`
	- Click and drag from a source neuron to a destination neuron to connect them. Auto generates a random weight.
- `Remove Connection`
	- <b> NOT IMPLEMENTED YET</b>
- `Mark Input/Output`
	- Left click a neuron to cycle it between an input neuron (blue), output neuron (red), and hidden neuron (gray).
- `Move Neuron`
	- Left click and drag a neuron to move it around the screen.

### Bottom Bar - Visualization
- `Inference`
	- Run a single inference on the current model. 
	- The data point is a random element from the generated dataset of numbers and their sums.
- `Backpropagation`
	- Complete a single backpropagation on the model. 
	- <b> IMPORTANT: This uses values from the previous inference. Do not run without running an inference first!</b>
	- <b> NOTE: This uses the previous input for training. If you don't re-run inference between consecutive backpropagation presses, the gradient will not be updated correctly!
- `Show Error Path`
	- Click a neuron to show the path error takes to get from the output to this neuron. 
	- Useful to illustrate that error is <b> additive </b> across all paths.

![Error path towards neuron 0](https://github.com/Ruchotzke/DNN-Visualizer/blob/main/Images/error-path.png)

### Right Panel - Training and Results
 - `MSE Per Epoch`
	- The mean-squared-error of the model across the entire dataset for a given epoch.
	- MSE is used as it is a good loss function when predicting real-valued outputs.
 - `Acc. Per Epoch`
	- The accuracy of the model across the entire dataset for a given epoch.
	- Accuracy is defined as the percentage of predicted outputs who were within 0.1 of their correct output.
 - `Train for Epochs`
	- Several "large scale" training buttons are provided for your usage in demonstration. 
	- Each epoch will do a single stochastic gradient descent across the entire dataset, updating the graphs and statistics accordingly.
## Feature Backlog
There are many features which I would like to implement for this project which were not included for the original project submission:
 - More diverse models. I would like to implement a "Color Classifier" model which would have 3 inputs (R,G,B) and 8 output color choices. The infrastructure is already complete to collect this data.
 - Better visualization
	 - More speed options
	 - More graphics (multiply, activation, derivative graphics during propagation)
	 - Better error path visualization
	 - More intuitive UI (ability to hide panels)
 - Better Formatting / Generation Options
	 - Ability to auto generate a "layer" of neurons and fully connect them to another layer
	 - Hover over connections to view weight value / statistics for that weight
	 - Activation displayed when mousing over a neuron

## More Images
![Forward Propagation in Progress](https://github.com/Ruchotzke/DNN-Visualizer/blob/main/Images/ongoing-inference.png)
![Backpropagation in Progress](https://github.com/Ruchotzke/DNN-Visualizer/blob/main/Images/ongoing-backprop.png)
![Trained Model](https://github.com/Ruchotzke/DNN-Visualizer/blob/main/Images/training.png)
