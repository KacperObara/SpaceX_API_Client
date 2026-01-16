using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Modules.SolarSimulationModule;
using NUnit.Framework;

public class SolarSimulationPresenterTests
{
    [Test]
    public Task Presenter_Waits_For_Return_Click()
    {
        var view = new FakeSolarSimulationView();
        var presenter = new SolarSimulationPresenter(view);
        
        bool exitRequestedFired = false;
        
        presenter.ExitRequested += () => 
        {
            exitRequestedFired = true;
            return UniTask.CompletedTask;
        };
        
        view.SimulateReturnClick();
        
        Assert.IsTrue(exitRequestedFired, "Presenter did not fire ExitRequested event when view was clicked.");
        return Task.CompletedTask;
    }
    
    [Test]
    public async Task Presenter_Hide_CallsViewHide()
    {
        var view = new FakeSolarSimulationView();
        var presenter = new SolarSimulationPresenter(view);
        
        await presenter.Hide();
        
        Assert.IsTrue(view.HideCalled, "Presenter.Hide() did not call View.Hide()");
    }
    
    
    private class FakeSolarSimulationView : ISolarSimulationView
    {
        public event Action ReturnClicked;

        public bool HideCalled { get; private set; }

        public UniTask Hide()
        {
            HideCalled = true;
            return UniTask.CompletedTask;
        }

        public void SimulateReturnClick()
        {
            ReturnClicked?.Invoke();
        }
    }
}




