using System;
using System.Collections.Generic;

namespace GameState {
    public class StateManager<TStore> {
        public TStore _store { get; private set; }
        private Storage<TStore> _storage = new Storage<TStore>();

        private class EventListeners : Dictionary<string, Dictionary<string, Action<TStore>>> {};                               
        private EventListeners _listeners = new EventListeners();

        public StateManager(TStore store) {
            this._store = store;
        }
        
        public void DispatchAction(IAction<TStore> action) {
            action.Apply(_store, Notify);
        }

        public void Notify(string eventName) {
            if (_listeners.ContainsKey(eventName)) {
                foreach (var listener in _listeners[eventName]) {
                    listener.Value(_store);
                }
            }
        }

        public Action AddEventListener(string eventName, Action<TStore> handler, bool immediate = false) {
            var handlerId = System.Guid.NewGuid().ToString();
            
            if (!_listeners.ContainsKey(eventName)) {
                _listeners[eventName] = new Dictionary<string, Action<TStore>>();
            }

            _listeners[eventName][handlerId] = handler;

            // optionally call immediately
            if (immediate) {
                _listeners[eventName][handlerId](_store);
            }

            // remove listener:
            return () => { _listeners[eventName].Remove(handlerId); };
        }
        
        public void SaveToFile(string fileName) {
            _storage.Save(fileName, _store);
        }

        public void LoadFromFile(string fileName) {
            var store = _storage.Load(fileName);
            if (store == null) {
                // could not load
            } else {
                _store = store;
                // fire all registered listeners since the entire _store has been replaced without an action
                foreach (var prop in _listeners) {
                    foreach (var listenerId in _listeners[prop.Key]) {
                        listenerId.Value(_store);
                    }
                }
            }
        }
    }

    public class NotifyEventArgs<TStore> : EventArgs {
        public TStore Store;
    }
}