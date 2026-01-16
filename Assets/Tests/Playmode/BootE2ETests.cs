using System.Collections;
using Modules.BootModule;
using Modules.LoadingErrorModule;
using Modules.MainMenuModule;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class BootE2ETests
{
    private const float MaxWaitTime = 10f;
    private float _timer;
    
    [UnitySetUp]
    public IEnumerator SetUp()
    {
        yield return SceneManager.LoadSceneAsync("MainScene", LoadSceneMode.Single);
    }

    [UnityTest]
    public IEnumerator Boot_Completes_And_Shows_MainMenu()
    {
        // Wait until loading finishes and MainMenuView is ready
        _timer = 0;
        yield return new WaitUntil(() =>
        {
            _timer += Time.deltaTime;
            if (_timer > MaxWaitTime)
                Assert.Fail("Loading MainMenuView took too long");
            
            var main = Object.FindFirstObjectByType<MainMenuView>();
            return main != null && main.gameObject.activeSelf;
        });

        var mainMenu = Object.FindFirstObjectByType<MainMenuView>(FindObjectsInactive.Exclude);
        Assert.IsTrue(mainMenu.gameObject.activeSelf, "MainMenu should be active after boot.");
    }
}