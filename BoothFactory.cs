using System;
using System.Collections.Generic;
using Data.Addressable;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace DI.Factories {
    public class BoothFactory {
        private readonly DiContainer _container;

        private readonly IDictionary<string, AddressableData> _references = new Dictionary<string, AddressableData>();

        [Inject]
        public BoothFactory(DiContainer container) {
            _container = container;
        }
    
        public void Create(string id, Transform point, Action<Booth> onComplete) {
            if (_references.ContainsKey(id)) {
                if (_references[id] != null && _references[id].Reference.IsDone) {
                    InstantiateBooth(id, point, onComplete);
                }
                else {
                    Observable.EveryEndOfFrame().TakeWhile(_ => _references[id] == null || !_references[id].Handle.IsDone).Subscribe(_ => {}, () => {
                        InstantiateBooth(id, point, onComplete);
                    });
                }
                return;
            }
            _references.Add(id, null);
            AssetReference reference = new AssetReference($"booth_{id}");
            reference.LoadAssetAsync<GameObject>().Completed += _ => {
                _references[id] = new AddressableData {
                    Reference = reference,
                    Handle = _
                };
                InstantiateBooth(id, point, onComplete);
            };
        }

        public void InstantiateBooth(string id, Transform point, Action<Booth> onComplete) {
            AddressableData data = _references[id];
            GameObject instantiatedObject = _container.InstantiatePrefab(data.Handle.Result, point);
            data.LinkedObjects.AddLast(instantiatedObject);
            Booth booth = instantiatedObject.GetComponent<Booth>();
            booth.OnDestroyed += () => {
                data.LinkedObjects.Remove(instantiatedObject);
                UpdateAddressableData(id);
            };
            onComplete?.Invoke(booth);
        }

        private void UpdateAddressableData(string id) {
            AddressableData data = _references[id];
            if (data.LinkedObjects.Count == 0) {
                data.Reference.ReleaseAsset();
                _references.Remove(id);
            }
        }
    }
}
