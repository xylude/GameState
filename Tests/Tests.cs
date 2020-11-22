using System;
using System.Threading;
using NUnit.Framework;
using GameState;

namespace Tests {
    [TestFixture]
    public class Tests {
        // does the action.Apply() method change the store:
        [Test]
        public void Test1() {
            var store = new Store();
            var stateManager = new StateManager<Store>(store);
            stateManager.DispatchAction(new MyAction() {
                name = "Hambob"
            });
            
            Assert.That(store.name, Is.EqualTo("Hambob"));
        }

        // does the event callback fire, this also tests GameState.Notify method
        [Test]
        public void Test2() {
            var name = "Phil";
            var store = new Store();
            var stateManager = new StateManager<Store>(store);
            var removeEventListener = stateManager.AddEventListener("NameChanged", (Store _store) => {
                name = _store.name;
            });
            
            stateManager.DispatchAction(new MyAction() {
                name = "Hambob"
            });

            // give it time for the event listener to fire
            Thread.Sleep(500);
            
            Assert.AreEqual("Hambob", name);

            removeEventListener();
            
            stateManager.DispatchAction(new MyAction() {
                name = "Moonunit"
            });

            // give it time for the event listener to fire
            Thread.Sleep(500);
            
            Assert.AreEqual("Hambob", name);
        }
    }
    
    public class MyAction : IAction<Store> {
        public string type => "MyAction";
        public string name;
        
        public void Apply(Store store, Action<string> notify) {
            store.name = name;
            notify("NameChanged");
        }
    }
    
    public class Store {
        public string name;
        public int age;
    }
}