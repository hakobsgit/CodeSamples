using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ReconnectPopup : BasePopup {
    [SerializeField] private Button _cancelButton;

    private IDisposable _disposable;
    private Action _onCancel;
    
    public void Initialize(Action onCancel) {
        _onCancel = onCancel;
    }
    
    private void Start() {
        _cancelButton.onClick.AddListener(() => _onCancel?.Invoke());
        _disposable = Observable.Timer(TimeSpan.FromSeconds(2)).Subscribe(_ => { _cancelButton.gameObject.SetActive(true); });
    }

    private void OnDestroy() {
        _disposable?.Dispose();
    }
}