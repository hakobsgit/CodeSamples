
using System;
using System.Collections;
using Controllers;
using Data.Models;
using ScriptableObjects;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;

public class ContentService {
    [Inject] private ContentSetupData _contentSetupData;
    [Inject] private ProgressPopupController _progressPopupController;

    private IDisposable _coroutine;
    private IDisposable _timerDisposable;
    private Action _onRetry;
    private int _index;
    
    private ReactiveProperty<float> _progress = new ReactiveProperty<float>();
    
    public void InitStartupContent(Action onFinish, bool showProgress = true) {
        ContentDependency[] dependencies = _contentSetupData.StartupContentData.Dependencies;
        _onRetry = () => InitStartupContent(onFinish, showProgress);
        _coroutine = Observable.FromCoroutine(() => InitContent(dependencies, onFinish, showProgress)).Subscribe();
    }

    public void InitExpoContent(Action onFinish, bool showProgress = true) {
        ContentDependency[] dependencies = _contentSetupData.ExpoContentData.Dependencies;
        _onRetry = () => InitStartupContent(onFinish, showProgress);
        _coroutine = Observable.FromCoroutine(() => InitContent(dependencies, onFinish, showProgress)).Subscribe();
    }

    private IEnumerator InitContent(ContentDependency[] dependencies, Action onFinish, bool showProgress = true) {
        float stepProgress = 1f / dependencies.Length;
        _index = 0;
        _timerDisposable = Observable.Timer(TimeSpan.FromSeconds(2f)).Subscribe(_ => {
            if (showProgress) {
                _progressPopupController.Show(_progress, dependencies[_index].ProgressInfo);
            }
        });
        for (int i = 0; i < dependencies.Length; i++) {
            _index = i;
            bool waiting = true;
            _progressPopupController.SetInfo(dependencies[i].ProgressInfo);
            AsyncOperationHandle handle = Addressables.DownloadDependenciesAsync(dependencies[i].Id);
            handle.Completed += operationHandle => {
                if (operationHandle.Status == AsyncOperationStatus.Failed) {
                    _timerDisposable?.Dispose();
                    _coroutine?.Dispose();
                    _progressPopupController.SetError(() => {
                        _progressPopupController.Close();
                        Observable.Timer(TimeSpan.FromSeconds(0.25f)).Subscribe(_ => {
                            _onRetry?.Invoke();
                        });
                    });
                }
                else {
                    waiting = false;
                }
            };
            while (waiting) {
                _progress.Value = i * stepProgress + stepProgress * handle.PercentComplete;
                yield return null;
            }
        }

        yield return null;
        _timerDisposable.Dispose();
        _progressPopupController.Close();
        yield return new WaitForSeconds(0.25f);
        onFinish?.Invoke();
    }
}