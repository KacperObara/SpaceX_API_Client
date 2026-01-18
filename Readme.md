# SpaceX API client IOS/Android (Unity) 
A technical showcase of a scalable Unity architecture featuring a SpaceX REST API client and an Earth movement simulation.

## Features
- Hierarchical State Machine Architecture (HSM)
- MVP pattern for in-state architecture
- Pure C# Classes, minimum MonoBehaviours
- Designed with UniTask in mind
- VContainer for Dependency Injection (DI)
- Unit tests and End-to-End tests (E2E)

## What issues does it solve?

Creating game prototypes in Unity comes with unique problems:
- Rapid prototyping prioritizes speed over structure. It leads to technical debt as the project is iterated.
- Every part of a prototype might need to be changed or even replaced as it grows. There's no way to predict and prepare for everything.
- It's difficult for teams to work on the same project.
- Making a project testable is difficult in Unity and many developers ignore tests because of it.

It seems like an impossible problem.  
This project explores how to design Unity applications that can solve these issues.

## Overview
The project starts with a loading state. When loading is successful, it enters the main menu. From there, the user can either browse a database of SpaceX rocket launches and read about payloads, or enter the Solar Simulation.  

The simulator shows real data of Earth’s position around the Sun, with real-world scale for the distance between the star and the planet.

<div align="center">
  <video src="https://github.com/user-attachments/assets/4196edfb-b852-4378-bce1-b8c81a760b5c"></video>
</div>

## Technical Stack

- **UniTask** - For efficient async/await operations
- **VContainer** - Dependency Injection framework
- **Newtonsoft.Json** - JSON serialization/deserialization
- **DOTween** - Smooth animations and transitions

