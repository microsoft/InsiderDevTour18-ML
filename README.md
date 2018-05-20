
# InsiderDevTour Machine Learning session demos

## Prerequisites
1. Windows 10 April 2018 Update
1. Visual Studio 2017 (15.7.1 or higher) (Community works great)
    - Universal Windows Platform development workload with Windows 10 SDK (10.0.17134.0)
1. Git (recommended)
1. [Netron](https://github.com/lutzroeder/Netron/releases) for visualizing ONNX models

## Getting started with WinML and Emotion model

### Resources
1. [Emotion recognition in faces model](https://gallery.azure.ai/Model/Emotion-recognition-in-faces-FER)

### Setup
1. Open *Microsoft Edge* and navigate to the *Emotion recognition* model linked above
1. Open *EmotionML/EmotionML.sln* in Visual Studio
1. Create the snippets that you will drag and drop in the code during the demo
    - Open MainPage.cs and find the two snippets - there should be a comment above each snippet
    - Make sure the toolbox is opened and pinned in Visual Studio
    - Select each snippet and drag it to the toolbox to create the toolbox snippet
    - Once the two snippets are created in the toolbox, delete them from MainPage.cs
1. Make sure the webcam capability is enabled in the manifest
1. Make sure "Microsoft.Toolkit.Uwp.UI.Controls" nuget package is installed
    > Note: the CameraPreview control needed for this demo will be available in the 3.0 release of the toolkit which will be released by the time you will be doing the demo. To practice today, use the pre-release packages found [here](https://1drv.ms/u/s!AjZLNGpIZBbgr9Ru0aDufcJ3gaXV2A)

### Demo Steps

#### The model
1. Show what model you will be using in the demo by navigating to the Azure AI model gallery in Edge and showing the Emotion model. Show both the input and output section.
1. Show the model you've downloaded and open it in Netron - show the input and output of the model by clicking on the Input338 box.
1. Drag and drop the Emotion model you pre-downloaded to the root of the project in Visual Studio. This should add the model to the project and create a new cs class in a new file.
1. Show that the auto-generated code is generated using the same input and outputs you saw in Netron and it's using the same name.

#### Hello World ML
1. Make sure to right click on the model and click on Properties to change the **Build Action** to `Content` 
1. Open MainPage.xaml and drag the `CameraPreview` control from the toolbox to the Grid in the page (it will be under the *Windows Community Toolkit* section)
1. Name the control `Camera.` Should look like this: `<my:CameraPreview x:Name="Camera" />`
1. Open MainPage.xaml.cs and drag and drop the first snippet from the toolbox - This snippet initializes the model using the auto-generated code and initializes the CameraPreview control to start receiving frames
1. Drag and drop the second snippet from the toolbox. This is the event handler for a received frame from the camera. The model is called to evaluate each frame.
1. Set a breakpoint on the closing bracket of the event handler and run the app
1. Once the app starts it will hit the breakpoint. Show the value of the `output` variable.

## CustomVision and AlarmML

### Setup

#### Custom Vision
1. Open **Microsoft Edge** and navigate to the [Custom Vision](https://www.customvision.ai) portal.
1. Log in using a valid account
1. If this is the first time visiting the portal, it will request some permissions. Review and agree.
1. Make sure you are on the *Projects* view

#### Visual Studio
1. Open Visual Studio and create a blank new app - you will use this app to demonstate the new model you will generate
1. Open another instance of Visual Studio and open the *AlarmML/AlarmML.sln* solution
1. Open *MainPage.xaml.cs*

### Demo Steps

#### Create a model
1. In the Custom Vision portal click **New Project**. Provide a name of your choosing. Chose the **General (compact)** model. Click **Create Project**
1. Click on **Add images**. 
1. Click on **Browse local files**
1. Browse to the `CustomVisionAssets` and repeat the following steps for each of the three folders
    - Select all images in the folder and click **Open**
    - Under `My Tags` create a new tag using the same name as the chosen folder
    - Click **Upload 8 files**
1. Once all images are upload, click on **Train** in the upper right corner
1. Once the training is complete, click on **Export** and download the model in the *ONNX* format
    > Note: Training might take few minutes and I recommend you have another Custom Vision project that has the model pre-trained to switch to. You should also have the model already downloaded.

#### Use the model Visual Studio
1. In the blank app you created in Visual Studio, drag and drop the new model you created
1. Show the auto-generated file, including the auto-generated dictionary with the tag labels.
1. Switch to the AlarmML project in Visual Studio and run the app.
1. Demonstrate the functionality and how the model is used to snooze the alarm
1. Stop the app and navigate to MainPage.xaml.cs to show the code
1. Show the code for initializing the model
1. Show the code for evaluating the model and how the results and probabilities are used to decide when to snooze the alarm.

> Note: For simplicity, we are not demonstrating the model integration in the AlarmML app. However, if you feel confident in writing the live code on stage, feel free to modify the script and drag and drop the model to the AlarmML app directly (instead of using a blank app first) and write the code on stage to evaluate the model.