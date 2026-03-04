using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace ZE.Flags 
{ 
    public class FlagsTest : MonoBehaviour
    {
        public class FlagA : IFlag { }
        public class FlagB : IFlag { }

        [SerializeField] private int _activeFlagsA = 0;
        [SerializeField] private int _activeFlagsB = 0;

        private IFlagsManager _flagsManager;
        private CompositeDisposable _compositeDisposable = new();
        private List<FlagA> _flagsA = new();
        private List<IDisposable> _flagsB = new();

        private GUIStyle _style;
    

        private void Start()
        {
            _flagsManager = new FlagsManager();

            for (var i = 0; i < _activeFlagsA; i++)
                AddFlagA();

            for (var i = 0; i< _activeFlagsB; i++)
                AddFlagBSub();

            Observable.CombineLatest(
                _flagsManager.GetFlagObserver<FlagA>(),
                _flagsManager.GetFlagObserver<FlagB>(),
                (bool a,bool b) => a & b)                
                .Where(x => x == true)
                .DistinctUntilChanged()
                .Subscribe(_ => Debug.Log($"both flags active"))
                .AddTo(_compositeDisposable);

            _flagsManager.Subscribe<FlagA>(x => Debug.Log($"flag A: {x}")).AddTo(_compositeDisposable);
            _flagsManager.Subscribe<FlagB>(x => Debug.Log($"flag B: {x}")).AddTo(_compositeDisposable);

            _style = new() { fontSize = 60};
        }

        private void OnDestroy()
        {
            foreach (var bSubs in _flagsB)
                bSubs.Dispose();

            _compositeDisposable.Dispose();
            _flagsManager.Dispose();
        }

        private void OnGUI()
        {
            if (GUILayout.Button("add " + nameof(FlagA), _style))
                AddFlagA();

            // flag instance remove from manager
            if (_flagsManager.IsFlagActive<FlagA>() && GUILayout.Button("remove " + nameof(FlagA), _style))
                RemoveFlagA();

            if (GUILayout.Button("add " + nameof(FlagB), _style))
                AddFlagBSub();

            
            if (_flagsManager.IsFlagActive<FlagB>() && GUILayout.Button("remove " + nameof(FlagB), _style))
                RemoveFlagBSub();
        }

        private void AddFlagA()
        {
            // create and add flag instance

            var flag = new FlagA();
            _flagsManager.AddFlag(flag);
            _flagsA.Add(flag);
            _activeFlagsA++;
        }

        private void RemoveFlagA()
        {
            var lastIndex = _flagsA.Count - 1;
            var flag = _flagsA[lastIndex];
            _flagsManager.RemoveFlag(flag);
            _flagsA.RemoveAt(lastIndex);
            _activeFlagsA--;
        }

        private void AddFlagBSub()
        {
            // handle flag as subscription. When sub is disposed, flag will be removed

            var flagSubscription = _flagsManager.AddTemporalFlag<FlagB>();
            _flagsB.Add(flagSubscription);
            _activeFlagsB++;
        }

        private void RemoveFlagBSub()
        {
            var lastIndex = _flagsB.Count - 1;
            var flag = _flagsB[lastIndex];
            _flagsB[lastIndex].Dispose();
            _flagsB.RemoveAt(lastIndex);
            _activeFlagsB--;
        }
    }
}