## Architecture diagram
![SpaceXDiagram](https://github.com/user-attachments/assets/2c62e811-5fba-4dea-b0ab-cfb5907534f4)


The architecture is based on a Hierarchical State Machine. 

## Why Hierarchical State Machine (HSM)?
At first glance, HSMs may look complicated, but the idea is the opposite. The goal is to keep everything linear and understandable.
It follows concrete rules:  
- States create child states.
- States can exit and return control to their parents.
This allows for full modularity.  

By looking at a stack at any given time, we should know exactly what the player is doing.  
For example `Root->MainMenu->Launches->PayloadPopup`
I've seen many implementations of HSM, but they were always hard to understand at a glance. That’s why I made my own and documented `StateMachine.cs` to the best of my abilities. 
In short, its job is to push and pop states when requested by other states. 

## What is a module?
A module is a separate part of the project. Each one has its own LifetimeScope (VContainer) for injecting references.

The core of each module is a pure C# state script. Its role is to decide when to spawn children or return to the parent, as well as initialize logic within the state (such as presenters).
It also contains a custom update loop that ensures states are updated only when active.

What should be a module is ultimately a design choice. For me, modules should be as small as possible within reason.
For example, `LoadingErrorModule` could be part of `BootModule`, and `PayloadPopupModule` could be part of `LaunchesModule`.

There are two reasons they are not:
1. Well-separated code is easier to reuse. A loading error module can be transferred to any project as a generic loading error handler.
2. I needed more modules to showcase different challenges—for example, an error state returning to its parent to retry loading, but failing again when data is unavailable.


## Isn't this overengineering? 
For a project this small, yes.  
But if the project grows, there will be almost no additional complexity. You can add or remove modules, and the only thing other modules need to know is when to instantiate or dispose of them.

## (DI) Why did I choose VContainer and why use DI at all?
Originally, the plan was to use Extenject (Zenject), but I quickly realized there were better solutions for my needs.

Zenject is a very large framework, to the point where it introduces noticeable overhead.
Its installers also don’t work well when stored as optional prefabs.

Most importantly, VContainer is far less complex and much easier to understand.

## Why UniTask instead of Coroutines?
1. Better performance and fewer GC allocations
2. Used often by companies, easier to get hired
3. Can return values easily
4. Personally, I think UniTask look simpler

One drawback is that UniTasks are not tied to game execution. This means that after stopping Play Mode, tasks may still be running.
The problem is even worse when Domain Reload is disabled to reduce compilation times.

However, this forces developers to be diligent and properly cancel UniTask methods.

## Why pure C# classes?
Decoupling from monobehaviours has some benefits: 
- It helps with separating the code, because not everything needs to be monobehaviour.
- The code is cleaner.

One issue arises when code needs to run over several frames, since there are no coroutines. However, because I replaced coroutines entirely with UniTask, this is no longer a problem.

I’m still a big fan of the component-based pattern Unity is built on, but this approach reduces clutter when attaching a script to a GameObject isn’t necessary.
Managing states and their transitions provides no benefit from being tied to components, although in this architecture, states are still connected to their modules, which are prefabs.

## Why does almost every script derive from an interface?
1. Reducing dependencies
2. Works better with DI
3. **The most important** Makes implementations swappable. For this project it's used primarily for mocking in unit tests. But it promotes the modular architecture I'm trying to achieve.

## Unit tests
This project includes both standard unit tests and End-to-End (E2E) tests to demonstrate how the architecture supports testing.

Because as many scripts as possible are pure C# and derive from interfaces, writing tests and mocking data is easy.

# Going through modules one by one

## Root Module
`RootState.cs` shows basics of state transitions. It starts by pushing BootState immediately. When BootState returns, MainMenu is pushed right after.
`RootLifetimeScope.cs` showcases how to bind different data. 
OrbitalDataService and LaunchDataService are stored here, because they need to be accessed by both BootState when loading and Launches/SolarSimulation states. Registering data here is available only to self and children, not parents.

## Boot Module
`BootState.cs` handles loading the data. Why here? Because other games have loading at startup. It complicates the project a little so it's good for the showcase.

Loading is divided into 2 parts:
1. `LaunchDataLoader.cs` requests data from `api.spacexdata.com`. It loads this data into a service that will be used later by multiple modules. Note that it downloads only required data, through minimum number of queries for highest efficiency. It also showcases how CancellationToken is used to stop loading when an application is closed.
2. `OrbitalDataLoader.cs` loads Earth data from csv file and converts orbital data into scaled-down Vector3

Data is loaded to the services provided by the root scope.
When loading fails, either when there is no internet connection or API does not respond, Loading Error State is called.
`BootLifetimeScope.cs` looks similar to RootState. OrbitalDataLoader and LaunchDataLoader can be seen here. They load data, but when we leave BootState, the loaders are disposed of.
`BootView.cs` First monobehaviour. It handles the loading bar. Presenter and Model does not exist, because they would only add bloat for this simple view.

## Loading Error Module
Nothing unique happens here. When the user presses the retry button, the module exits back to BootState, which restarts the loading process.

## Main Menu Module
`MainMenuPresenter.cs` is the first use of a presenter. It connects UI buttons between the state and the view.
I dislike wiring buttons through the Inspector, as it makes debugging harder later.
Also in `MainMenuView.cs` the Hide method hides UI, because when entering a child state, the parent state remains active.

## Launches Module
`LaunchesState.cs` demonstrates how to pass custom data to a child state. When clicking a rocket launch, payload data is passed to `PayloadPopupState.cs`.  
`LaunchesView.cs` handles the large list of launches. It uses a technique I wanted to share: dynamic list loading.  
Instead of creating entries for every item, only a few reusable entries exist, allowing the list to support infinite size.
That means, there's always only a few entries that can handle a list of an infinite size.
As the user scrolls, the position is reset behind the scenes and new data is loaded into the same entries.

## Payload Popup Module
Handles displaying data passed from `LaunchesState.cs`

## Solar Simulation Module
The project needed something more complex for the showcase, so I created a simple solar simulation.  

The game allows zooming out, Earth rotates around the Sun using real-world data, and the distance between Earth and the Sun is correctly scaled.  

This module showcases input handling, a custom update loop, and the complexity found in larger modules with multiple scripts.
`SolarSimulationState.cs` gets injected references to components that require an update loop.
