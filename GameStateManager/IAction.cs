using System;

namespace GameState {
    public interface IAction<TStore> {
        void Apply(TStore store, Action<string> notify);
    }
}